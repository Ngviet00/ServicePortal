# =====================================================================
# KIỂM TRA VÀ NÂNG QUYỀN ADMINISTRATOR
# =====================================================================
# This block ensures the script runs with Administrator privileges.
# If not, it relaunches itself in an elevated process.
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Warning "⚠️ Script is not running with Administrator privileges. Relaunching with elevated permissions..."
    # Start a new PowerShell process with administrative rights and pass the current script file.
    Start-Process PowerShell -Verb RunAs -ArgumentList "-File `"$($MyInvocation.MyCommand.Path)`""
    exit # Exit the current non-elevated process.
}

Write-Host "✅ Script is running with Administrator privileges."

# =====================================================================
# CONFIGURATION - Cập nhật các giá trị này cho môi trường của bạn
# =====================================================================

# Cấu hình trên máy tính cục bộ (nơi bạn chạy script này và mã nguồn)
$solutionPath = "E:\Projects\ServicePortal"
$projectPath = "$solutionPath\ServicePortal"
$publishTempPath = "$solutionPath\publish_temp" # Thư mục tạm để publish cục bộ
$buildConfiguration = "Release" # Cấu hình build (.NET: Release, Debug, v.v.)

# Cấu hình trên máy chủ từ xa (máy chủ IIS đích)
$remoteServerName = "10.0.0.149" # Địa chỉ IP hoặc tên máy chủ từ xa
$remotePublishTargetPath = "E:\Projects\ServicePortal\ServicePortal\bin\Release\net8.0\publish" # Đường dẫn trên máy chủ từ xa (NƠI ỨNG DỤNG SẼ ĐƯỢC ĐẶT)
$remoteIISWebsiteName = "2" # Tên Website IIS trên máy chủ từ xa
$remoteIISApplicationPoolName = "2" # Tên Application Pool liên quan trên máy chủ từ xa

# =====================================================================
# BẮT ĐẦU QUY TRÌNH TRIỂN KHAI
# =====================================================================
Write-Host "`n🚀 Starting deployment of application '$remoteIISWebsiteName' to remote server: '$remoteServerName'..."
Write-Host "========================================================================================"

# --- 1. Publish ứng dụng vào thư mục tạm thời trên máy cục bộ ---
Write-Host "`n----------------------------------------------------------------------------------------"
Write-Host "➡️ Step 1: Publishing application to temporary folder on local machine: '$publishTempPath'..."
try {
    # Remove existing temporary publish folder to ensure a clean build.
    if (Test-Path $publishTempPath) {
        Remove-Item -Path $publishTempPath -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "    ✅ Removed old temporary folder '$publishTempPath'."
    }
    
    # Run the .NET publish command.
    dotnet publish "$projectPath" -c $buildConfiguration -o "$publishTempPath"
    # Check the exit code of dotnet publish.
    if ($LASTEXITCODE -ne 0) {
        throw "The 'dotnet publish' command failed."
    }
    Write-Host "✅ Successfully published application locally."
} catch {
    Write-Error "🔴 Error during local application publish: $_. Please check your project path and build configuration."
    exit 1 # Exit script if publish fails, as subsequent steps depend on it.
}

# Các bước thực thi trên máy chủ từ xa (qua PowerShell Remoting)
Write-Host "`n----------------------------------------------------------------------------------------"
Write-Host "🔗 Establishing a Remote PowerShell Session with '$remoteServerName'..."

$session = $null # Initialize session variable for proper cleanup.

