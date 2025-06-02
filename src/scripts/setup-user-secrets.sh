#!/bin/bash
# Bash script to set up user secrets for local development
# Run this from the src/api/server directory

echo -e "\033[32mSetting up user secrets for FSH Starter Kit...\033[0m"

# Navigate to server project directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SERVER_PATH="$SCRIPT_DIR/../api/server"
cd "$SERVER_PATH"

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

# Graph API Client Secret
dotnet user-secrets set "GraphApi:ClientSecret" "YOUR_GRAPH_API_CLIENT_SECRET"

echo -e "\033[32mUser secrets have been configured!\033[0m"
echo ""
echo -e "\033[33mTo view your secrets, run:\033[0m"
echo -e "\033[37m  dotnet user-secrets list\033[0m"
echo ""
echo -e "\033[33mTo remove a secret, run:\033[0m"
echo -e "\033[37m  dotnet user-secrets remove \"SecretName\"\033[0m"
echo ""
echo -e "\033[31mRemember to update the GraphApi:ClientSecret with your actual value!\033[0m"