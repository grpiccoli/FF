$(function () {
    // when the modal is closed
    $('#modal-action').on('hidden.bs.modal', function () {
        // remove the bs.modal data attribute from it
        $(this).removeData('bs.modal');
        // and empty the modal-content element
        $('#modal-action .modal-content').empty();
    });
    $('#modal-container').change(function () {
        $.validator.unobtrusive.parse('form#modal-form');
    });
});