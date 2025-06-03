# PowerShell script to set up user secrets for local development
# Run this from the src/api/server directory

Write-Host "Setting up user secrets for FSH Starter Kit..." -ForegroundColor Green

# Navigate to server project directory
$serverPath = Join-Path $PSScriptRoot ".." "api" "server"
Set-Location $serverPath

# Azure AD B2C secrets
dotnet user-secrets set "AuthenticationOptions:AzureAdB2C:ClientSecret" "YOUR_B2C_CLIENT_SECRET"

# Database connection string
dotnet user-secrets set "DatabaseOptions:ConnectionString" "Server=localhost;Database=fshdb;User Id=postgres;Password=YOUR_DB_PASSWORD"

# JWT secret key (generate a secure key)
dotnet user-secrets set "JwtOptions:Key" "YOUR_JWT_SECRET_KEY_BASE64"

# Hangfire password
dotnet user-secrets set "HangfireOptions:Password" "YOUR_HANGFIRE_PASSWORD"

# Mail service password
dotnet user-secrets set "MailOptions:Password" "YOUR_MAIL_PASSWORD"

# Graph API Settings
dotnet user-secrets set "GraphApi:TenantId" "YOUR_B2C_TENANT_ID"
dotnet user-secrets set "GraphApi:ClientId" "YOUR_GRAPH_API_CLIENT_ID"
dotnet user-secrets set "GraphApi:ClientSecret" "YOUR_GRAPH_API_CLIENT_SECRET"
dotnet user-secrets set "GraphApi:B2CExtensionAppClientId" "YOUR_EXTENSION_APP_ID_WITHOUT_HYPHENS"
dotnet user-secrets set "GraphApi:B2CDomain" "yourb2ctenant.onmicrosoft.com"

Write-Host "User secrets have been configured!" -ForegroundColor Green
Write-Host ""
Write-Host "To view your secrets, run:" -ForegroundColor Yellow
Write-Host "  dotnet user-secrets list" -ForegroundColor White
Write-Host ""
Write-Host "To remove a secret, run:" -ForegroundColor Yellow
Write-Host "  dotnet user-secrets remove ""SecretName""" -ForegroundColor White
Write-Host ""
Write-Host "Remember to update the GraphApi:ClientSecret with your actual value!" -ForegroundColor Red