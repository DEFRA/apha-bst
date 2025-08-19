param (
  [string]$CommitMsg
)

$versionFile = "VERSION"
if (-Not (Test-Path $versionFile)) {
  "0.0.0" | Out-File -Encoding ASCII $versionFile
}

$version = Get-Content $versionFile -Raw
$parts = $version -split '\.'

$major = [int]$parts[0]
$minor = [int]$parts[1]
$patch = [int]$parts[2]

$bumpedVersion = "$major.$minor.$patch"

# Set output
"version=v$bumpedVersion" >> $env:GITHUB_OUTPUT
