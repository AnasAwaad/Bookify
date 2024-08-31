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
            },
            error: function (error) {
                ShowErrorMessage();
            }
        })
    })
})