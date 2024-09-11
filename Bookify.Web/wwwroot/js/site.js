// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var updatedRow;
var table;
var datatable;
var exportedCols = [];

function OnSuccessSubmit(row) {
    var newRow = $(row);


    // add new row to table
    datatable.row.add(newRow).draw();
    if (updatedRow !== undefined) {
        // Remove the old row
        datatable.row(updatedRow).remove().draw();
        updatedRow = undefined;
    }

    $('#Modal').modal('hide');
    ShowSuccessMessage();
}


function DisableSubmitButton() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}

function OnBeginModal() {
    DisableSubmitButton();
}
function OnCompleteModal() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator');
}

function ShowSuccessMessage(message = "Saved Successfully") {
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
        text: message.responseText ==undefined ? message : message.responseText,
        showConfirmButton: true,
    });
}

function ShowToastrMessage(type, message) {
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

// select2 event
// revalidation for this element when select item from selectList
function ApplySelect2() {
    $('.js-select2').select2();
    $('.js-select2').on('select2:select', function (e) {
        var selectItem = $(this);
        $('form').not('#signOutForm').not('.execlude-from-validation').validate().element("#" + selectItem.attr('id'));// ex: #AuthorId
    });
}

$(function () {

    // Handle disable submit button 
    $('form').not('#signOutForm').not('.execlude-from-validation').on('submit', function () {
        if ($(this).valid()) DisableSubmitButton();
    })


    // Handle Toggle status 
    // Attach a click event handler to all elements with class 'js-btn-toggle-status'
    $('table').on('click', '.js-btn-toggle-status', function () {
        var btn = $(this);

        // Make an AJAX POST request to the server to toggle the status
        $.ajax({
            url: btn.data('url'),
            method: "post",
            data: {
                "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                var isActiveCell = btn.parents('tr').find('.js-toggle-status');
                var newStatus = isActiveCell.html().trim() == "Available" ? "Not Available" : "Available";

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

    $('table').on('click', '.js-btn-unlockUser', function () {
        var btn = $(this);
        $.ajax({
            url: btn.data('url'),
            method: "POST",
            data: {
                "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                var lockedCell = btn.parents('tr').find('.js-unlockUser');
                lockedCell.html("Active");
                lockedCell.removeClass("badge-danger");
                lockedCell.addClass("badge-primary");
                ShowToastrMessage('success', 'Unlocked successfully');

            },
            error: function (message) {
                ShowErrorMessage(message);
            }
        })
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

                ApplySelect2();
                
            },
            error: function () {
                ShowErrorMessage();
            }
        });

    });

    // handle header in export datatable 
    var headers = $('th');
    $.each(headers, function () {
        var col = $(this);
        if (!col.hasClass('js-no-export'))
            exportedCols.push(col);
    })
    //Handle DataTable

    var KTDatatables = function () {


        // Private functions
        var initDatatable = function () {

            // Init datatable --- more info on datatables: https://datatables.net/manual/
            datatable = $(table).DataTable({
                "info": true,
                'pageLength': 10,
            });
        }

        // Hook export buttons
        var exportButtons = () => {
            const documentTitle = $('.js-data-table').data("export-title");
            var buttons = new $.fn.dataTable.Buttons(table, {
                buttons: [
                    {
                        extend: 'copyHtml5',
                        title: documentTitle,
                        exportOptions: {
                            columns: exportedCols
                        }
                    },
                    {
                        extend: 'excelHtml5',
                        title: documentTitle,
                        exportOptions: {
                            columns: exportedCols
                        }
                    },
                    {
                        extend: 'csvHtml5',
                        title: documentTitle,
                        exportOptions: {
                            columns: exportedCols
                        }
                    },
                    {
                        extend: 'pdfHtml5',
                        title: documentTitle,
                        exportOptions: {
                            columns: exportedCols
                        }
                    }
                ]
            }).container().appendTo($('#kt_datatable_example_buttons'));

            // Hook dropdown menu click event to datatable export buttons
            const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
            exportButtons.forEach(exportButton => {
                exportButton.addEventListener('click', e => {
                    e.preventDefault();

                    // Get clicked export value
                    const exportValue = e.target.getAttribute('data-kt-export');
                    const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                    // Trigger click event on hidden datatable export buttons
                    target.click();
                });
            });
        }

        // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
        var handleSearchDatatable = () => {
            const filterSearch = document.querySelector('[data-kt-filter="search"]');
            filterSearch.addEventListener('keyup', function (e) {
                datatable.search(e.target.value).draw();
            });
        }


        // Public methods
        return {
            init: function () {
                table = document.querySelector('.js-datatables');

                if (!table) {
                    return;
                }

                initDatatable();
                exportButtons();
                handleSearchDatatable();
            }
        };

    }();
    // On document ready
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });


    // Handle tinymce 
    if ($('.js-tinymce').length > 0) { // check if there is tinymce library js file in page
        

        if (KTThemeMode.getMode() === "dark") {
            options["skin"] = "oxide-dark";
            options["content_css"] = "dark";
        }
        tinymce.init({
            selector: ".js-tinymce",
            height: "350",
            setup: function (editor) {
                // Sync content with textarea on change
                editor.on('change', function () {
                    editor.save();
                    // Manually trigger form validation
                    var textareaId = editor.getElement().id;
                    $('form').not('#signOutForm').validate().element("#" + textareaId);
                });
            }
        });
    }


    // Handle DatePicker
    $('.js-datepicker').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        maxDate:new Date(),
        autoApply: false,
        timePicker: false,

    });

    // select2
    ApplySelect2();

    //$('.js-select2-modal').select2({
    //    dropdownParent: $('#Modal')
    //});

    // Trigger validation when file input changes
    $('#ImageFile').on('change', function () {
        // Clear the validation message when the input changes
        $('#validationImageMessage').text('');
    });

    // Handle signout button
    $('.js-sign-out').on('click', function () {
        $('#signOutForm').submit();
    });


})