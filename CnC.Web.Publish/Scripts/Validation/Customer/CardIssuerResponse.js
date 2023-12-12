// Wait for the DOM to be ready
$(document).ready(function () {
    jQuery.validator.addMethod("nowhitespace", function (value, element) {
        return this.optional(element) || /^\S*$/.test(value);
    }, 'Can not contain any spaces.');
    jQuery.validator.addMethod("PassportNo", function (value, element) {
        return this.optional(element) || /^[a-zA-Z][0-9]{8}$/.test(value);
    }, 'Passport No. pattern does not match, Correct pattern A12345678');
    jQuery.validator.addMethod("valueNotEquals", function (value, element, arg) {
        return arg != value;
    }, "Please select an item!");
    jQuery.validator.addMethod("validDate", function (value, element) {
        return this.optional(element) || moment(value, "yy/mm/dd").isValid();
    }, "Please enter a valid date in the format yy/mm/dd");
    jQuery.validator.addMethod("alpha", function (value, element) {
        return this.optional(element) || value == value.match(/^[a-zA-Z\s]+$/);
    }, "Please enter a valid Name, only alphabets and spaces allowed");

    // Initialized form validation on the CardInfoApprove form.
    // It has the name attribute "CardInfoApprove"
    $("form[name='CardInfoApprove']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined on the right side
            txtClientId: {
                digits: true,
                required: true,                
            },
            txtEmailAddress: {
                required: true,
                email: true,
                maxlength: 50,
            },
            txtBillingAddress: {
                required: true,
                minlength: 10,
                maxlength: 500,
            },
            // Specify validation error messages
            messages: {
                Email: "Please enter a valid email address!",
                digits: "Please enter only digits!",
                date: "Please enter a valid date!",
                required: "This field is required!"

            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                //alert("form is valid");
                form.submit();
            }
        }
    });
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