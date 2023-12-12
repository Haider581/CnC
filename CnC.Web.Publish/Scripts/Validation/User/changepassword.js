// submit form data
// Wait for the DOM to be ready
$(function () {
    jQuery.validator.addMethod("pwcheck", function (value) {
        return /^[A-Za-z0-9\d=!\-@._*]*$/.test(value) // consists of only these
            && /[a-z]/.test(value) // has a lowercase letter
            && /\d/.test(value) // has a digit
    }, "choose strong password: at least one lower-case character, at least one digit, Allowed Characters: A-Z a-z 0-9 @ * _ - . !");
    $("form[name='changePasswordForm']").validate({
        rules: {
            oldPassword: "required",
            password: {
                required: true,
                minlength: 8,
                maxlength: 20,
                pwcheck: true
            },
            cfmPassword: {
                required: true,
                equalTo: "#password",
                minlength: 8,
                maxlength: 20,
                pwcheck: true
            }
        },
        messages: {
            password: {
                required: "the password is required"
            }
        }
    });
});