$ErrorActionPreference = "Stop"

$toolDir = Resolve-Path "external/CPyMO/cpymo-tool"
$outputDir = Resolve-Path "."
$buildDir = Join-Path $outputDir "artifacts/cpymo-tool/win"
New-Item -ItemType Directory -Force $buildDir | Out-Null

Push-Location $toolDir
try {
    $out = Join-Path $buildDir "cpymo-tool.exe"
    gcc *.c -I../cpymo -I../stb -I../endianness.h -DNDEBUG -O2 -o $out
    if ($LASTEXITCODE -ne 0) {
        throw "gcc failed with exit code $LASTEXITCODE"
    }

    & $out
}
finally {
    Pop-Location
}
