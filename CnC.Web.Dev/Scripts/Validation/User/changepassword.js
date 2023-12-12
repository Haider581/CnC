// submit form data
// Wait for the DOM to be ready
$(function () {
    var a = 2;
    jQuery.validator.addMethod("pwcheck", function (value) {
        return /^[A-Za-z0-9\d=!/[\@\#\$\%\^\&\*\(\)\_\+\!]/.test(value) // consists of only these
            && /[a-z]/.test(value) // has a lowercase letter 
            && /[A-Z]/.test(value)
            && /[\@\#\$\%\^\&\*\(\)\_\+\!]/.test(value)
            && /\d/.test(value) // has a digit
    }, GetLocalizedString("StrongPasswordCheck"));
    //jQuery.validator.addMethod("notEqual", function (value, element, param) {
    //    return this.optional(element) || value != $(param).val();
    //}, "This has to be different...");
    jQuery.validator.addMethod("notEqualTo", function (value, element, param) {
        return this.optional(element) || value != $(param).val();
    }, "New password cannot be equal to old password");

    $("form[name='changePasswordForm']").validate({
        rules: {
            oldPassword: {
                required: true,               
            },
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
                pwcheck: true,
                notEqualTo: "#txtOldPassword"
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
                required: GetLocalizedString("RequiredField"),
                minlength: GetLocalizedString("MinimumLength8"),
                maxlength: GetLocalizedString("MaximumLength20"),
                digits: GetLocalizedString("CheckDigits")
            },

            oldPassword: {
                required: GetLocalizedString("RequiredField"),
                minlength: GetLocalizedString("MinimumLength8"),
                maxlength: GetLocalizedString("MaximumLength20"),
                pwcheck: GetLocalizedString("PasswordCheck"),
                digits: GetLocalizedString("CheckDigits")
            },
            cfmPassword: {
                required: GetLocalizedString("RequiredField"),
                minlength: GetLocalizedString("MinimumLength8"),
                maxlength: GetLocalizedString("MaximumLength20"),
                equalTo: GetLocalizedString("PasswordCheck"),
                digits: GetLocalizedString("CheckDigits")
            }
        }
    });
});