try {
    # Create a new PSSession. You'll be prompted for credentials for the remote server.
    # Enter the username and password of an account with Administrator rights on '$remoteServerName'.
    $session = New-PSSession -ComputerName $remoteServerName -Credential (Get-Credential) -ErrorAction Stop
    Write-Host "✅ Successfully connected to '$remoteServerName'."

    # Use Invoke-Command to execute a script block on the remote server.
    Invoke-Command -Session $session -ScriptBlock {
        param($targetPath, $websiteName, $appPoolName)

        # Ensure the WebAdministration module is loaded on the remote server.
        try {
            if (-not (Get-Module -ListAvailable -Name WebAdministration)) {
                Write-Error "🔴 Error: WebAdministration module is not installed on the remote server. Please install IIS Management Tools."
                throw "WebAdministration Module Missing" # Throw to break out of Invoke-Command.
            }
            Import-Module WebAdministration -ErrorAction Stop
            Write-Host "✅ Loaded WebAdministration module on the remote server."
        } catch {
            Write-Error "🔴 Error loading WebAdministration module on remote server: $_"
            throw $_ # Re-throw the error to the main script.
        }

        # --- 2. Dừng Application Pool và Website IIS trên máy chủ từ xa ---
        Write-Host "`n----------------------------------------------------------"
        Write-Host "➡️ Step 2: Stopping Application Pool and IIS Website on the remote server..."
        try {
            # Stop the Application Pool first to release file locks.
            if (Get-WebAppPoolState -Name $appPoolName -ErrorAction SilentlyContinue) {
                Stop-WebAppPool -Name $appPoolName -ErrorAction Stop
                Write-Host "    ✅ Stopped Application Pool '$appPoolName'."
            } else {
                Write-Warning "    ⚠️ Application Pool '$appPoolName' not found on remote server. Skipping App Pool stop."
            }

            # Stop the Website.
            if (Get-Website -Name $websiteName -ErrorAction SilentlyContinue) {
                Stop-Website -Name $websiteName -ErrorAction Stop
                Write-Host "    ✅ Stopped IIS Website '$websiteName'."
            } else {
                Write-Warning "    ⚠️ IIS Website '$websiteName' not found on remote server. Skipping Website stop."
            }

            Start-Sleep -Seconds 3 # Give a moment for processes to gracefully shut down.
            Write-Host "✅ Waited 3 seconds to ensure IIS resources are released on the remote server."
        } catch {
            Write-Error "🔴 Error stopping IIS on remote server: $_"
            throw $_ # Re-throw the error to the main script.
        }

        # --- 3. Xóa các file cũ trong thư mục đích trên máy chủ từ xa ---
        # This step ensures no old or locked files interfere with the copy process.
        Write-Host "`n----------------------------------------------------------"
        Write-Host "➡️ Step 3: Deleting old files in the target directory on the remote server: '$targetPath'..."
        try {
            if (Test-Path $targetPath) {
                # Delete only files within the target directory, preserving sub-folders.
                Get-ChildItem -Path $targetPath -Recurse | Where-Object { -not $_.PSIsContainer } | Remove-Item -Force -ErrorAction SilentlyContinue
                Write-Host "✅ Deleted old files in the remote target directory."
            } else {
                Write-Warning "⚠️ Target directory '$targetPath' does not exist on the remote server. Creating it now."
                New-Item -ItemType Directory -Path $targetPath -ErrorAction Stop | Out-Null
            }
        } catch {
            Write-Error "🔴 Error deleting old files on remote server: $_"
            throw $_ # Re-throw the error to the main script.
        }

    } -ArgumentList $remotePublishTargetPath, $remoteIISWebsiteName, $remoteIISApplicationPoolName

    # --- 4. Copy file từ máy cục bộ lên máy chủ từ xa ---
    Write-Host "`n----------------------------------------------------------"
    Write-Host "➡️ Step 4: Copying published files from local machine to remote server..."
    try {
        # Use Copy-Item with -ToSession to transfer files from local $publishTempPath to remote $remotePublishTargetPath.
        Copy-Item -Path "$publishTempPath\*" -Destination $remotePublishTargetPath -ToSession $session -Recurse -Force -ErrorAction Stop
        Write-Host "✅ Successfully copied files to the remote server."
    } catch {
        Write-Error "🔴 Error copying files to remote server: $_. Please check write permissions on the remote target directory."
        # Don't exit here, as we still want to attempt to restart IIS in finally block.
    }

} catch {
    Write-Error "🔴 Critical error during remote connection or command execution: $_"
    # This error could be connection failure or an error thrown from inside Invoke-Command.
    # We proceed to finally block to attempt IIS restart if session was established.
} finally {
    # --- 5. Khởi động lại Application Pool và Website IIS trên máy chủ từ xa ---
    Write-Host "`n----------------------------------------------------------------------------------------"
    Write-Host "➡️ Step 5: Starting Application Pool and IIS Website on the remote server..."
    try {
        # Only attempt to restart if a remote session was successfully established.
        if ($session) { 
            Invoke-Command -Session $session -ScriptBlock {
                param($websiteName, $appPoolName)
                
                # Start the Application Pool first.
                if (Get-WebAppPoolState -Name $appPoolName -ErrorAction SilentlyContinue) {
                    Start-WebAppPool -Name $appPoolName -ErrorAction Stop
                    Write-Host "✅ Started Application Pool '$appPoolName'."
                } else {
                    Write-Warning "⚠️ Application Pool '$appPoolName' not found on remote server. Skipping App Pool start."
                }

                # Start the Website.
                if (Get-Website -Name $websiteName -ErrorAction SilentlyContinue) {
                    Start-Website -Name $websiteName -ErrorAction Stop
                    Write-Host "✅ Started IIS Website '$websiteName'."
                } else {
                    Write-Warning "⚠️ Website IIS '$websiteName' not found on remote server. Skipping Website start."
                }
            } -ArgumentList $remoteIISWebsiteName, $remoteIISApplicationPoolName
        } else {
            Write-Warning "⚠️ No remote session was established. Cannot start IIS on the remote server."
        }
    } catch {
        Write-Error "🔴 Error starting IIS on remote server: $_. Please check website/App Pool names or permissions on the remote server."
    } finally {
        # Ensure the remote session is closed to release resources.
        if ($session) {
            Remove-PSSession -Session $session -ErrorAction SilentlyContinue
            Write-Host "✅ Closed remote PowerShell session."
        }
    }
}

Write-Host "`n🎉 Deployment of application '$remoteIISWebsiteName' to '$remoteServerName' completed!"
Write-Host "========================================================================================"