// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {      
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

function setCookie(name, value, daysToLive = undefined) {
    // Encode value in order to escape semicolons, commas, and whitespace
    var cookie = name + "=" + encodeURIComponent(value);

    if (typeof daysToLive === "number") {
        /* Sets the max-age attribute so that the cookie expires
        after the specified number of days */
        cookie += "; max-age=" + (daysToLive * 24 * 60 * 60);
    }

    document.cookie = cookie;
}

function getCookie(name) {
    // Split cookie string and get all individual name=value pairs in an array
    var cookieArr = document.cookie.split(";");

    // Loop through the array elements
    for (var i = 0; i < cookieArr.length; i++) {
        var cookiePair = cookieArr[i].split("=");

        /* Removing whitespace at the beginning of the cookie name
        and compare it with the given string */
        if (name == cookiePair[0].trim()) {
            // Decode the cookie value and return
            return decodeURIComponent(cookiePair[1]);
        }
    }

    // Return null if not found
    return null;
}