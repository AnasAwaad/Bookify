$(function () {

    $(document).on('click','.page-link', function () {
        var btn = $(this);
        var pageNumber = btn.data('page-number');

        if (!btn.parent().hasClass('active')) {
            $('#pageNumber').val(pageNumber);
            $('#ReportsForm').submit();
        }
    })

    $('.js-date-range').daterangepicker({
        autoUpdateInput: false,
        autoApply: true,
        showDropdowns: true,
        minYear: 2023,
        maxYear: parseInt(moment().format('YYYY'), 10),
        maxDate: moment()

    });

    $('.js-date-range').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
    });
})