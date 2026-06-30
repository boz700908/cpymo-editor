$ErrorActionPreference = "Stop"

New-Item -ItemType Directory -Force patches/cpymo, patches/yukimiscript | Out-Null

Write-Host "Patch refresh is intentionally manual."
Write-Host "Create patch files from changes inside external/CPyMO or external/YukimiScript and place them under patches/."
