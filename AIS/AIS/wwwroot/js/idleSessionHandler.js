$(document).ready(function () {
    const idleDurationSecs = 1800;    // X number of seconds
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
           window.location.href = "/Login";
        },
        dataType: "json",
    });
}