

var g_asiBaseURL ="/ZTBLAIS";
var g_secretKey="";

$(document).ready(function () {
    // Override default options for all modals
    $.fn.modal.Constructor.Default.backdrop = 'static';
    $.fn.modal.Constructor.Default.keyboard = false;

    $('body').append('<div id="alertMessagesPopup" class="modal" tabindex="-1" role="dialog"><div class="modal-dialog" role="document">  <div class="modal-content">    <div class="modal-header">      <h5 class="modal-title">Alert</h5>      <button type="button" class="close" data-dismiss="modal" aria-label="Close">        <span aria-hidden="true">&times;</span>      </button>    </div>    <div class="modal-body">      <div id="content_alertMessagesPopup"></div>    </div>    <div class="modal-footer"><button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>    </div>  </div></div></div >');
    $('#alertMessagesPopup').on('hidden.bs.modal', function (e) {
        closeFuncCalled();
    });

    $('body').append('<div id="confirmAlertMessagesPopup" class="modal" tabindex="-1" role="dialog"><div class="modal-dialog" role="document">  <div class="modal-content">    <div class="modal-header">      <h5 class="modal-title">Confirmation Box</h5>      <button type="button" class="close" data-dismiss="modal" aria-label="Close">        <span aria-hidden="true">&times;</span>      </button>    </div>    <div class="modal-body">      <div id="content_confirmAlertMessagesPopup"></div>    </div>    <div class="modal-footer"><button type="button" onclick="event.preventDefault();onConfirmationCallback();" class="btn btn-danger" data-dismiss="modal">Yes</button><button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>    </div>  </div></div></div >');
    $('#confirmAlertMessagesPopup').on('hidden.bs.modal', function (e) {
        confirmAlertcloseFuncCalled();
    });

    $('.modal').on("hidden.bs.modal", function (e) { //fire on closing modal box
        if ($('.modal:visible').length) { // check whether parent modal is opend after child modal close
            $('body').addClass('modal-open'); // if open mean length is 1 then add a bootstrap css class to body of the page
        }
    });

    $('.modal').on('show.bs.modal', function (e) {
        if (!($('.modal.in').length)) {
            $('.modal-dialog').css({
                top: 0,
                left: 0
            });
        }
        $('.modal-dialog').draggable();      
    });


    $('.modal').on('shown.bs.modal', function (e) {
        $(this).modal({
            backdrop: 'static',
            keyboard: true
        });
    });




    //$.ajaxSetup({
    //    beforeSend: function (xhr, settings) {
    //        // Append the directory to the start of the URL
    //        settings.url = g_asiBaseURL+  settings.url;
    //    }
    //});
});
function alert(message) {
    $('#content_alertMessagesPopup').empty();
    $('#content_alertMessagesPopup').html(message);
    $('#alertMessagesPopup').modal('show');
}
function extractPlainText(clobContent) {
    // Implement your logic here to extract plain text from CLOB content
    // This might involve removing HTML tags or any other formatting

    // For example, a basic approach might involve removing HTML tags using a regular expression
    var plainText = clobContent.replace(/<[^>]+>/g, '');

    return plainText;
}
function onAlertCallback(funcToCall) {
    closeFuncCalled = funcToCall;
}
function closeFuncCalled() {

}
function confirmAlert(message) {
    $('#content_confirmAlertMessagesPopup').empty();
    $('#content_confirmAlertMessagesPopup').html(message);
    $('#confirmAlertMessagesPopup').modal('show');
}
function onconfirmAlertCallback(funcToCall) {
    onConfirmationCallback = funcToCall;
}
function onConfirmationCallback() {

}
function confirmAlertcloseFuncCalled() { }
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
function getBase64(file) {
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        return reader.result;
    };
    reader.onerror = function (error) {
        return "";
    };
}

function encryptPassword(password) {
    return btoa(password);
}





