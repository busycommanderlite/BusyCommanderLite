package main

import "C"
import (
	"bytes"
	"context"
	"encoding/base64"
	"encoding/json"
	"fmt"
	"image"
	"image/color"
	"image/draw"
	"image/jpeg"
	_ "image/png"
	"net/http"
	"os"
	"path/filepath"
	"strings"
	"sync"
	"time"

	"github.com/liyue201/goqr"
	"github.com/skip2/go-qrcode"
	_ "github.com/mattn/go-sqlite3"
	"go.mau.fi/whatsmeow"
	"go.mau.fi/whatsmeow/proto/waE2E"
	"go.mau.fi/whatsmeow/store/sqlstore"
	"go.mau.fi/whatsmeow/types"
	"go.mau.fi/whatsmeow/types/events"
	waLog "go.mau.fi/whatsmeow/util/log"
	"google.golang.org/protobuf/proto"
)

var (
	server       *http.Server
	waClient     *whatsmeow.Client
	container    *sqlstore.Container
	clientLog    waLog.Logger
	pairingMutex sync.Mutex
	isPairing    bool
	mu           sync.Mutex
	qrCodeB64    string
	isLogged     bool
	jidCache     = make(map[string]types.JID)
	jidCacheMu   sync.RWMutex
)

func getCachedJID(phone string) (types.JID, bool) {
	jidCacheMu.RLock()
	defer jidCacheMu.RUnlock()
	jid, ok := jidCache[phone]
	return jid, ok
}

func setCachedJID(phone string, jid types.JID) {
	jidCacheMu.Lock()
	defer jidCacheMu.Unlock()
	jidCache[phone] = jid
}

func normalizePhoneNumber(phone string) string {
	var digits strings.Builder
	for _, ch := range phone {
		if ch >= '0' && ch <= '9' {
			digits.WriteRune(ch)
		}
	}
	cleanNum := digits.String()
	if len(cleanNum) == 10 {
		cleanNum = "91" + cleanNum
	} else if len(cleanNum) == 11 && strings.HasPrefix(cleanNum, "0") {
		cleanNum = "91" + cleanNum[1:]
	}
	return cleanNum
}

func registerClientHandlers(client *whatsmeow.Client) {
	client.AddEventHandler(func(rawEvt interface{}) {
		switch evt := rawEvt.(type) {
		case *events.Connected:
			mu.Lock()
			isLogged = true
			qrCodeB64 = ""
			mu.Unlock()
			fmt.Println("WhatsApp event: Connected successfully")

		case *events.LoggedOut:
			fmt.Printf("WhatsApp event: LoggedOut (Reason: %v, OnConnect: %v). Resetting session...\n", evt.Reason, evt.OnConnect)
			mu.Lock()
			isLogged = false
			qrCodeB64 = ""
			mu.Unlock()
			jidCacheMu.Lock()
			jidCache = make(map[string]types.JID)
			jidCacheMu.Unlock()
			go startPairingFlow()

		case *events.StreamReplaced:
			fmt.Println("WhatsApp event: StreamReplaced by another device connection")
			mu.Lock()
			isLogged = false
			mu.Unlock()
		}
	})
}

