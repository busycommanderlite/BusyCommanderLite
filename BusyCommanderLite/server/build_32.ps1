# Build script for 32-bit (x86) server.dll for BusyWin
$ErrorActionPreference = "Stop"

$mingw32Bin = "C:\Users\Jeetu\mingw32\mingw32\bin"
if (-not (Test-Path (Join-Path $mingw32Bin "gcc.exe"))) {
    Write-Error "32-bit MinGW GCC not found at: $mingw32Bin"
    exit 1
}

$env:Path = "$mingw32Bin;" + $env:Path
$env:CC = Join-Path $mingw32Bin "gcc.exe"
$env:CGO_ENABLED = "1"
$env:GOARCH = "386"

$serverDir = $PSScriptRoot
Set-Location $serverDir

Write-Host "Building 32-bit server.dll with whatsmeow and SQLite..." -ForegroundColor Cyan
go build -buildmode=c-shared -ldflags="-s -w -extldflags '-static'" -o server.dll .

if ($LASTEXITCODE -eq 0 -and (Test-Path (Join-Path $serverDir "server.dll"))) {
    Write-Host "Build SUCCESS! server.dll (32-bit x86) generated." -ForegroundColor Green
    
    # Optionally deploy to C:\BusyWin\Whatsapp
    $busyWinWhatsapp = "C:\BusyWin\Whatsapp\server.dll"
    if (Test-Path (Split-Path $busyWinWhatsapp)) {
        try {
            Copy-Item (Join-Path $serverDir "server.dll") $busyWinWhatsapp -Force
            Write-Host "Auto-deployed to $busyWinWhatsapp" -ForegroundColor Green
        } catch {
            Write-Warning "Could not auto-deploy (file may be in use by Busy): $($_.Exception.Message)"
        }
    }
} else {
    Write-Error "Build failed!"
}

