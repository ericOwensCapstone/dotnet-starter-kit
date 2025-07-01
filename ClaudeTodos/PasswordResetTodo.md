# Password Reset Implementation Plan

## Overview
This plan addresses two key requirements for password reset functionality:
1. **Thing 1**: Implement proper handling of the B2C password reset flow when users click "Forgot your password?"
2. **Thing 2**: Create a B2C template for the password reset user flow that matches the existing unified.html styling

## Thing 1: Implement B2C Password Reset Flow Handling

### Current Issue
When users click "Forgot your password?" on the B2C login page, they are redirected back to the app with error parameters instead of being shown the password reset flow. The app receives:
```
https://localhost:7100/authentication/login-callback#error=access_denied&error_description=AADB2C90118%3a+The+user+has+forgotten+their+password...
```

### Implementation Steps

#### Step 1: Update B2CAuthenticationService to Parse Errors
**File**: `/src/apps/blazor/infrastructure/Auth/AzureB2C/B2CAuthenticationService.cs`

1. Modify the `ProcessAuthenticationCallbackAsync` method (lines 122-173) to:
   - Parse the `error` and `error_description` parameters from the URL fragment
   - Check specifically for the AADB2C90118 error code
   - Return a new result type that indicates password reset is needed

2. Update the `ParseFragment` method (lines 175-193) to handle error parameters

3. Add a new method `NavigateToPasswordReset` that constructs the B2C password reset URL using:
   - The `ResetPasswordPolicyId` from configuration (already exists as "B2C_1_passwordreset")
   - Same redirect URI pattern as login
   - Proper scopes and response modes

**Test Point**: Log the parsed error parameters to console and verify AADB2C90118 is detected

#### Step 2: Create AuthenticationCallbackResult Enum
**New File**: `/src/apps/blazor/infrastructure/Auth/AuthenticationCallbackResult.cs`

Create an enhanced result type that can indicate:
- Success (with redirect URL)
- Failure (with error message and redirect URL)
- PasswordResetRequired (triggers navigation to B2C password reset)

#### Step 3: Update AuthenticationCallbackHandler
**File**: `/src/apps/blazor/infrastructure/Auth/AuthenticationCallbackHandler.cs`

1. Modify `HandleB2CLoginCallbackAsync` method (lines 42-79) to:
   - Handle the new PasswordResetRequired result type
   - When password reset is required, redirect to B2C password reset flow
   - Maintain the user's original return URL in state

**Test Point**: Verify that clicking "Forgot password?" now redirects to B2C password reset page

#### Step 4: Handle Password Reset Callback
**File**: `/src/apps/blazor/infrastructure/Auth/AuthenticationCallbackHandler.cs`

1. Add a new case in `HandleCallbackAsync` for "passwordreset-callback"
2. Process the password reset completion similar to login-callback
3. Redirect user to login page with a success message

**Test Point**: Complete a password reset and verify user is redirected appropriately

#### Step 5: Update Navigation Routes
**File**: `/src/apps/blazor/client/Pages/Auth/AuthenticationCallback.razor`

Ensure the authentication callback page can handle the password reset flow URLs

### Configuration Requirements
Verify these settings exist in `appsettings.json`:
- `ResetPasswordPolicyId`: "B2C_1_passwordreset"
- Ensure the B2C tenant has the password reset user flow configured

## Thing 2: Create B2C Password Reset Template

### Template Requirements
Create a new template that exactly matches `unified.html` styling including:
- Dark theme with `--background-color: #1b1f22` and `--surface-color: #202528`
- Cornfield background image from Azure blob storage
- Roboto font family
- Green primary color scheme (`--primary-color: rgba(76,175,80,1)`)
- Same card layout with elevation shadow
- "OHD Harvest Marketplace" title styling

### Implementation Steps

#### Step 1: Create Base Template
**New File**: `/b2ctemplates/passwordreset.html`

1. Copy the entire structure from `unified.html`
2. Update the title to "Reset Password - OHD Harvest Marketplace"
3. Keep all CSS variables and styling intact
4. Maintain the same container and paper structure

#### Step 2: Adapt for Password Reset Flow
The B2C password reset flow typically has these stages:
1. Email verification (enter email, receive code)
2. Code verification (enter verification code)
3. New password entry (enter and confirm new password)

Ensure the template handles all three stages with proper styling.

#### Step 3: Add Password Reset Specific JavaScript
Add JavaScript to handle:
1. Form reorganization for password reset fields
2. Error message styling
3. Success message display
4. Progress indicators between stages

**Test Point**: Upload template to B2C and verify each stage displays correctly

#### Step 4: Create Supporting Assets
Ensure all assets referenced in the template are available:
- Cornfield background image (already at https://ohdb2ctemplates.blob.core.windows.net/ohdb2c-templates/cornfield.png)
- Roboto font from Google Fonts

### B2C Configuration Steps
1. In Azure Portal, navigate to your B2C tenant
2. Go to User flows > B2C_1_passwordreset
3. Select "Page layouts"
4. For each page in the flow:
   - Local account password reset page
   - Local account verification page
   - Password change page
5. Set each to use "Custom page content" with the passwordreset.html template URL

## Testing Plan

### Phase 1: Error Detection Testing
1. Click "Forgot password?" on login page
2. Verify AADB2C90118 error is logged in browser console
3. Confirm no authentication failure occurs

### Phase 2: Password Reset Flow Testing
1. Click "Forgot password?" and verify redirect to B2C password reset
2. Enter email address and request verification code
3. Enter verification code
4. Set new password
5. Verify redirect back to app with success indication

### Phase 3: Template Testing
1. Verify dark theme displays correctly at each stage
2. Confirm background image loads
3. Check form field styling matches login page
4. Test responsive behavior on mobile devices
5. Verify error messages are visible on dark background

### Phase 4: End-to-End Testing
1. Complete full password reset flow
2. Log in with new password
3. Verify all navigation states work correctly
4. Test edge cases (invalid email, expired code, etc.)

## Success Criteria
- Users clicking "Forgot password?" are redirected to B2C password reset flow
- Password reset pages match the exact styling of the login page
- Users can successfully reset their password and log in
- No authentication errors occur during the password reset process
- All three stages of password reset display correctly with dark theme

## Additional Notes
- The `AADB2C90118` error code is specific to B2C's way of handling password reset requests
- The password reset policy ID `B2C_1_passwordreset` must exist in your B2C tenant
- Consider adding user-friendly messages during the flow transitions
- Monitor for other B2C error codes that might need similar handling (e.g., AADB2C90091 for expired passwords)