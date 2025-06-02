# Setup Scripts

This directory contains template scripts for setting up user secrets in development.

## Important Security Notice

**NEVER commit actual secrets to the repository!**

The scripts in this directory are templates with placeholder values. Before using them:

1. **Copy the script** to a local version:
   ```bash
   cp setup-user-secrets.sh setup-user-secrets.local.sh
   # or
   cp setup-user-secrets.ps1 setup-user-secrets.local.ps1
   ```

2. **Edit the local copy** with your actual secret values

3. **Run the local copy** to set up your user secrets

The `.gitignore` file in this directory ensures that `*.local.*` files are never committed.

## Required Secrets

You'll need to provide values for:
- `AuthenticationOptions:AzureAdB2C:ClientSecret` - Your B2C application client secret
- `DatabaseOptions:ConnectionString` - Your database connection string with password
- `JwtOptions:Key` - A Base64-encoded JWT signing key
- `HangfireOptions:Password` - Password for Hangfire dashboard
- `MailOptions:Password` - SMTP server password
- `GraphApi:ClientSecret` - Microsoft Graph API client secret

## Generating a JWT Key

To generate a secure JWT key:
```powershell
# PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }) -as [byte[]])
```

```bash
# Linux/Mac
openssl rand -base64 32
```