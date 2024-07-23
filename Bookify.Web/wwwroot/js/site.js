// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var updatedRow;
function OnSuccessSubmit(item) {
    var tablebody = $('#category-table-body');

    if (updatedRow === undefined)
        tablebody.append(item);
    else {
        updatedRow.replaceWith(item);
        updatedRow = undefined;
    }

    $('#Modal').modal('hide');
    ShowSuccessMessage();
}

function ShowSuccessMessage(message="Saved Successfully") {
    Swal.fire({
        position: "center",
        icon: "success",
        title: "Your work has been saved",
        showConfirmButton: false,
        timer: 1000
    });
}

function ShowErrorMessage(message = "Something went wrong!") {
    Swal.fire({
        position: "center",
        icon: "error",
        title: "Oops...",
        text: message,
        showConfirmButton: false,
        timer: 1000
    });
}

function ShowToastrMessage(type,message) {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    toastr[type](message);

}


// Handle Toggle status 
// Attach a click event handler to all elements with class 'js-btn-toggle-status'
$('table').on('click', '.js-btn-toggle-status', function () {
    var btn = $(this);

    // Make an AJAX POST request to the server to toggle the status
    $.ajax({
        url: "/Categories/ToggleStatus/" + btn.data('id'),
        method: "post",
        data: {
            "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (res) {
            var isActiveCell = btn.parents('tr').find('.js-toggle-status');
            var newStatus = isActiveCell.html() === "Available" ? "Deleted" : "Available";


            isActiveCell.html(newStatus);
            isActiveCell.toggleClass("badge-success badge-danger");
            
            // Update the last updated on cell with the new timestamp from the server response
            btn.parents('tr').find('.js-last-updated-on').html(res.lastUpdatedOn);
            ShowToastrMessage('success', 'Toggled successfully');
        },
        error: function () {
            ShowErrorMessage();
        }
    });
});

// Handle bootstrap modal
$('body').on('click', '.js-render-modal', function () {
    var modal = $('#Modal');
    var btn = $(this);

    modal.modal('show');
    $('#ModalLabel').text(btn.data('modal-title'));

    // for update row 
    if (btn.data("update") != undefined)
        updatedRow = btn.parents("tr");
    $.ajax({
        url: btn.data('url'),
        method: 'GET',
        success: function (res) {
            modal.find('.modal-body').html(res);
            //apply validation on form when add form (because validation scripts exsits before and we added form after it and didn't know to apply validations on form)
            $.validator.unobtrusive.parse(modal);
        },
        error: function () {
            ShowErrorMessage();
        }
    });

});