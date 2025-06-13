# B2C Invitation Integration Endpoints

This document describes the B2C-specific endpoints created for invitation validation during the Azure AD B2C signup flow.

## Endpoints

### 1. B2C Invitation Validation
**Endpoint**: `GET /api/public/b2c/invitations/validate/{token}`
**Purpose**: Validates an invitation token during B2C signup flow
**Authentication**: Anonymous/Public
**Response**: Returns invitation details if valid, error message if invalid

### 2. B2C Pre-Registration Validation
**Endpoint**: `POST /api/public/b2c/invitations/pre-validate`
**Purpose**: Called by B2C custom policy before user creation to validate invitation and get user metadata
**Authentication**: Anonymous/Public
**Request Body**:
```json
{
  "email": "user@example.com",
  "invitationToken": "token-value",
  "firstName": "John",
  "lastName": "Doe",
  "objectId": "b2c-object-id"
}
```
**Response**: Returns user metadata and custom attributes for B2C

### 3. B2C Post-Registration
**Endpoint**: `POST /api/public/b2c/invitations/post-registration`
**Purpose**: Called by B2C after successful user creation to mark invitation as accepted
**Authentication**: Anonymous/Public
**Request Body**:
```json
{
  "objectId": "b2c-object-id",
  "email": "user@example.com",
  "invitationToken": "token-value",
  "tenantId": "tenant-id"
}
```
**Response**: Confirms invitation acceptance

## Integration with B2C Custom Policies

These endpoints should be called from your B2C custom policies:

1. During signup, validate the invitation token
2. Before creating the user, call pre-validation to get user metadata
3. After user creation, call post-registration to mark invitation as accepted

The endpoints are designed to be resilient and will not block the B2C flow if there are non-critical errors.