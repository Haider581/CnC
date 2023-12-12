// Wait for the DOM to be ready
$(document).ready(function () {
    // Initialized form validation on the SearchCardRequests form.
    // It has the name attribute "SearchCardRequests"
    $("form[name='SearchCardRequests']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined on the right side

            txtEmail: {
                // Specify that email should be validated
                // by the built-in "email" rule
                email: true

            },
            txtNICNo: {
                // Specify that email should be validated
                // by the built-in "email" rule
                digits: true

            },
            txtCreatedDateFrom: {
                // Specify that email should be validated
                // by the built-in "email" rule
                date: true

            },
            txtCreatedDateTo: {
                // Specify that email should be validated
                // by the built-in "email" rule
                date: true

            },
            txtDeliveryDateFrom: {
                // Specify that email should be validated
                // by the built-in "email" rule
                date: true

            },
            txtDeliveryDateTo: {
                // Specify that email should be validated
                // by the built-in "email" rule
                date: true

            },
            // Specify validation error messages
            messages: {
                Email: "Please enter a valid email address",
                digits: "Please enter only digits.",
                date: "Please enter a valid date.",
            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                form.submit();
            }
        }
    });
});