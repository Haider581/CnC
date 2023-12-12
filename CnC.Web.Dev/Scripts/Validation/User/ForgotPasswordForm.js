// submit form data
// Wait for the DOM to be ready
$(function () {
    jQuery.validator.addMethod("pwcheck", function (value) {
        return /^[A-Za-z0-9\d=!/[\@\#\$\%\^\&\*\(\)\_\+\!]/.test(value) // consists of only these
            && /[a-z]/.test(value) // has a lowercase letter 
            && /[A-Z]/.test(value)
            && /[\@\#\$\%\^\&\*\(\)\_\+\!]/.test(value)
            && /\d/.test(value) // has a digit
    }, "choose strong password: At-least one lower-case character, At-least one upper-case character, At-least one digit, At-least one special characters from allowed list, Allowed Characters: A-Z, a-z, 0-9, @ # $ % ^ & * () _ + !");
    $("form[name='ForgotPasswordForm']").validate({
        rules: {
            oldPassword: "required",
            txtEmailAddress: {
                required: true,
                email: true,
                maxlength: 50
            },
            txtVerificationCode: {
                required: true,
                minlength: 6,
                maxlength: 6,
                digits: true
            },
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