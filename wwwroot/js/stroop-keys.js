// wwwroot/js/stroop-keys.js
//
// Stroop.razor needs arrow-key input to work without requiring the
// user to click into the game area first (clicking one of the answer
// buttons, or Bootstrap's modal returning focus to its trigger button
// after closing, moves DOM focus away from the game div). Listening
// on the document instead of a focusable element sidesteps browser
// focus entirely.

window.stroopKeys = (function () {
    let handler = null;

    return {
        register: function (dotNetRef) {
            if (handler) return;
            handler = function (e) {
                dotNetRef.invokeMethodAsync('OnGlobalKeyDown', e.key, e.repeat);
            };
            document.addEventListener('keydown', handler);
        },
        unregister: function () {
            if (handler) {
                document.removeEventListener('keydown', handler);
                handler = null;
            }
        }
    };
})();
