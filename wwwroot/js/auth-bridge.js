// wwwroot/js/auth-bridge.js
//
// Sits inside the Blazor app and listens for a message posted in
// from the parent WordPress window. That message contains the
// user's login token AND their display name, so Blazor doesn't
// need to make an extra call just to say "Hello [Name]".

window.wordGameAuth = (function () {
    let authData = null;
    let resolveWaiter = null;
    let waiterPromise = new Promise((resolve) => { resolveWaiter = resolve; });

    window.addEventListener('message', function (event) {
        // For now this accepts messages from any origin so it's
        // easy to get working. Once it's working, tighten this to
        // check event.origin === "https://yourwordpresssite.com"

        const data = event.data;
        if (data && data.type === 'WORDGAME_AUTH') {
            authData = {
                token: data.token,
                displayName: data.displayName
            };
            if (resolveWaiter) {
                resolveWaiter(authData);
                resolveWaiter = null;
            }
        }
    });

    return {
        // Blazor calls this and waits (it resolves once the
        // message has arrived from the parent page)
        waitForAuth: function () {
            return waiterPromise;
        }
    };
})();