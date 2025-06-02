# Azure Key Vault Setup Guide

This guide explains how to configure Azure Key Vault for managing secrets in the FSH Starter Kit.

## Overview

The application now supports Azure Key Vault for secure secret management. In development, it uses .NET User Secrets, and in production, it can use Azure Key Vault.

## Local Development Setup

### 1. Set Up User Secrets

Run the provided script to configure user secrets for local development:

**Windows (PowerShell):**
```powershell
cd src/api/server
../../../scripts/setup-user-secrets.ps1
```

**Linux/Mac (Bash):**
```bash
cd src/api/server
../../../scripts/setup-user-secrets.sh
```

### 2. Update Graph API Secret

After running the script, update the Graph API client secret:

```bash
dotnet user-secrets set "GraphApi:ClientSecret" "YOUR_ACTUAL_GRAPH_API_SECRET"
```

### 3. Verify Secrets

List all configured secrets:
```bash
dotnet user-secrets list
```

## Production Setup with Azure Key Vault

### 1. Create Azure Key Vault

```bash
# Create resource group
az group create --name rg-fsh-keyvault --location eastus

# Create Key Vault
az keyvault create --name kv-fsh-prod --resource-group rg-fsh-keyvault --location eastus
```

### 2. Add Secrets to Key Vault

```bash
# B2C Client Secret
az keyvault secret set --vault-name kv-fsh-prod --name "AuthenticationOptions--AzureAdB2C--ClientSecret" --value "YOUR_B2C_CLIENT_SECRET"

# Database Connection String
az keyvault secret set --vault-name kv-fsh-prod --name "DatabaseOptions--ConnectionString" --value "YOUR_PRODUCTION_CONNECTION_STRING"

# JWT Key
az keyvault secret set --vault-name kv-fsh-prod --name "JwtOptions--Key" --value "YOUR_JWT_KEY"

# Hangfire Password
az keyvault secret set --vault-name kv-fsh-prod --name "HangfireOptions--Password" --value "YOUR_HANGFIRE_PASSWORD"

# Mail Password
az keyvault secret set --vault-name kv-fsh-prod --name "MailOptions--Password" --value "YOUR_MAIL_PASSWORD"

# Graph API Client Secret
az keyvault secret set --vault-name kv-fsh-prod --name "GraphApi--ClientSecret" --value "YOUR_GRAPH_API_SECRET"
```

### 3. Configure Application Access

#### Option A: Using Managed Identity (Recommended for Azure hosting)

```bash
# Enable system-assigned managed identity on your App Service
az webapp identity assign --name your-app-service --resource-group your-rg

# Grant Key Vault access to the managed identity
az keyvault set-policy --name kv-fsh-prod --object-id <MANAGED_IDENTITY_OBJECT_ID> --secret-permissions get list
```

#### Option B: Using Service Principal

```bash
# Create service principal
az ad sp create-for-rbac --name sp-fsh-keyvault --skip-assignment

# Grant Key Vault access
az keyvault set-policy --name kv-fsh-prod --spn <SERVICE_PRINCIPAL_APP_ID> --secret-permissions get list
```

### 4. Update appsettings.Production.json

```json
{
  "KeyVault": {
    "Enabled": true,
    "VaultUri": "https://kv-fsh-prod.vault.azure.net/",
    "UseUserSecrets": false
  }
}
```

For service principal authentication, also add:
```json
{
  "KeyVault": {
    "Enabled": true,
    "VaultUri": "https://kv-fsh-prod.vault.azure.net/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "SERVICE_PRINCIPAL_CLIENT_ID",
    "ClientSecret": "SERVICE_PRINCIPAL_CLIENT_SECRET",
    "UseUserSecrets": false
  }
}
```

## Security Best Practices

1. **Never commit secrets to source control**
2. **Use different Key Vaults for different environments**
3. **Rotate secrets regularly**
4. **Use Managed Identity when hosting in Azure**
5. **Limit Key Vault access to only necessary permissions**
6. **Enable Key Vault logging and monitoring**

## Troubleshooting

### Common Issues

1. **"Access Denied" errors**
   - Verify the service principal or managed identity has proper permissions
   - Check Key Vault firewall rules

2. **Secrets not loading**
   - Ensure KeyVault:Enabled is set to true
   - Verify the VaultUri is correct
   - Check application logs for detailed errors

3. **Local development issues**
   - Run `dotnet user-secrets list` to verify secrets are set
   - Ensure you're in the correct project directory (src/api/server)

## Secret Structure

The application expects the following secrets:

| Secret Name | Description | Example |
|------------|-------------|---------|
| AuthenticationOptions:AzureAdB2C:ClientSecret | B2C Application client secret | KQk8Q~... |
| DatabaseOptions:ConnectionString | Database connection string | Server=... |
| JwtOptions:Key | JWT signing key (Base64) | QsJbcz... |
| HangfireOptions:Password | Hangfire dashboard password | Secure... |
| MailOptions:Password | SMTP password | wygzuX... |
| GraphApi:ClientSecret | Graph API client secret | YOUR_SECRET |

## Migration from appsettings.json

The secrets have been removed from appsettings.json for security. The application will now:
1. First check Azure Key Vault (if enabled)
2. Then check User Secrets (in development)
3. Finally fall back to appsettings.json (for non-sensitive config only)