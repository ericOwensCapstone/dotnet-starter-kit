# Future Optional TODOs

This file consolidates all optional, additional, and future enhancement items extracted from the various TODO files in the ClaudeTodos directory. These items are not part of the core implementation plans but represent valuable enhancements for security, user experience, and code quality.

## Security Enhancements

### From InvitationAcceptanceSecurityTodo.md

#### 1. Implement Rate Limiting for Invitation Acceptance Page
**Priority**: High  
**Source**: InvitationAcceptanceSecurityTodo.md (Additional TODO Items)

To prevent brute force attacks on invitation tokens, implement rate limiting:

**Implementation Requirements**:
1. Add rate limiting middleware or service
2. Configure limits for invitation acceptance endpoint:
   - 5 attempts per IP address per hour
   - 20 attempts per invitation token per day
   - 100 total attempts per IP per day

**Files to modify**:
- Add rate limiting configuration in `Program.cs`
- Create custom rate limiting policy for invitation endpoints
- Add telemetry for rate limit violations

**Testing**:
- Verify rate limits are enforced
- Confirm legitimate users aren't blocked
- Test rate limit reset after time window

### From B2CLoginSecurityTodo.md

#### 2. Enhanced B2C Token Validation
**Priority**: High  
**Source**: B2CLoginSecurityTodo.md (Recommended Security Enhancements)

Implement comprehensive token validation beyond basic JWT verification:

1. **Proper B2C Token Validation**
   - Validate issuer matches expected B2C tenant
   - Verify audience matches your client ID
   - Check token hasn't been used before (implement token usage tracking)

2. **Implement Token Usage Tracking**
   - Store used token JTIs in cache
   - Prevent token replay attacks
   - Set expiration based on token lifetime

3. **Add Time Window Check**
   - Ensure tokens are processed within reasonable time (e.g., 5 minutes)
   - Reject tokens with suspicious timing

4. **Validate State Parameter**
   - Implement proper state parameter validation
   - Store state in session before redirect
   - Validate on callback

5. **Consider Authorization Code Flow**
   - More secure than implicit flow
   - Requires backend token exchange

6. **Secure Token Transport**
   - Ensure all authentication endpoints use HTTPS
   - Implement secure cookie settings

### From PasswordResetTodo.md

#### 3. Monitor Additional B2C Error Codes
**Priority**: Medium  
**Source**: PasswordResetTodo.md (Additional Notes)

Extend error handling to cover more B2C scenarios:
- `AADB2C90091`: Expired password requiring reset
- `AADB2C90157`: Social account email conflict
- Other B2C-specific error codes that might need special handling

## Architecture Improvements

### From InvitationAcceptanceSecurityTodo.md

#### 4. Fix Command Types and IUserService Architecture
**Priority**: Medium  
**Source**: InvitationAcceptanceSecurityTodo.md (Additional TODO Items)

Address DDD principle violations and compilation issues:

**Issues to resolve**:
1. `CreateUserWithInvitationCommand` is in Application layer but uses Domain types
2. `IUserService` in Domain layer has methods returning Application DTOs
3. Circular dependency between layers

**Solution Options**:

**Option A**: Move invitation-specific commands to Domain
- Create `Domain.Users.Commands` namespace
- Move `CreateUserWithInvitationCommand` there
- Keep it as domain command since it uses domain types

**Option B**: Create proper separation
- Keep command in Application layer
- Create domain service interface that doesn't return DTOs
- Map between domain and application concerns properly

**Files to refactor**:
- `/src/api/application/Users/Commands/CreateUserWithInvitationCommand.cs`
- `/src/api/domain/Users/IUserService.cs`
- Related command handlers and services

## User Experience Enhancements

### From SeamlessLoadingTodo.md

#### 5. Advanced Loading Experience Features
**Priority**: Low  
**Source**: SeamlessLoadingTodo.md (Future Enhancements)

Enhance the loading experience with:

1. **Progress Percentage**
   - Show actual progress if possible
   - Implement progress tracking for multi-step operations

2. **Skeleton Screens**
   - Create skeleton loaders for partial content
   - Show UI structure while loading

3. **Subtle Animations**
   - Add smooth transitions to spinner
   - Consider fade effects for panel transitions

4. **Role-Based Messages**
   - Different loading messages for different user types
   - Personalized experience based on context

5. **Offline Detection**
   - Detect when user is offline
   - Show appropriate messaging
   - Queue actions for when connection returns

## Additional Monitoring & Observability

### From Multiple Sources

#### 6. Comprehensive Telemetry Implementation
**Priority**: Medium

Add telemetry for:
- Authentication flow completion times
- Rate limit violations
- Token validation failures
- Loading time metrics
- User flow drop-off points

## Implementation Priority Guide

### High Priority (Security Critical)
1. Rate Limiting for Invitation Acceptance
2. Enhanced B2C Token Validation
3. Token Usage Tracking

### Medium Priority (Architecture & Stability)
4. Fix Command Types and IUserService Architecture
5. Monitor Additional B2C Error Codes
6. Comprehensive Telemetry Implementation

### Low Priority (User Experience)
7. Advanced Loading Experience Features
8. Progress Indicators
9. Skeleton Screens
10. Role-Based Messaging

## Notes

- All security enhancements should be implemented before going to production
- Architecture improvements will make the codebase more maintainable
- User experience enhancements can be added incrementally based on user feedback
- Consider creating separate GitHub issues for each major item for better tracking