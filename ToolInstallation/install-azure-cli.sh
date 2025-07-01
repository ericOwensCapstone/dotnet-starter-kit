#!/bin/bash

# Azure CLI Installation Script for WSL
echo "Installing Azure CLI on WSL..."

# Update package index
apt-get update

# Install prerequisites
apt-get install -y ca-certificates curl apt-transport-https lsb-release gnupg

# Add Microsoft GPG key
curl -sL https://packages.microsoft.com/keys/microsoft.asc | \
    gpg --dearmor | \
    tee /etc/apt/trusted.gpg.d/microsoft.gpg > /dev/null

# Add Azure CLI repository
AZ_REPO=$(lsb_release -cs)
echo "deb [arch=amd64] https://packages.microsoft.com/repos/azure-cli/ $AZ_REPO main" | \
    tee /etc/apt/sources.list.d/azure-cli.list

# Update package index with new repo
apt-get update

# Install Azure CLI
apt-get install -y azure-cli

echo "Azure CLI installation complete!"
echo "Version:"
az --version