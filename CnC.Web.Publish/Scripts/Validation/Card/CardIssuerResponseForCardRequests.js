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
            },
            txtCardTitle: {
                required: true,
            },
            txtCardNo: {
                digits: true,
                required: true,
            },
            txtExpiryDate: {
                date: true,
                required: true,
            },
            // Specify validation error messages
            messages: {
                Email: "Please enter a valid email address",
                digits: "Please enter only digits.",
                date: "Please enter a valid date.",
                required: "This field is required.",
            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                form.submit();
            }
        }
    });

    $("form[name='CardInfo']").validate({
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

            submitHandler: function (form) {
                form.submit();
            }
        }
    });
});