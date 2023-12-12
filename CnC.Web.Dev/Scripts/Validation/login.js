// submit form data
$(document).ready(function () {
    $("#btnPost").click(function () {
        $("#myform").validate();

        if (!$("#myform").valid()) {
            return;
        }
    }
    );
});
$(function () {

    $("form[name='myform']").validate({
        // Specify validation rules
        rules: {
            Username: "required",
            Password: {
                required: true,
                minlength: 8
            },
        },

        messages: {
            Username: "Username is required",
            Password: {
                required: "Password is required",
                minlength: "Enter at least 8 characters",
            }
        },

        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        //submitHandler: function (form) {
        //    form.submit();
        //}
    });
});