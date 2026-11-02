# Build and install the development mod for RimWorld 1.6 on Windows.
# Run from repository root: powershell -ExecutionPolicy Bypass -File scripts/build_install_windows.ps1 -GameDir "D:\SteamLibrary\steamapps\common\RimWorld"
param(
    [Parameter(Mandatory = $true)]
    [string]$GameDir,
    [switch]$SkipSmokeTests
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$managed = Join-Path $GameDir 'RimWorldWin64_Data\Managed'
$assembly = Join-Path $managed 'Assembly-CSharp.dll'
if (-not (Test-Path -LiteralPath $assembly -PathType Leaf)) {
    throw "RimWorld 1.6 Assembly-CSharp.dll not found: $assembly"
}
foreach ($command in @('dotnet', 'python')) {
    if (-not (Get-Command $command -ErrorAction SilentlyContinue)) {
        throw "Missing executable: $command"
    }
}

Push-Location $root
try {
    if (-not $SkipSmokeTests) {
        & dotnet run --project 'tests/BridgeSmoke/BridgeSmoke.csproj' -c Release
        if ($LASTEXITCODE -ne 0) { throw 'BridgeSmoke failed; installation aborted.' }
    }
    & dotnet build 'Source/Aquacultura.RimWorld/Aquacultura.RimWorld.csproj' -c Release '-p:RimWorldVersion=1.6' "-p:GameManagedDir=$managed"
    if ($LASTEXITCODE -ne 0) { throw 'RimWorld assembly build failed; installation aborted.' }

    $modSource = Join-Path $root 'Aquacultura'
    $dll = Join-Path $modSource '1.6\Assemblies\Aquacultura.RimWorld.dll'
    if (-not (Test-Path -LiteralPath $dll -PathType Leaf) -or (Get-Item -LiteralPath $dll).Length -eq 0) {
        throw "Built DLL is missing or empty: $dll"
    }
    & python 'scripts/activate_runtime.py' --version 1.6
    if ($LASTEXITCODE -ne 0) { throw 'Runtime activation failed; installation aborted.' }

    $mods = Join-Path $GameDir 'Mods'
    if (-not (Test-Path -LiteralPath $mods -PathType Container)) {
        throw "RimWorld Mods directory missing: $mods"
    }
    $destination = Join-Path $mods 'Aquacultura'
    if (Test-Path -LiteralPath $destination) {
        $backup = Join-Path $mods ('Aquacultura-backup-' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
        Move-Item -LiteralPath $destination -Destination $backup
        Write-Host "Previous installation backed up: $backup"
    }
    New-Item -ItemType Directory -Path $destination -Force | Out-Null
    Copy-Item -Path (Join-Path $modSource '*') -Destination $destination -Recurse -Force
    Write-Host "Aquacultura development build installed: $destination"
    Write-Host 'Not a playable release. Enable mod and check game log on a test save.'
}
finally {
    Pop-Location
}
