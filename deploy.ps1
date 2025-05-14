# Đường dẫn dự án
$projectPath = "E:\Projects\ServicePortal"

# Đường dẫn publish (VS 2022 và IIS cùng trỏ vào)
$publishPath = "$projectPath\bin\Release\net8.0\publish"

# Tên App Pool
$appPoolName = "ServicePortal"

# Đường dẫn file app_offline
$appOfflineFile = "$publishPath\app_offline.htm"

# Tạo file app_offline.htm
Write-Host "📴 Putting app offline..."
Set-Content -Path $appOfflineFile -Value "<html><body><h2>Deploying... Please wait.</h2></body></html>"

# Chờ vài giây để IIS unload ứng dụng
Start-Sleep -Seconds 3

# Build lại project
Write-Host "🛠️ Publishing project..."
cd $projectPath
dotnet publish -c Release -o $publishPath

# (Optional) Clean các file cũ nếu cần
# Write-Host "🧹 Cleaning old files..."
# Get-ChildItem -Path $publishPath -Exclude "app_offline.htm" | Remove-Item -Force -Recurse

# (Nếu có dùng thư mục tạm thì copy từ tạm vào $publishPath tại đây)

# Chờ cho chắc chắn sau khi copy xong
Start-Sleep -Seconds 2

# Xóa app_offline để kích hoạt lại ứng dụng
Write-Host "✅ Bringing app back online..."
Remove-Item -Path $appOfflineFile -Force

Write-Host "🚀 Deployment completed successfully with minimal downtime!"
