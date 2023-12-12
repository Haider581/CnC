// Wait for the DOM to be ready
$(document).ready(function () {
    $("#txtExpiryDate").datepicker({
        minDate: 0,
        dateFormat: 'yy/mm/dd',
        onSelect: function (selected) {
            //$("#txtCreatedDateFrom").datepicker("option", "maxDate", selected)
        }
    });
    $(function () {
        $("#txtExpiryDate").datepicker({ minDate: 0 });
    });
    jQuery.validator.addMethod("declined_reason", function (value, element) {
        return this.optional(element) || value == value.match(/^[a-zA-Z\s]+$/);
    }, "Please enter proper decline reasons, only alphabets and spaces allowed");
    jQuery.validator.addMethod("alpha", function (value, element) {
        return this.optional(element) || value == value.match(/^[a-zA-Z\s]+$/);
    }, "Please enter a valid Card Title, only alphabets and spaces allowed");
    // Initialized form validation on the CardInfo form.
    // It has the name attribute "CardInfo"
    $("form[name='CardInfoApproved']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined on the right side

            txtClientId: {
                digits: true,
                required: true,
            },
            txtCardId: {
                digits: true,
                required: true,
                minlength: 4,
                maxlength: 8
            },
            txtCardTitle: {
                required: true,
                minlength: 4,
                maxlength: 25,
                alpha: true
            },
            txtCardNo: {
                digits: true,
                required: true,
                minlength: 7,
                maxlength: 16
            },
            txtExpiryDate: {
                date: true,
                required: true,
            }
        },
        // Specify validation error messages
        messages: {
            //Email: "Please enter a valid email address",
            //digits: "Please enter only digits.",
            //date: "Please enter a valid date.",
            //required: "This field is required.",
            txtCardId: {
                required: GetLocalizedString("RequiredField"),
                minlength: GetLocalizedString("MinimumLength4"),
                maxlength: GetLocalizedString("MaximumLength8"),
                digits: GetLocalizedString("OnlyDigits")
            },
            txtCardNo: {
                required: GetLocalizedString("RequiredField"),
                minlength: GetLocalizedString("MinimumLength7"),
                maxlength: GetLocalizedString("MaximumLength16"),
                digits: GetLocalizedString("OnlyDigits")
            },
            txtCardTitle: {
                required: GetLocalizedString("RequiredField"),
                minlength: GetLocalizedString("MinimumLength4"),
                maxlength: GetLocalizedString("MaximumLength25"),
            },
            txtExpiryDate: {
                required: GetLocalizedString("RequiredField"),
            }
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }

    });

    $("form[name='CardInfo']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined on the right side
            txtFailure: {
                required: true,
                declined_reason: true,
                minlength: 4,
                maxlength: 200,
            },

            // Specify validation error messages
            messages: {
                required: "This field is required!",

            },

            submitHandler: function (form) {
                form.submit();
            }
        }
    });
});