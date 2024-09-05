
var selectedCopies = [];
console.log(maxAllowedCopies)
$(function () {
	$('.js-search').on('click', function (e) {
		e.preventDefault();
		var serialNumber = $("#Value").val();

		if (selectedCopies.find((copy)=>copy.serialNumber==serialNumber)) {
			ShowErrorMessage("You cannot add the same copy");
			return;
		}

		if (selectedCopies.length >= maxAllowedCopies) {
			ShowErrorMessage(`You cannot add more than ${maxAllowedCopies} of books`);
			return;
		}
		$('#BookCopyForm').submit();
	})
})
function OnGetCopySuccess(copy) {

	var bookId = $(copy).find('.js-copy').data('book-id');
	
	if (selectedCopies.find((val) => val.bookId == bookId)){
		ShowErrorMessage("You cannot add more than one copy for the same book");
		return;
	}

	$("#Value").val('');
	$('#CopiesForm').prepend(copy);

	selectedCopies = [];
	$('.js-copy').each(function (idx, ele) {
		var input = $(this);

		if (!selectedCopies.includes(input.val())) {

			selectedCopies.push({
				serialNumber: input.val(),
				bookId: input.data('book-id')
			});

			input.attr('name', `SelectedCopies[${idx}]`);
		}

	});
	
	console.log(selectedCopies)
}