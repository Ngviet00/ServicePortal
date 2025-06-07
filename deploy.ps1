$ProjectFile = "ServicePoral.csproj" 

$PublishPath = "./bin/Release/net8.0/publish_temp"

$ServiceName = "service-portal-api-app" 

# --- Start Script ---

Write-Host "Start publish and docker..." -ForegroundColor Green

# dotnet publish
Write-Host "Publish .NET..." -ForegroundColor Yellow
try {
    dotnet publish -c Release -o $PublishPath

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Publish error." -ForegroundColor Red
        exit 1
    }
    Write-Host "Publish success." -ForegroundColor Green
} catch {
    Write-Host "Error when running publish: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# docker build
Write-Host "Start Docker Compose..." -ForegroundColor Yellow
try {
    docker compose up -d --build --force-recreate

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Docker compose up errror." -ForegroundColor Red
        exit 1
    }
    Write-Host "Docker compose up success." -ForegroundColor Green
} catch {
    Write-Host "Error when run docker compose up: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "Deploy success" -ForegroundColor Green

Write-Host "View status container: docker-compose ps" -ForegroundColor Cyan
Write-Host "View logs: docker-compose logs $ServiceName" -ForegroundColor Cyan