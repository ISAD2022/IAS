// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
  

        const idleDurationSecs = 1800;    // X number of seconds
        const redirectUrl = "/Login/logout";  // Redirect idle users to this URL
        let idleTimeout; // variable to hold the timeout, do not modify

        const resetIdleTimeout = function () {

            // Clears the existing timeout
            if (idleTimeout) clearTimeout(idleTimeout);

            // Set a new idle timeout to load the redirectUrl after idleDurationSecs
            idleTimeout = setTimeout(() => location.href = redirectUrl, idleDurationSecs * 1000);
        };

        // Init on page load
        resetIdleTimeout();

        // Reset the idle timeout on any of the events listed below
        ['click', 'touchstart', 'mousemove'].forEach(evt =>
            document.addEventListener(evt, resetIdleTimeout, false)
        );
    $('body').append('<div id="alertMessagesPopup" class="modal" tabindex="-1" role="dialog"><div class="modal-dialog" role="document">  <div class="modal-content">    <div class="modal-header">      <h5 class="modal-title">Alert</h5>      <button type="button" class="close" data-dismiss="modal" aria-label="Close">        <span aria-hidden="true">&times;</span>      </button>    </div>    <div class="modal-body">      <div id="content_alertMessagesPopup"></div>    </div>    <div class="modal-footer"><button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>    </div>  </div></div></div >');
    $('#alertMessagesPopup').on('hidden.bs.modal', function (e) {
        closeFuncCalled();
    })
});
function alert(message) {
    $('#content_alertMessagesPopup').empty();
    $('#content_alertMessagesPopup').html(message);
    $('#alertMessagesPopup').modal('show');
}
function onAlertCallback(funcToCall) {
    closeFuncCalled = funcToCall;
}
function closeFuncCalled() {

}
