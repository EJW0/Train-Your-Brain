// wwwroot/js/fullscreen.js
//
// Wraps the browser Fullscreen API for the fullscreen toggle button in
// TopBar.razor. Fullscreen can only be entered from a real user gesture
// (a click), so this is called directly from the button's onclick.

window.fullscreenHelper = (function () {
    let dotNetRef = null;

    function isFullscreen() {
        return !!document.fullscreenElement;
    }

    document.addEventListener('fullscreenchange', function () {
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnFullscreenChanged', isFullscreen());
        }
    });

    return {
        register: function (ref) {
            dotNetRef = ref;
        },
        unregister: function () {
            dotNetRef = null;
        },
        toggle: function () {
            if (isFullscreen()) {
                document.exitFullscreen().catch(function (err) {
                    console.warn('Could not exit fullscreen:', err.message);
                });
            } else if (document.fullscreenEnabled) {
                document.documentElement.requestFullscreen().catch(function (err) {
                    console.warn('Fullscreen unavailable (likely blocked by the embedding page\'s permissions policy):', err.message);
                });
            } else {
                console.warn('Fullscreen is disabled by the embedding page\'s permissions policy.');
            }
        },
        isFullscreen: isFullscreen
    };
})();
