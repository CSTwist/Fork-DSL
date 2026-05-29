# PowerShell Script to run Unity Tests in Batch Mode
# Usage: .\tools\run_tests.ps1 [-UnityPath "path/to/unity.exe"] [-TestPlatform "EditMode"]

param (
    [string]$UnityPath = "C:\Program Files\Unity\Hub\Editor\2022.3.20f1\Editor\Unity.exe",
    [string]$TestPlatform = "EditMode",
    [string]$ProjectPath = "$PSScriptRoot\..\unity-project"
)

Write-Host "Starting Unity Test Runner for $TestPlatform..."
Write-Host "Project path: $ProjectPath"
Write-Host "Unity path: $UnityPath"

if (-not (Test-Path $UnityPath)) {
    Write-Warning "Unity editor not found at $UnityPath."
    Write-Host "Please specify your Unity path, e.g.:"
    Write-Host ".\tools\run_tests.ps1 -UnityPath 'C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe'"
    exit 1
}

$resultsPath = "$PSScriptRoot\..\TestResults_$TestPlatform.xml"

# Execute Unity in batch mode to run NUnit tests
$arguments = "-batchmode -runTests -projectPath `"$ProjectPath`" -testResults `"$resultsPath`" -testPlatform $TestPlatform -noGraphics"

Write-Host "Running command: & '$UnityPath' $arguments"

Start-Process -FilePath $UnityPath -ArgumentList $arguments -Wait -NoNewWindow

if (Test-Path $resultsPath) {
    Write-Host "Tests completed. Results written to: $resultsPath"
    # Print a summary from the XML if possible
    [xml]$xml = Get-Content $resultsPath
    $total = $xml.'test-run'.total
    $passed = $xml.'test-run'.passed
    $failed = $xml.'test-run'.failed
    Write-Host "Total tests: $total, Passed: $passed, Failed: $failed" -ForegroundColor Green
    if ($failed -gt 0) {
        Write-Error "Some tests failed!"
        exit 1
    }
} else {
    Write-Error "Unity test runner failed to produce results. Check editor log for errors."
    exit 1
}