func startPairingFlow() {
	pairingMutex.Lock()
	if isPairing {
		pairingMutex.Unlock()
		return
	}
	isPairing = true
	pairingMutex.Unlock()

	defer func() {
		pairingMutex.Lock()
		isPairing = false
		pairingMutex.Unlock()
	}()

	for {
		mu.Lock()
		if isLogged {
			mu.Unlock()
			return
		}
		qrCodeB64 = ""
		mu.Unlock()

		if container == nil {
			return
		}

		if waClient != nil {
			if waClient.IsConnected() {
				waClient.Disconnect()
			}
			if waClient.Store != nil && !waClient.Store.Deleted {
				_ = waClient.Store.Delete(context.Background())
			}
		}

		deviceStore, err := container.GetFirstDevice(context.Background())
		if err != nil {
			fmt.Println("Failed to get fresh device store:", err)
			return
		}

		waClient = whatsmeow.NewClient(deviceStore, clientLog)
		registerClientHandlers(waClient)

		qrChan, err := waClient.GetQRChannel(context.Background())
		if err != nil {
			fmt.Println("Failed to get QR channel:", err)
			time.Sleep(2 * time.Second)
			continue
		}

		err = waClient.Connect()
		if err != nil {
			fmt.Println("Failed to connect for QR pairing:", err)
			time.Sleep(2 * time.Second)
			continue
		}

		timedOut := false
		for evt := range qrChan {
			if evt.Event == "code" {
				png, err := qrcode.Encode(evt.Code, qrcode.Medium, 256)
				if err == nil {
					mu.Lock()
					qrCodeB64 = base64.StdEncoding.EncodeToString(png)
					isLogged = false
					mu.Unlock()
					fmt.Println("New QR code generated")
				}
			} else if evt.Event == "success" {
				fmt.Println("Login successful from QR scan")
				mu.Lock()
				isLogged = true
				qrCodeB64 = ""
				mu.Unlock()
				return
			} else if evt.Event == "timeout" {
				fmt.Println("QR code timeout, refreshing pairing flow...")
				timedOut = true
				break
			}
		}

		if !timedOut {
			mu.Lock()
			logged := isLogged
			mu.Unlock()
			if logged {
				return
			}
			time.Sleep(1 * time.Second)
		}
	}
}

func getDatabaseDSN() string {
	const params = "_foreign_keys=on&_journal_mode=WAL&_busy_timeout=5000"
	busyWhatsappDir := `C:\BusyWin\WhatsApp`
	if info, err := os.Stat(busyWhatsappDir); err == nil && info.IsDir() {
		absDb := filepath.Join(busyWhatsappDir, "store.db")
		absDbSlash := filepath.ToSlash(absDb)
		return fmt.Sprintf("file:%s?%s", absDbSlash, params)
	}
	return fmt.Sprintf("file:store.db?%s", params)
}

func initWhatsApp() {
	if waClient != nil {
		return
	}

	dbLog := waLog.Stdout("Database", "DEBUG", true)
	dsn := getDatabaseDSN()
	fmt.Printf("Connecting to SQLite database at: %s\n", dsn)
	var err error
	container, err = sqlstore.New(context.Background(), "sqlite3", dsn, dbLog)
	if err != nil {
		fmt.Println("Failed to connect to database:", err)
		return
	}

	deviceStore, err := container.GetFirstDevice(context.Background())
	if err != nil {
		fmt.Println("Failed to get device:", err)
		return
	}

	clientLog = waLog.Stdout("Client", "DEBUG", true)

	if deviceStore.ID == nil {
		// No session stored, begin pairing immediately
		go startPairingFlow()
	} else {
		// Existing session stored, connect and wait for Connected or LoggedOut event
		waClient = whatsmeow.NewClient(deviceStore, clientLog)
		registerClientHandlers(waClient)

		err = waClient.Connect()
		if err != nil {
			fmt.Println("Failed to connect existing session:", err)
			return
		}
	}
}

func createThumbnail(imgData []byte) []byte {
	src, _, err := image.Decode(bytes.NewReader(imgData))
	if err != nil {
		return nil
	}
	bounds := src.Bounds()
	w, h := bounds.Dx(), bounds.Dy()
	if w <= 0 || h <= 0 {
		return nil
	}

	maxDim := 100
	var targetW, targetH int
	if w > h {
		targetW = maxDim
		targetH = (h * maxDim) / w
	} else {
		targetH = maxDim
		targetW = (w * maxDim) / h
	}
	if targetW < 1 {
		targetW = 1
	}
	if targetH < 1 {
		targetH = 1
	}

	dst := image.NewRGBA(image.Rect(0, 0, targetW, targetH))
	draw.Draw(dst, dst.Bounds(), &image.Uniform{color.White}, image.Point{}, draw.Src)

	for y := 0; y < targetH; y++ {
		for x := 0; x < targetW; x++ {
			srcX := bounds.Min.X + (x*w)/targetW
			srcY := bounds.Min.Y + (y*h)/targetH
			dst.Set(x, y, src.At(srcX, srcY))
		}
	}

	var buf bytes.Buffer
	if err := jpeg.Encode(&buf, dst, &jpeg.Options{Quality: 50}); err != nil {
		return nil
	}
	return buf.Bytes()
}

