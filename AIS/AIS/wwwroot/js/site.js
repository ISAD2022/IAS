// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('body').append('<div id="alertMessagesPopup" class="modal" tabindex="-1" role="dialog"><div class="modal-dialog" role="document">  <div class="modal-content">    <div class="modal-header">      <h5 class="modal-title">Alert</h5>      <button type="button" class="close" data-dismiss="modal" aria-label="Close">        <span aria-hidden="true">&times;</span>      </button>    </div>    <div class="modal-body">      <div id="content_alertMessagesPopup"></div>    </div>    <div class="modal-footer"><button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>    </div>  </div></div></div >');
});
function alert(message) {
    $('#content_alertMessagesPopup').empty();
    $('#content_alertMessagesPopup').html(message);
    $('#alertMessagesPopup').modal('show');
}