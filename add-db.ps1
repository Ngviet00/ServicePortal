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

$outputDir = Read-Host "Nhập thư mục migrations (default: Data/Migrations)"
if ([string]::IsNullOrWhiteSpace($outputDir)) {
    $outputDir = "Data/Migrations"
}

$migrationName = Read-Host "Nhập tên migration (bắt buộc)"
if ([string]::IsNullOrWhiteSpace($migrationName)) {
    Write-Host "Bạn phải nhập tên migration." -ForegroundColor Red
    exit 1 # Thoát với mã lỗi 1
}

Write-Host "Đang thêm migration '$migrationName' vào context '$context'..." -ForegroundColor Green

& dotnet ef migrations add $migrationName `
    --context $context `
    --project $project `
    --startup-project $startupProject `
    --output-dir $outputDir

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration tạo thành công." -ForegroundColor Green
} else {
    Write-Host "Lỗi: Không thể tạo migration '$migrationName'." -ForegroundColor Red
    exit $LASTEXITCODE
}