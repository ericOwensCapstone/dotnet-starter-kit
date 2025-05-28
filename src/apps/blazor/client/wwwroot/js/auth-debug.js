// Authentication debugging helper
window.authDebug = {
    logMsalState: function() {
        if (window.AuthenticationService) {
            console.log("[AuthDebug] MSAL Service exists");
            if (window.AuthenticationService.instance) {
                console.log("[AuthDebug] MSAL instance exists");
                
                // Try to get active account
                try {
                    const accounts = window.AuthenticationService.instance.getAllAccounts();
                    console.log(`[AuthDebug] Accounts found: ${accounts.length}`);
                    accounts.forEach((account, index) => {
                        console.log(`[AuthDebug] Account ${index}: ${account.username}`);
                    });
                } catch (e) {
                    console.error("[AuthDebug] Error getting accounts:", e);
                }
                
                // Check cache
                try {
                    const cache = window.AuthenticationService.instance.getTokenCache();
                    console.log("[AuthDebug] Token cache exists:", !!cache);
                } catch (e) {
                    console.error("[AuthDebug] Error accessing cache:", e);
                }
            }
        } else {
            console.log("[AuthDebug] MSAL Service not found");
        }
    },
    
    interceptLogin: function() {
        if (window.AuthenticationService && window.AuthenticationService.signIn) {
            const originalSignIn = window.AuthenticationService.signIn;
            window.AuthenticationService.signIn = function(state) {
                console.log("[AuthDebug] SignIn called with state:", state);
                
                // Log the MSAL configuration
                if (window.AuthenticationService.msalInstance) {
                    const config = window.AuthenticationService.msalInstance.config;
                    console.log("[AuthDebug] MSAL Config:", {
                        auth: config?.auth,
                        cache: config?.cache
                    });
                }
                
                return originalSignIn.call(this, state);
            };
            console.log("[AuthDebug] SignIn intercepted");
        }
        
        // Also check for handleRedirectPromise
        if (window.AuthenticationService && window.AuthenticationService.instance) {
            console.log("[AuthDebug] Checking for redirect result...");
            
            // Check if there's a redirect in progress
            const hash = window.location.hash;
            const search = window.location.search;
            console.log("[AuthDebug] Current hash:", hash);
            console.log("[AuthDebug] Current search:", search);
        }
    }
};

// Auto-run on load
window.addEventListener('load', () => {
    window.authDebug.logMsalState();
    window.authDebug.interceptLogin();
});