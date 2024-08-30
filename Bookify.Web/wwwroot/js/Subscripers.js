$(function () {
	$('.js-selected-city').on('change', function () {
		var cityItem = $(this);
		$.get({
			url: "/Subscripers/GetAreasBasedOnCity",
			data: { cityId: cityItem.val() },
			success: function (res) {

				// clear all option
				$('.js-areas').html('').select2({ data: [{ id: '', text: '' }] });

				// clear and add new option
				$(".js-areas").html('').select2({
					data: [{ id: '', text: '' }, ...res.areas]
				});
			}

		})
	})
})