[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$context = Read-Host "Nhập tên DbContext (default: ApplicationDbContext)"
if ([string]::IsNullOrWhiteSpace($context)) {
    $context = "ApplicationDbContext"
}

$project = Read-Host "Nhập tên project chứa DbContext (default: ServicePortals.Infrastructure)"
if ([string]::IsNullOrWhiteSpace($project)) {
    $project = "ServicePortals.Infrastructure"
}

$startupProject = Read-Host "Nhập tên startup project (default: ServicePortal)"
if ([string]::IsNullOrWhiteSpace($startupProject)) {
    $startupProject = "ServicePortal"
}

Write-Host "Đang xóa migration mới nhất khỏi context '$context'..." -ForegroundColor Yellow

& dotnet ef migrations remove `
    --context $context `
    --project $project `
    --startup-project $startupProject

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration đã được xóa thành công." -ForegroundColor Green
} else {
    Write-Host "Lỗi: Không thể xóa migration." -ForegroundColor Red
    exit $LASTEXITCODE
}
