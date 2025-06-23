# database update migrations

$context = "ApplicationDbContext"
$project = "ServicePortals.Infrastructure"
$startupProject = "ServicePortal"

Write-Host "-------> Updating database for context: $context" -ForegroundColor Green

dotnet ef database update `
    --context $context `
    --project $project `
    --startup-project $startupProject

Write-Host "-------> Update database complete." -ForegroundColor Green