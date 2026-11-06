<p align="center">
  <img src="assets/logo.png" width="130" height="130" alt="BusyCommanderLite Logo" />
</p>

# BusyCommanderLite User Manual & Setup Guide 📖

> [!CAUTION]
> **Disclaimer:** Please take a complete backup of your Busy company data and entire system before proceeding. The BusyCommander Team is not responsible for any loss of important data.

---

## 📋 Table of Contents
1. [Initial Setup: WhatsApp / SMS Configuration in Busy](#1-initial-setup-whatsapp--sms-configuration-in-busy)
2. [Sales Register Shortcuts & Workflows](#2-sales-register-shortcuts--workflows)
3. [Dispatch Slips & Gadi No. Tracking](#3-dispatch-slips--gadi-no-tracking)
4. [Bill-wise Statement (Hisab)](#4-bill-wise-statement-hisab)
5. [Account Ledger (Ctrl + L)](#5-account-ledger-ctrl--l)
6. [Account Master & Item Master Utilities](#6-account-master--item-master-utilities)
7. [Automatic Receipt (Jama) Sharing](#7-automatic-receipt-jama-sharing)
8. [Troubleshooting & FAQ](#8-troubleshooting--faq)
9. [Support & Contact](#9-support--contact)

---

## 1. Initial Setup: WhatsApp / SMS Configuration in Busy

To allow Busy to communicate with the local BusyCommander engine and send invoices as WhatsApp messages via the SMS module:

### Step-by-Step Configuration:
1. In **Busy**, press <kbd>Ctrl</kbd> + <kbd>F</kbd> (Configuration Type) &rarr; Select **WhatsApp / SMS Configuration**.
2. Click **Add New Format** (e.g. name it `BCommander` or create two formats: `WaPDF` and `WaImg`).
3. Under the **WhatsApp / SMS API** field, enter one of the following endpoints:

| Format Purpose | Local API Link |
| :--- | :--- |
| **Send Document as PDF** *(Sales Invoice, Ledger, Statement)* | `http://localhost:54321/send?phoneNo=mobNum&messageText=message` |
| **Send Document as Image** *(Auto-cropped graphic)* | `http://localhost:54321/sendimage?phoneNo=mobNum&messageText=message` |

> [!TIP]
> **Secret Key Combination for Busy:**  
> Ensure **Service Provider Server** is selected (not *Busy Server*). If this radio option is hidden on your screen, press <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>V</kbd> to unhide it.

4. Save the configuration format.
5. You can now use the standard **SMS** send option from any Busy document/report. Ensure the **BusyCommander** server is running.

---

## 2. Sales Register Shortcuts & Workflows

Inside the **Sales Register**, select any voucher to trigger one-touch actions:

| Action | Shortcut Key | Description |
| :--- | :---: | :--- |
| **Send Bill** | <kbd>B</kbd> | Dispatches selected Sales Invoice directly to customer via WhatsApp. |
| **Send QR Code** | <kbd>Q</kbd> | Send your configured merchant QR code (with invoice amount embedded or standalone). |
| **Send Custom Message** | <kbd>M</kbd> | Opens quick message prompt to send a custom text to the party. |
| **Quick Party Jump** | <kbd>Shift</kbd> + *Initials* | Instant search and jump to the party's invoice in the list. |
| **Create Dispatch Slip** | <kbd>S</kbd> | Adds selected party + cartons to the active dispatch slip. |
| **Print Dispatch Slip** | <kbd>P</kbd> | Prints dispatch slip (A6 size) and automatically updates voucher's Long Narration. |
| **Clear Dispatch Slip** | <kbd>C</kbd> | Clears current dispatch list. |
| **Update Gadi No.** | <kbd>G</kbd> | Opens vehicle/transport entry dialog to record transport slip or vehicle number. |
| **Select Un-updated Bills** | <kbd>U</kbd> | Filters and highlights invoices that do not have vehicle/dispatch info recorded. |

---

## 3. Dispatch Slips & Gadi No. Tracking

BusyCommander simplifies multi-party transport packaging:

1. **Building a Dispatch Slip:**
   * Select a sales invoice and press <kbd>S</kbd> (*Sort/Sign*).
   * Enter the number of cartons for that party.
   * Repeat for other invoices being loaded onto the same transport.
   * *(Note: We recommend adding no more than 4–5 parties per slip, as it is formatted for A6 size printing).*
2. **Review & Print:**
   * The total carton count is displayed at the top of the Sales Register report.
   * Press <kbd>P</kbd> to print the compact A6 dispatch slip. This simultaneously updates each invoice's **Long Narration** with the dispatch details.
   * Press <kbd>C</kbd> anytime to clear the slip.
3. **Recording Vehicle / Gadi Details:**
   * Select an invoice and press <kbd>G</kbd> (*Gadi No.*).
   * Enter transport details or vehicle number, or choose from pre-saved options.
   * You can immediately send this dispatch update to the customer via WhatsApp.
   * Press <kbd>U</kbd> to quickly find invoices where Gadi info is still pending.

---

## 4. Bill-wise Statement (Hisab)

Generate customer outstanding slips, print hisab, and deliver payment reminders:

| Action | Shortcut Key | Description |
| :--- | :---: | :--- |
| **Show Total Amount** | <kbd>T</kbd> | Displays the exact total of all highlighted/selected entries. |
| **Send Hisab to Customer** | <kbd>H</kbd> | Sends beautiful statement / hisab summary via WhatsApp. |
| **Print Hisab** | <kbd>P</kbd> | Prints short or long hisab on A6 paper (automatically scales/shrinks for long lists). |
| **Send Bill** | <kbd>B</kbd> | Sends selected bill via WhatsApp. |
| **Send QR Code** | <kbd>Q</kbd> | Sends configured merchant UPI QR code. |
| **Send Custom Message** | <kbd>M</kbd> | Sends custom WhatsApp text to party. |

---

## 5. Account Ledger (<kbd>Ctrl</kbd> + <kbd>L</kbd>)

From the Account Ledger screen, you can also execute quick dispatch actions:

| Action | Shortcut Key | Description |
| :--- | :---: | :--- |
| **Send Bill** | <kbd>B</kbd> | Dispatches selected invoice via WhatsApp. |
| **Send QR Code** | <kbd>Q</kbd> | Sends merchant UPI QR code. |
| **Send Message** | <kbd>M</kbd> | Dispatches custom message to party. |

---

## 6. Account Master & Item Master Utilities

### Automatic WhatsApp Number & DP Verification:
* When entering a 10-digit mobile number in the **Account Master**, press <kbd>Enter</kbd>.
* BusyCommander queries WhatsApp status:
  * If WhatsApp is not registered on that number, an alert appears.
  * If WhatsApp is active and has a public profile picture (DP), the DP pops up immediately for instant visual confirmation.

### Hindi Transliteration (Kruti Dev 010 Conversion):
* While editing **Party Print Name** (in Account Master) or **Item Print Name** (in Item Master):
  1. Type the phonetic English word (e.g., `RAVI` or `SHARMA`) and press <kbd>Enter</kbd>.
  2. A transliteration window appears with Hindi options (e.g. `रवि`, `रवी`, `रावी`).
  3. Selecting an option automatically converts it into **Kruti Dev 010** encoding and inserts it into Print Name.
  4. Use Busy's *Data-Font* option in Advanced Document Printing to print professional Hindi bills and labels.
  5. If you do not select an option, your original text remains unchanged.
  6. **To skip transliteration:** Press <kbd>Tab</kbd> instead of <kbd>Enter</kbd> to move to the next field.

---

## 7. Automatic Receipt (Jama) Sharing

* Whenever you save a **Receipt Voucher**, BusyCommander can automatically open a WhatsApp dispatch prompt.
* Deliver instant payment confirmation / jama receipt messages to your customer's WhatsApp.
* *Note: This feature can be enabled or disabled at any time from the **User Console**.*

---

## 8. Troubleshooting & FAQ

| Problem | Root Cause | Solution |
| :--- | :--- | :--- |
| **BusyCommander Not Responding** | Windows Defender / Antivirus blocking background engine, or missing Busy COM permissions. | • Add an antivirus exclusion for the BusyCommander installation folder.<br>• Ensure Busy is running with a valid license.<br>• Restart the server from Busy's Add-on menu or re-open the company. |
| **WhatsApp Message Not Sent** | Incorrect number format or phone session disconnected. | • Check that the mobile number is 10 digits (without extra prefix).<br>• Verify that WhatsApp is active and connected in the User Console QR screen.<br>• Check that your phone has an active internet connection. |
| **Uninstalling BusyCommander** | Need to remove integration cleanly. | 1. When opening a company, choose **No** at the "Start Server" prompt.<br>2. Confirm the uninstallation prompt.<br>3. Go to Windows **Control Panel &rarr; Add/Remove Programs** and uninstall **BusyCommanderLite**. |

---

## 9. Support & Contact

If you have questions, feedback, or require assistance:
* 📧 **Email:** [busycommander@gmail.com](mailto:busycommander@gmail.com)
* 🐙 **GitHub Repository:** [busycommanderlite/BusyCommanderLite](https://github.com/busycommanderlite/BusyCommanderLite)

---
*(Developed with ❤️ by the BusyCommander Team)*

