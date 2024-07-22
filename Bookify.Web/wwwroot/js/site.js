// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function OnSuccessSubmit(item) {
    $('#category-table-body').append(item);
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