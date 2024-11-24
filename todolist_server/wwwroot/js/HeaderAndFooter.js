$(function () {
    $('#selectedTime').on('input', function () {

        if ($('#selectedTime').val() !== '') {
            let splitTime = $('#selectedTime').val().split('-');
            $(location).prop('href', `${window.location.origin}/Web/Task?month=${splitTime[1]}&year=${splitTime[0]}`);
        }
    })


    $('#searchBar').on('keydown', function (e) {
        if (e.which == 13) {
            let keyword = $(this).val();
            $(location).prop('href', `${window.location.origin}/Web/Task?keyword=` + keyword);
        }
    })
})