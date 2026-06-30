param(
    [switch]$SkipPatch
)

$ErrorActionPreference = "Stop"

git submodule update --init --recursive external/CPyMO external/YukimiScript

if (-not $SkipPatch) {
    $patchRoots = @("patches/cpymo", "patches/yukimiscript")
    foreach ($patchRoot in $patchRoots) {
        if (-not (Test-Path $patchRoot)) {
            continue
        }

        Get-ChildItem -Path $patchRoot -Filter "*.patch" | Sort-Object Name | ForEach-Object {
            git apply --check $_.FullName
            git apply $_.FullName
        }
    }
}

dotnet restore CpymoEditor.slnx