func resolveJID(phone string) types.JID {
	if strings.HasSuffix(phone, "@g.us") {
		jid, _ := types.ParseJID(phone)
		return jid
	}

	rawNumber := phone
	if strings.Contains(phone, "@") {
		parts := strings.Split(phone, "@")
		rawNumber = parts[0]
	}
	cleanNum := normalizePhoneNumber(rawNumber)
	if cleanNum == "" {
		jid, _ := types.ParseJID(phone)
		return jid
	}

	if cached, ok := getCachedJID(cleanNum); ok {
		return cached
	}

	if waClient != nil && waClient.IsConnected() {
		ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
		resp, err := waClient.IsOnWhatsApp(ctx, []string{"+" + cleanNum})
		cancel()
		if err == nil && len(resp) > 0 && resp[0].IsIn && !resp[0].JID.IsEmpty() {
			if resp[0].JID.String() != cleanNum+"@"+types.DefaultUserServer {
				fmt.Printf("Canonical JID resolution: %s -> %s\n", cleanNum, resp[0].JID.String())
			}
			setCachedJID(cleanNum, resp[0].JID)
			return resp[0].JID
		}
	}

	if strings.Contains(phone, "@") {
		jid, _ := types.ParseJID(phone)
		setCachedJID(cleanNum, jid)
		return jid
	}
	targetJID := types.NewJID(cleanNum, types.DefaultUserServer)
	setCachedJID(cleanNum, targetJID)
	return targetJID
}

func wrapMessage(innerMsg *waE2E.Message, isEphemeral bool, expiration uint32, jid types.JID) *waE2E.Message {
	if !isEphemeral && expiration == 0 {
		return innerMsg
	}
	if expiration == 0 {
		expiration = 86400 // Default to 24 hours (86400 seconds)
	}

	ctxInfo := &waE2E.ContextInfo{
		Expiration:                proto.Uint32(expiration),
		EphemeralSettingTimestamp: proto.Int64(time.Now().Unix()),
		DisappearingMode: &waE2E.DisappearingMode{
			Initiator: waE2E.DisappearingMode_CHANGED_IN_CHAT.Enum(),
			Trigger:   waE2E.DisappearingMode_CHAT_SETTING.Enum(),
		},
	}

	if innerMsg.ExtendedTextMessage != nil {
		innerMsg.ExtendedTextMessage.ContextInfo = ctxInfo
	} else if innerMsg.Conversation != nil {
		innerMsg.ExtendedTextMessage = &waE2E.ExtendedTextMessage{
			Text:        innerMsg.Conversation,
			ContextInfo: ctxInfo,
		}
		innerMsg.Conversation = nil
	} else if innerMsg.ContactMessage != nil {
		innerMsg.ContactMessage.ContextInfo = ctxInfo
	} else if innerMsg.ImageMessage != nil {
		innerMsg.ImageMessage.ContextInfo = ctxInfo
	} else if innerMsg.DocumentMessage != nil {
		innerMsg.DocumentMessage.ContextInfo = ctxInfo
	}

	return &waE2E.Message{
		EphemeralMessage: &waE2E.FutureProofMessage{
			Message: innerMsg,
		},
	}
}

