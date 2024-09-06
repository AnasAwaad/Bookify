$(function () {
    $('.js-renew-subscription').on('click', function () {
        var subscriperKey = $(this).data("key");
        $.ajax({
            url: `/Subscripers/renewSubscription?subscriperKey=${subscriperKey}`,
            type: "post",
            data: {
                "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                $('#subscriptionsTable').find('tbody').append(res);
                $('#subscriperStatus').removeClass('bg-warning').addClass('bg-success');
                $('#AddRentalBtn').removeClass('d-none');
                $('#subscriperStatus').find('.js-subscriper-status-text').text('Active Subscriber');
                $('#subscriberStatusBadge').removeClass('badge-light-warning').addClass('badge-light-success').text('Active member');

            },
            error: function (error) {
                ShowErrorMessage();
            }
        })
    });
    $('.js-remove-rental').on('click', function () {
        var btn = $(this);
        bootbox.confirm({
            message: 'Are you sure that you want to remove this rental?',
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-success'
                }
            },
            callback: function (result) {
                console.log("remove");
                var subscriperId = btn.data("id");
                $.ajax({
                    url: `/Rentals/RemoveRental/${subscriperId}`,
                    type: "post",
                    data: {
                        "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (res) {
                        btn.parents('tr').remove();
                        if ($('#RentalsTable tbody tr').length == 0) {
                            $('#RentalsTable').fadeOut();
                            $('#RentalsAlert').fadeIn();
                        }
                    },
                    error: function (error) {
                        ShowErrorMessage();
                    }
                })
            }
        });
        
    });


})