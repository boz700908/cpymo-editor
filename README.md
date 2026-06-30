# CPyMO Editor

Accessible graphical editor for CPyMO/PyMO projects on Windows and Android.

## Current Status

This repository currently contains the first implementation slice:

- .NET 9 solution and shared core library.
- Windows/Android MAUI shell with native tab-based UI.
- Project classification for YKM source, native PyMO, and rejected YKM-compiled products.
- Shared compiler and tool service contracts.
- CPyMO and YukimiScript as git submodules.
- MIT license.

## Setup

```powershell
.\tools\bootstrap.ps1 -SkipPatch
```

The bootstrap script initializes submodules and restores .NET packages.

## Test

```powershell
dotnet test .\src\CpymoEditor.Tests\CpymoEditor.Tests.csproj
```

## Build

Windows:

```powershell
dotnet build .\src\CpymoEditor\CpymoEditor.csproj -f net9.0-windows10.0.19041.0
```

CPyMO tool for Windows:

```powershell
.\tools\bootstrap.ps1
.\tools\build-cpymo-tool-win.ps1
```

The generated executable is written to `artifacts/cpymo-tool/win/cpymo-tool.exe`.

Android:

```powershell
$sdk = Join-Path $env:LOCALAPPDATA 'Android\Sdk'
dotnet build .\src\CpymoEditor\CpymoEditor.csproj -f net9.0-android -p:AndroidSdkDirectory="$sdk" -p:JavaSdkDirectory="C:\Program Files\Java\jdk-23"
```

The Android build currently succeeds with JDK 23 on this machine, but .NET Android emits a version-string warning. A JDK 17 installation is expected to reduce that warning.