//export StartServer_Go
func StartServer_Go() {
	defer func() { recover() }()
	if server != nil {
		return
	}

	// Init WhatsApp Client on start in the background
	go initWhatsApp()
	
	mux := http.NewServeMux()
	
	mux.HandleFunc("/ping", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintf(w, "pong")
	})

	mux.HandleFunc("/status", func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "application/json")
		mu.Lock()
		defer mu.Unlock()
		
		status := "DISCONNECTED"
		if waClient != nil {
			if isLogged {
				status = "LOGGED_IN"
			} else if qrCodeB64 != "" || (waClient.Store != nil && waClient.Store.ID == nil) {
				status = "WAITING_FOR_QR"
			} else {
				status = "CONNECTING"
			}
		}
		
		json.NewEncoder(w).Encode(map[string]interface{}{
			"status": status,
			"qr": qrCodeB64,
		})
	})

	mux.HandleFunc("/logout", func(w http.ResponseWriter, r *http.Request) {
		if waClient != nil {
			waClient.Logout(context.Background())
			mu.Lock()
			isLogged = false
			qrCodeB64 = ""
			mu.Unlock()
			jidCacheMu.Lock()
			jidCache = make(map[string]types.JID)
			jidCacheMu.Unlock()
			go startPairingFlow()
			fmt.Fprintf(w, "logged out")
		}
	})

	mux.HandleFunc("/groups", func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "application/json")
		if waClient == nil || !waClient.IsConnected() {
			http.Error(w, "Not connected", http.StatusServiceUnavailable)
			return
		}
		
		groups, err := waClient.GetJoinedGroups(context.Background())
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		
		groupMap := make(map[string]string)
		for _, group := range groups {
			groupMap[group.JID.String()] = group.Name
		}
		
		json.NewEncoder(w).Encode(groupMap)
	})

	mux.HandleFunc("/check", func(w http.ResponseWriter, r *http.Request) {
		if waClient == nil || !waClient.IsConnected() {
			http.Error(w, "Not connected", http.StatusServiceUnavailable)
			return
		}
		
		number := r.URL.Query().Get("number")
		if number == "" {
			http.Error(w, "Missing number", http.StatusBadRequest)
			return
		}

		cleanNum := normalizePhoneNumber(number)
		if cleanNum == "" {
			json.NewEncoder(w).Encode(map[string]bool{"isValid": false})
			return
		}

		if cached, ok := getCachedJID(cleanNum); ok {
			json.NewEncoder(w).Encode(map[string]bool{"isValid": !cached.IsEmpty()})
			return
		}

		ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
		defer cancel()

		resp, err := waClient.IsOnWhatsApp(ctx, []string{"+" + cleanNum})
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		
		isValid := false
		if len(resp) > 0 {
			isValid = resp[0].IsIn
			if isValid && !resp[0].JID.IsEmpty() {
				setCachedJID(cleanNum, resp[0].JID)
			}
		}
		
		json.NewEncoder(w).Encode(map[string]bool{"isValid": isValid})
	})

	mux.HandleFunc("/profile_pic", func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "application/json")
		if waClient == nil || !isLogged || !waClient.IsConnected() {
			http.Error(w, "Not connected", http.StatusServiceUnavailable)
			return
		}

		phone := r.URL.Query().Get("phone")
		if phone == "" {
			http.Error(w, "Missing phone", http.StatusBadRequest)
			return
		}

		targetJID := resolveJID(phone)

		// Try to get full resolution image first
		picInfo, err := waClient.GetProfilePictureInfo(context.Background(), targetJID, &whatsmeow.GetProfilePictureParams{
			Preview: false,
		})
		// Fallback to preview thumbnail if full resolution is unavailable
		if err != nil || picInfo == nil || picInfo.URL == "" {
			picInfo, err = waClient.GetProfilePictureInfo(context.Background(), targetJID, &whatsmeow.GetProfilePictureParams{
				Preview: true,
			})
		}

		if err != nil || picInfo == nil || picInfo.URL == "" {
			json.NewEncoder(w).Encode(map[string]interface{}{
				"status": "NOT_FOUND",
				"url":    "",
			})
			return
		}

		json.NewEncoder(w).Encode(map[string]interface{}{
			"status": "OK",
			"url":    picInfo.URL,
		})
	})
	
	mux.HandleFunc("/send", func(w http.ResponseWriter, r *http.Request) {
		if waClient == nil || !isLogged || !waClient.IsConnected() {
			http.Error(w, "Not logged in or connected", http.StatusUnauthorized)
			return
		}
		
		var payload struct {
			Phone      string `json:"phone"`
			Message    string `json:"message"`
			Ephemeral  bool   `json:"ephemeral"`
			Expiration uint32 `json:"expiration"`
		}
		if err := json.NewDecoder(r.Body).Decode(&payload); err != nil {
			http.Error(w, "Bad request", http.StatusBadRequest)
			return
		}
		
		// Resolve canonical JID (handles migrated numbers, LIDs, etc.)
		jid := resolveJID(payload.Phone)
		
		msg := &waE2E.Message{
			Conversation: proto.String(payload.Message),
		}
		msg = wrapMessage(msg, payload.Ephemeral, payload.Expiration, jid)
		
		// Send message
		ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
		defer cancel()
		_, err := waClient.SendMessage(ctx, jid, msg)
		if err != nil {
			http.Error(w, fmt.Sprintf("Error sending message: %v", err), http.StatusInternalServerError)
			return
		}
		fmt.Fprintf(w, "Message sent to %s", payload.Phone)
	})

	mux.HandleFunc("/send_contact", func(w http.ResponseWriter, r *http.Request) {
		if waClient == nil || !isLogged || !waClient.IsConnected() {
			http.Error(w, "Not logged in or connected", http.StatusUnauthorized)
			return
		}

		var payload struct {
			Recipient   string `json:"recipient"`
			PhoneNumber string `json:"phone"`
			Name        string `json:"name"`
			Ephemeral   bool   `json:"ephemeral"`
			Expiration  uint32 `json:"expiration"`
		}
		if err := json.NewDecoder(r.Body).Decode(&payload); err != nil {
			http.Error(w, "Bad request", http.StatusBadRequest)
			return
		}

		jid := resolveJID(payload.Recipient)
		cleanNum := normalizePhoneNumber(payload.PhoneNumber)

		vcard := fmt.Sprintf("BEGIN:VCARD\nVERSION:3.0\nFN:%s\nTEL;type=CELL;type=VOICE;waid=%s:+%s\nEND:VCARD",
			payload.Name, cleanNum, cleanNum)

		msg := &waE2E.Message{
			ContactMessage: &waE2E.ContactMessage{
				DisplayName: proto.String(payload.Name),
				Vcard:       proto.String(vcard),
			},
		}
		msg = wrapMessage(msg, payload.Ephemeral, payload.Expiration, jid)

		ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
		defer cancel()
		_, err := waClient.SendMessage(ctx, jid, msg)
		if err != nil {
			http.Error(w, fmt.Sprintf("Failed to send contact: %v", err), http.StatusInternalServerError)
			return
		}
		fmt.Fprintf(w, "Contact sent to %s", payload.Recipient)
	})

	mux.HandleFunc("/send_media", func(w http.ResponseWriter, r *http.Request) {
		if waClient == nil || !isLogged || !waClient.IsConnected() {
			http.Error(w, "Not logged in or connected", http.StatusUnauthorized)
			return
		}
		var payload struct {
			Phone      string `json:"phone"`
			MimeType   string `json:"mimetype"`
			FileName   string `json:"filename"`
			Caption    string `json:"caption"`
			Base64     string `json:"base64"`
			Type       string `json:"type"` // "image" or "document"
			Ephemeral  bool   `json:"ephemeral"`
			Expiration uint32 `json:"expiration"`
		}
		if err := json.NewDecoder(r.Body).Decode(&payload); err != nil {
			http.Error(w, "Bad request", http.StatusBadRequest)
			return
		}

		data, err := base64.StdEncoding.DecodeString(payload.Base64)
		if err != nil {
			http.Error(w, "Invalid base64", http.StatusBadRequest)
			return
		}

		mediaType := whatsmeow.MediaImage
		if payload.Type == "document" {
			mediaType = whatsmeow.MediaDocument
		}

		ctxUpload, cancelUpload := context.WithTimeout(context.Background(), 30*time.Second)
		uploaded, err := waClient.Upload(ctxUpload, data, mediaType)
		cancelUpload()
		if err != nil {
			http.Error(w, fmt.Sprintf("Upload failed: %v", err), http.StatusInternalServerError)
			return
		}

		msg := &waE2E.Message{}
		if payload.Type == "document" {
			msg.DocumentMessage = &waE2E.DocumentMessage{
				Caption:       proto.String(payload.Caption),
				Mimetype:      proto.String(payload.MimeType),
				FileName:      proto.String(payload.FileName),
				Title:         proto.String(payload.FileName),
				URL:           proto.String(uploaded.URL),
				DirectPath:    proto.String(uploaded.DirectPath),
				MediaKey:      uploaded.MediaKey,
				FileEncSHA256: uploaded.FileEncSHA256,
				FileSHA256:    uploaded.FileSHA256,
				FileLength:    proto.Uint64(uint64(len(data))),
			}
		} else {
			thumb := createThumbnail(data)
			msg.ImageMessage = &waE2E.ImageMessage{
				Caption:       proto.String(payload.Caption),
				Mimetype:      proto.String(payload.MimeType),
				URL:           proto.String(uploaded.URL),
				DirectPath:    proto.String(uploaded.DirectPath),
				MediaKey:      uploaded.MediaKey,
				FileEncSHA256: uploaded.FileEncSHA256,
				FileSHA256:    uploaded.FileSHA256,
				FileLength:    proto.Uint64(uint64(len(data))),
				JPEGThumbnail: thumb,
			}
		}

		// Resolve canonical JID (handles migrated numbers, LIDs, etc.)
		jid := resolveJID(payload.Phone)
		msg = wrapMessage(msg, payload.Ephemeral, payload.Expiration, jid)

		ctxSend, cancelSend := context.WithTimeout(context.Background(), 30*time.Second)
		defer cancelSend()
		_, err = waClient.SendMessage(ctxSend, jid, msg)
		if err != nil {
			http.Error(w, fmt.Sprintf("Send failed: %v", err), http.StatusInternalServerError)
			return
		}
		fmt.Fprintf(w, "Media sent to %s", payload.Phone)
	})

	mux.HandleFunc("/set_disappearing", func(w http.ResponseWriter, r *http.Request) {
		if waClient == nil || !isLogged || !waClient.IsConnected() {
			http.Error(w, "Not connected", http.StatusServiceUnavailable)
			return
		}

		var payload struct {
			Phone string `json:"phone"`
			Timer uint32 `json:"timer"` // seconds, e.g. 86400 (24h)
		}
		if err := json.NewDecoder(r.Body).Decode(&payload); err != nil {
			http.Error(w, "Bad request", http.StatusBadRequest)
			return
		}
		if payload.Timer == 0 {
			payload.Timer = 86400
		}

		jid := resolveJID(payload.Phone)
		err := waClient.SetDisappearingTimer(context.Background(), jid, time.Duration(payload.Timer)*time.Second, time.Now())
		if err != nil {
			http.Error(w, fmt.Sprintf("Failed to set disappearing timer: %v", err), http.StatusInternalServerError)
			return
		}

		w.Header().Set("Content-Type", "application/json")
		json.NewEncoder(w).Encode(map[string]interface{}{
			"status": "OK",
			"phone":  payload.Phone,
			"timer":  payload.Timer,
		})
	})

	mux.HandleFunc("/decode_qr", func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "application/json")
		filePath := r.URL.Query().Get("path")
		if filePath == "" {
			w.WriteHeader(http.StatusBadRequest)
			json.NewEncoder(w).Encode(map[string]interface{}{
				"status": "ERROR",
				"error":  "Missing path parameter",
			})
			return
		}

		f, err := os.Open(filePath)
		if err != nil {
			json.NewEncoder(w).Encode(map[string]interface{}{
				"status": "ERROR",
				"error":  fmt.Sprintf("Failed to open file: %v", err),
			})
			return
		}
		defer f.Close()

		img, _, err := image.Decode(f)
		if err != nil {
			json.NewEncoder(w).Encode(map[string]interface{}{
				"status": "ERROR",
				"error":  fmt.Sprintf("Failed to decode image: %v", err),
			})
			return
		}

		qrCodes, err := goqr.Recognize(img)
		if err != nil || len(qrCodes) == 0 {
			json.NewEncoder(w).Encode(map[string]interface{}{
				"status": "ERROR",
				"error":  "No QR code found in image",
			})
			return
		}

		payload := string(qrCodes[0].Payload)
		pa := extractPA(payload)

		json.NewEncoder(w).Encode(map[string]interface{}{
			"status":  "OK",
			"pa":      pa,
			"payload": payload,
		})
	})


	server = &http.Server{Addr: ":18080", Handler: mux}

	go func() {
		defer func() { recover() }()
		fmt.Println("Go WhatsApp server starting on port :18080...")
		if err := server.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			fmt.Printf("Server failed to start: %v\n", err)
		}
	}()
}

func extractPA(raw string) string {
	lower := strings.ToLower(raw)
	if idx := strings.Index(lower, "pa="); idx != -1 {
		sub := raw[idx+3:]
		if endIdx := strings.Index(sub, "&"); endIdx != -1 {
			return sub[:endIdx]
		}
		return sub
	}
	return ""
}

//export StopServer_Go
func StopServer_Go() {
	defer func() { recover() }()
	if waClient != nil {
		waClient.Disconnect()
	}
	if server != nil {
		server.Close()
		server = nil
		fmt.Println("Go server stopped.")
	}
}

func main() {
	StartServer_Go()
	// Block forever
	select {}
}
