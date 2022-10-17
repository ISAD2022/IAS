$(document).ready(function () {
    const idleDurationSecs = 30;    // X number of seconds
    const redirectUrl = "/ApiCalls/terminate_idle_session";  // Redirect idle users to this URL
    let idleTimeout; // variable to hold the timeout, do not modify

    const resetIdleTimeout = function () {

        // Clears the existing timeout
        if (idleTimeout) clearTimeout(idleTimeout);

        // Set a new idle timeout to load the redirectUrl after idleDurationSecs
        idleTimeout = setTimeout(() => terminateUserSession(), idleDurationSecs * 1000);
    };

    // Init on page load
    resetIdleTimeout();

    // Reset the idle timeout on any of the events listed below
    ['click', 'touchstart', 'mousemove'].forEach(evt =>
        document.addEventListener(evt, resetIdleTimeout, false)
    );
});
function terminateUserSession() {
    $.ajax({
        url: "/ApiCalls/terminate_idle_session",
        type: "POST",
        data: {
         
        },
        cache: false,
        success: function (data) {
            if (data) {
                window.location.href = "/Login";
            }
        },
        dataType: "json",
    });
}