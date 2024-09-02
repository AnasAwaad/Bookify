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
                $('#subscriperStatus').find('.js-subscriper-status-text').text('Active Subscriber');
                $('#subscriberStatusBadge').removeClass('badge-light-warning').addClass('badge-light-success').text('Active member');

            },
            error: function (error) {
                ShowErrorMessage();
            }
        })
    })
})