param(
    [switch]$SkipPatch
)

$ErrorActionPreference = "Stop"

git submodule update --init --recursive external/CPyMO external/YukimiScript

if (-not $SkipPatch) {
    $patchSets = @(
        @{ Root = "patches/cpymo"; Repository = "external/CPyMO" },
        @{ Root = "patches/yukimiscript"; Repository = "external/YukimiScript" }
    )

    foreach ($patchSet in $patchSets) {
        if (-not (Test-Path $patchSet.Root)) {
            continue
        }

        Get-ChildItem -Path $patchSet.Root -Filter "*.patch" | Sort-Object Name | ForEach-Object {
            $patchPath = Resolve-Path $_.FullName
            git -C $patchSet.Repository apply --check $patchPath
            git -C $patchSet.Repository apply $patchPath
        }
    }
}

dotnet restore CpymoEditor.slnx
