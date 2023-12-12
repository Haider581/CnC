$(document).ready(function () {
    $("form[name='formTestEmail']").validate({
        // Specify validation rules
        rules: {
            emailAccounts: {
                required: true,
            },
            emailTo: {
                email: true,
                maxlength: 50,
                required: true
            },
            submitHandler: function (form) {
                form.submit();
            }
        }
    });
});