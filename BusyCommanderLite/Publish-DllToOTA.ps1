<#
.SYNOPSIS
    Packages and prepares BusyCommanderLite.dll or server.dll for Firebase OTA distribution.

.DESCRIPTION
    1. Reads the compiled DLL from bin\Release.
    2. Compresses it with GZip (as expected by OtaUpdater).
    3. Converts it to Base64.
    4. Generates a JSON payload ready to import or publish to Firebase Realtime Database.
#>

param(
    [ValidateSet("bclite", "server")]
    [string]$Target = "bclite",
    
    [int]$NewVersion = 0,
    
    [string]$DllPath = ""
)

$ErrorActionPreference = "Stop"

$appDir = $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($DllPath)) {
    if ($Target -eq "bclite") {
        $DllPath = Join-Path $appDir "bin\Release\BusyCommanderLite.dll"
    } else {
        $DllPath = Join-Path $appDir "server.dll"
    }
}

if (-not (Test-Path $DllPath)) {
    Write-Error "DLL file not found at: $DllPath"
    exit 1
}

# 1. Determine current version from Firebase
$baseUrl = "https://busymasterdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/bc_scripts/"
$remoteVer = 0
try {
    $verUrl = "$baseUrl$($Target)_version.json"
    $verResp = (Invoke-RestMethod -Uri $verUrl).ToString().Trim().Trim('"')
    [int]::TryParse($verResp, [ref]$remoteVer) | Out-Null
    Write-Host "Current Remote Version in Firebase ($($Target)_version): $remoteVer" -ForegroundColor Cyan
} catch {
    Write-Warning "Could not fetch remote version: $($_.Exception.Message)"
}

if ($NewVersion -le 0) {
    $NewVersion = $remoteVer + 1
}

Write-Host "Target: $Target" -ForegroundColor Green
Write-Host "Publishing Version: v$NewVersion" -ForegroundColor Green
Write-Host "Source DLL: $DllPath" -ForegroundColor Green

# 2. Read DLL bytes
$dllBytes = [System.IO.File]::ReadAllBytes($DllPath)
$origSize = $dllBytes.Length
Write-Host "Original Size: $([math]::Round($origSize / 1024, 2)) KB ($origSize bytes)"

# 3. Compress with GZip
$ms = New-Object System.IO.MemoryStream
$gz = New-Object System.IO.Compression.GZipStream($ms, [System.IO.Compression.CompressionMode]::Compress)
$gz.Write($dllBytes, 0, $dllBytes.Length)
$gz.Close()
$compressedBytes = $ms.ToArray()
$ms.Close()
$compSize = $compressedBytes.Length
Write-Host "Compressed Size: $([math]::Round($compSize / 1024, 2)) KB ($compSize bytes)"

# 4. Base64 string
$base64Data = [System.Convert]::ToBase64String($compressedBytes)

# 5. Chunking (approx 2MB per chunk max for Firebase RTDB reliability)
$maxChunkLength = 1000000
$chunks = @()
if ($base64Data.Length -le $maxChunkLength) {
    $chunks += $base64Data
} else {
    $offset = 0
    while ($offset -lt $base64Data.Length) {
        $len = [Math]::Min($maxChunkLength, $base64Data.Length - $offset)
        $chunks += $base64Data.Substring($offset, $len)
        $offset += $len
    }
}

Write-Host "Chunk Count: $($chunks.Count)"

# 6. Build the payload
$payload = [ordered]@{
    "$($Target)_version" = $NewVersion
    "$($Target)_dll_chunk_count" = $chunks.Count
    "$($Target)_dll_chunks" = $chunks
}

$outputJsonPath = Join-Path $appDir "OTA_Payload_$($Target)_v$($NewVersion).json"
$payload | ConvertTo-Json -Depth 5 | Set-Content -Path $outputJsonPath -Encoding UTF8

Write-Host "`nSUCCESS! OTA Payload generated at:" -ForegroundColor Green
Write-Host "  $outputJsonPath" -ForegroundColor Yellow
Write-Host "`nHow to deploy to Firebase:" -ForegroundColor Cyan
Write-Host "1. Go to Firebase Console -> Realtime Database -> Data"
Write-Host "2. Navigate to: /bc_scripts"
Write-Host "3. Click the 3 dots (...) -> 'Import JSON' and select the file above."
Write-Host "   OR manually set:"
Write-Host "     - $($Target)_version = $NewVersion"
Write-Host "     - $($Target)_dll_chunk_count = $($chunks.Count)"
Write-Host "     - $($Target)_dll_chunks/0 = <base64 content in payload file>"
