
function OnAddCopyBookSuccess(row) {

    // hide model and update table by adding the new row that coming form controller
    $('#Modal').modal('hide');
    ShowSuccessMessage();
    $('.js-book-copy-table').prepend(row);

    // show alert when no copies and show table when there is copies
    $('.js-alert').addClass('d-none');
    $('.js-book-copy-table').removeClass('d-none');

    // update number of copies in card-header
    var numOfCopies = parseInt($('.js-copies-number').text());
    $('.js-copies-number').text(numOfCopies + 1);
}
function OnEditCopyBookSuccess(row) {
    $('#Modal').modal('hide');

    ShowSuccessMessage();

    $(updatedRow).replaceWith(row);
}