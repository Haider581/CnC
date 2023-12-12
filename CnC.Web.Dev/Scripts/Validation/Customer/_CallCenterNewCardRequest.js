// Wait for the DOM to be ready
$(function () {
    // Initialize form validation on the registration form.
    // It has the name attribute "registration"
    $("form[name='newCardRequestForm']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            txtTransactionNo: "required",
            txtAccountNo: "required",
            txtName: "required",
            txtAmount: "required",

            // Specify validation error messages
            messages: {
                password: {
                    required: GetLocalizedString("RequiredField"),
                },

                oldPassword: {
                    required: GetLocalizedString("RequiredField"),
                },
                cfmPassword: {
                    required: GetLocalizedString("RequiredField"),
                }
            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                form.submit();
            }
        }
    });
});