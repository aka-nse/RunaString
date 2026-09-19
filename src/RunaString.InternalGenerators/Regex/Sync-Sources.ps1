#!/usr/bin/env pwsh

# Script constant values
$Owner = "dotnet"
$Repo = "runtime"
$Branch = "main"
$TargetPath = "src/libraries/System.Text.RegularExpressions/src/System"
$ResourceRoot = Join-Path -Path $PSScriptRoot -ChildPath "resources"

# Configure PowerShell strict mode and error handling
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ==============================================================================
# functions ====================================================================

function New-GitHubHeaders {
    $headers = @{
        "Accept"     = "application/vnd.github+json"
        "User-Agent" = "RunaString-Sync-Sources"
    }

    if ($env:GITHUB_TOKEN) {
        $headers["Authorization"] = "Bearer $($env:GITHUB_TOKEN)"
    }

    return $headers
}

function Get-GitHubContentsUri {
    param(
        [Parameter(Mandatory = $true)] [string] $Owner,
        [Parameter(Mandatory = $true)] [string] $Repo,
        [Parameter(Mandatory = $true)] [string] $Path,
        [Parameter(Mandatory = $true)] [string] $Ref)

    $escapedPath = [Uri]::EscapeDataString($Path).Replace("%2F", "/")
    return "https://api.github.com/repos/$($Owner)/$($Repo)/contents/$($escapedPath)?ref=$($Ref)"
}

function Get-GitHubFilesRecursive {
    param(
        [Parameter(Mandatory = $true)] [string] $Owner,
        [Parameter(Mandatory = $true)] [string] $Repo,
        [Parameter(Mandatory = $true)] [string] $Path,
        [Parameter(Mandatory = $true)] [string] $Ref,
        [Parameter(Mandatory = $true)] [hashtable] $Headers)

    $uri = Get-GitHubContentsUri -Owner $Owner -Repo $Repo -Path $Path -Ref $Ref
    $items = Invoke-RestMethod -Method Get -Uri $uri -Headers $Headers

    foreach ($item in $items) {
        if ($item.type -eq "dir") {
            Get-GitHubFilesRecursive -Owner $Owner -Repo $Repo -Path $item.path -Ref $Ref -Headers $Headers
            continue
        }

        if ($item.type -eq "file") {
            [PSCustomObject]@{
                Path        = $item.path
                Name        = $item.name
                Sha         = $item.sha
                DownloadUrl = $item.download_url
            }
        }
    }
}

function Get-LocalFilesWithSha {
    param(
        [Parameter(Mandatory = $true)] [string] $RootPath
    )

    if (-not (Test-Path -LiteralPath $RootPath -PathType Container)) {
        return @()
    }

    Get-ChildItem -LiteralPath $RootPath -File -Recurse | ForEach-Object {
        $relativePath = [System.IO.Path]::GetRelativePath($RootPath, $_.FullName).Replace('\\', '/')

        [PSCustomObject]@{
            Path = $relativePath
            Name = $_.Name
            Sha  = (git hash-object $_.FullName).ToLowerInvariant()
        }
    } | Sort-Object Path
}

function Get-NormalizedProjectRelativePath {
    param(
        [Parameter(Mandatory = $true)] [string] $Path,
        [Parameter(Mandatory = $true)] [string] $BasePath
    )

    $normalized = $Path.Trim('/').Trim('\\')
    $basePrefix = $BasePath.Trim('/').Trim('\\')

    if ($normalized.StartsWith($basePrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        $normalized = $normalized.Substring($basePrefix.Length).TrimStart('/').TrimStart('\\')
    }

    return $normalized.Replace('\\', '/')
}

function Save-GitHubFileToResource {
    param(
        [Parameter(Mandatory = $true)] [string] $RemotePath,
        [Parameter(Mandatory = $true)] [string] $ResourceRoot,
        [Parameter(Mandatory = $true)] [string] $DownloadUrl,
        [Parameter(Mandatory = $true)] [hashtable] $Headers
    )

    $targetPath = Join-Path -Path $ResourceRoot -ChildPath $RemotePath
    $targetDirectory = Split-Path -Path $targetPath -Parent

    if ($targetDirectory -and -not (Test-Path -LiteralPath $targetDirectory -PathType Container)) {
        New-Item -ItemType Directory -Path $targetDirectory -Force | Out-Null
    }

    $response = Invoke-WebRequest -Uri $DownloadUrl -Headers $Headers
    $contentBytes = [System.Text.Encoding]::UTF8.GetBytes($response.Content)
    [System.IO.File]::WriteAllBytes($targetPath, $contentBytes)
}

# ==============================================================================
# script execution =============================================================

$currentDir = Get-Location
try {
    # step 1: Fetches the list of files from the GitHub repository
    $headers = New-GitHubHeaders
    $remoteFiles = @(Get-GitHubFilesRecursive -Owner $Owner -Repo $Repo -Path $TargetPath -Ref $Branch -Headers $headers)

    if ($remoteFiles.Count -eq 0) {
        Write-Warning "No files found under '$TargetPath' on '$Owner/$Repo@$Branch'."
        exit 0
    }

    $remoteFiles = $remoteFiles | Sort-Object Path

    Write-Host "Fetched $($remoteFiles.Count) files from $($Owner)/$($Repo)@$($Branch):$($TargetPath)"
    $remoteFiles | Select-Object Path, Sha | Format-Table -AutoSize

    # step 2: Calculates hash values of the local files
    $localFiles = @(Get-LocalFilesWithSha -RootPath $ResourceRoot)

    if ($localFiles.Count -eq 0) {
        Write-Host "No local files found under '$ResourceRoot'."
    }
    else {
        Write-Host "Found $($localFiles.Count) local files under '$ResourceRoot'"
        $localFiles | Select-Object Path, Sha | Format-Table -AutoSize
    }

    # step 3: Compares the remote and local files to determine which files need to be updated
    $localLookup = @{}
    foreach ($localFile in $localFiles) {
        $localLookup[$localFile.Path.Replace('\', '/')] = $localFile
    }

    $filesToUpdate = @()
    foreach ($remoteFile in $remoteFiles) {
        $relativeRemotePath = Get-NormalizedProjectRelativePath -Path $remoteFile.Path -BasePath $TargetPath
        if ([string]::IsNullOrWhiteSpace($relativeRemotePath)) {
            continue
        }
        $relativeRemotePath = $relativeRemotePath.Replace('\', '/')

        $localFile = $localLookup[$relativeRemotePath]
        if ($null -eq $localFile -or $localFile.Sha -ne $remoteFile.Sha) {
            Write-Host "File $($relativeRemotePath) needs to be updated."
            $filesToUpdate += [PSCustomObject]@{
                RelativePath = $relativeRemotePath
                DownloadUrl  = $remoteFile.DownloadUrl
                Sha          = $remoteFile.Sha
            }
        }
        else {
            Write-Host "No update needed for $($relativeRemotePath)"
        }
    }

    if ($filesToUpdate.Count -eq 0) {
        Write-Host "No remote file differences found. The local resources are already in sync."
    }
    else {
        Write-Host "Found $($filesToUpdate.Count) file(s) to sync from $($Owner)/$($Repo)@$($Branch)."
        foreach ($fileToUpdate in $filesToUpdate) {
            Save-GitHubFileToResource -RemotePath $fileToUpdate.RelativePath -ResourceRoot $ResourceRoot -DownloadUrl $fileToUpdate.DownloadUrl -Headers $headers
            Write-Host "Updated $($fileToUpdate.RelativePath)"
        }
    }
}
finally {
    Set-Location -Path $currentDir
}
