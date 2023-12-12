$(document).ready(function (e) {


    $("form[name='CustomerInfo']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined on the right side
            txtFailure: {
                required: true,
            },

            // Specify validation error messages
            messages: {
                required: "This field is required!",

            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                //alert("form is valid");
                form.submit();
            }
        }
    });

});