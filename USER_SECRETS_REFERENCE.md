# User Secrets Reference

This document lists all the user secrets required for the FSH Starter Kit application to run properly with Azure AD B2C and Microsoft Graph API integration.

## Required User Secrets

Run these commands from the `/src/api/server` directory:

### 1. Database Configuration
```bash
dotnet user-secrets set "DatabaseOptions:ConnectionString" "Server=localhost;Database=fshdb;User Id=postgres;Password=YOUR_PASSWORD"
```

### 2. JWT Configuration
```bash
dotnet user-secrets set "JwtOptions:Key" "YOUR_BASE64_ENCODED_KEY"
```

### 3. Hangfire Configuration
```bash
dotnet user-secrets set "HangfireOptions:Password" "YOUR_HANGFIRE_PASSWORD"
```

### 4. Mail Configuration
```bash
dotnet user-secrets set "MailOptions:Password" "YOUR_SMTP_PASSWORD"
```

### 5. Azure AD B2C Configuration
```bash
dotnet user-secrets set "AuthenticationOptions:AzureAdB2C:ClientSecret" "YOUR_B2C_CLIENT_SECRET"
```

### 6. Microsoft Graph API Configuration
```bash
dotnet user-secrets set "GraphApi:TenantId" "YOUR_B2C_TENANT_ID"
dotnet user-secrets set "GraphApi:ClientId" "YOUR_GRAPH_APP_CLIENT_ID"
dotnet user-secrets set "GraphApi:ClientSecret" "YOUR_GRAPH_APP_CLIENT_SECRET"
dotnet user-secrets set "GraphApi:B2CExtensionAppClientId" "YOUR_B2C_EXTENSION_APP_CLIENT_ID"
dotnet user-secrets set "GraphApi:B2CDomain" "YOUR_B2C_DOMAIN.onmicrosoft.com"
```

## Example Values (for reference - DO NOT commit actual secrets to source control)

Replace the placeholders below with your actual values:

```bash
# Database
dotnet user-secrets set "DatabaseOptions:ConnectionString" "Server=localhost;Database=fshdb;User Id=postgres;Password=YOUR_DB_PASSWORD"

# JWT
dotnet user-secrets set "JwtOptions:Key" "YOUR_BASE64_ENCODED_JWT_KEY_MIN_32_CHARS"

# Hangfire
dotnet user-secrets set "HangfireOptions:Password" "YOUR_HANGFIRE_DASHBOARD_PASSWORD"

# Mail
dotnet user-secrets set "MailOptions:Password" "YOUR_SMTP_PASSWORD"

# Azure AD B2C
dotnet user-secrets set "AuthenticationOptions:AzureAdB2C:ClientSecret" "YOUR_B2C_APP_CLIENT_SECRET"

# Microsoft Graph API
dotnet user-secrets set "GraphApi:TenantId" "YOUR-B2C-TENANT-ID-GUID"
dotnet user-secrets set "GraphApi:ClientId" "YOUR-GRAPH-APP-CLIENT-ID-GUID"
dotnet user-secrets set "GraphApi:ClientSecret" "YOUR_GRAPH_APP_CLIENT_SECRET"  
dotnet user-secrets set "GraphApi:B2CExtensionAppClientId" "YOUR-B2C-EXTENSION-APP-CLIENT-ID"
dotnet user-secrets set "GraphApi:B2CDomain" "yourb2cdomain.onmicrosoft.com"
```

## Verify Configuration

To verify all secrets are properly set, run:
```bash
dotnet user-secrets list
```

You should see all 10 secrets listed.

## Important Notes

1. **Graph API TenantId** should match your Azure AD B2C TenantId
2. **B2CExtensionAppClientId** is the client ID of the b2c-extensions-app (without hyphens)
3. **GraphApi:ClientId** is the client ID of your app registration that has Graph API permissions
4. User secrets are only loaded in Development environment by default
5. In production, use Azure Key Vault or environment variables instead of user secrets

Note that this file has been sanitized of all actual secrets.