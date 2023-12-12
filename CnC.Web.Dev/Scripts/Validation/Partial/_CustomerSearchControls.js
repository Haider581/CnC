$(document).ready(function () {
    $("#txtRegistrationDateFrom").datepicker({
        dateFormat: 'yy/mm/dd', onSelect: function (selected) {
            //alert("yes");
            $("#txtRegistrationDateTo").datepicker("option", "minDate", selected)
        }
    });
    $("#txtRegistrationDateTo").datepicker({
        dateFormat: 'yy/mm/dd', onSelect: function (selected) {
            $("#txtRegistrationDateFrom").datepicker("option", "maxDate", selected)
        }
    });
    $("#txtCreationDate").datepicker({
        dateFormat: 'yy/mm/dd'
});
$('#btnClearSearch').on("click", function (e) {
    $(this).closest('form').find("input[type=text]").val("");
    $(this).closest('form').find("select").val("");
});
jQuery.validator.addMethod("nowhitespace", function (value, element) {
    return this.optional(element) || /^\S*$/.test(value);
}, 'Can not contain any spaces.');
jQuery.validator.addMethod("PassportNo", function (value, element) {
    return this.optional(element) || /^[a-zA-Z][0-9]{8}$/.test(value);
}, 'Passport No. pattern does not match, Correct pattern A12345678');
$("form[name='SearchCustomers']").validate({
    rules: {
        txtPassportNo: {
            maxlength: 9,
            minlength: 9,
            nowhitespace: true,
            PassportNo: true
        },
        txtNICNo: {
            minlength: 10,
            maxlength: 10,
            nowhitespace: true,
            digits: true,
        },
        submitHandler: function (form) {
            form.submit();
        }
    },
    messages: {
        txtPassportNo: {
            minlength: GetLocalizedString("MinimumLength9"),
            maxlength: GetLocalizedString("MaximumLength9"),
        },
        txtNICNo: {
            minlength: GetLocalizedString("MinimumLength10"),
            maxlength: GetLocalizedString("MaximumLength10"),
        }
    }
});
$("form[name='CardRequests']").validate({
    rules: {
        txtPassportNo: {
            maxlength: 9,
            minlength: 9,
            nowhitespace: true,
            PassportNo: true
        },
        txtNICNo: {
            minlength: 10,
            maxlength: 10,
            nowhitespace: true,
            digits: true,
        },
        submitHandler: function (form) {
            form.submit();
        }
    },
    messages: {
        txtPassportNo: {
            minlength: GetLocalizedString("MinimumLength9"),
            maxlength: GetLocalizedString("MaximumLength9"),
        },
        txtNICNo: {
            minlength: GetLocalizedString("MinimumLength10"),
            maxlength: GetLocalizedString("MaximumLength10"),
        }
    }
});
$("form[name='CustomerSignedDocument']").validate({
    rules: {
        txtPassportNo: {
            maxlength: 9,
            minlength: 9,
            nowhitespace: true,
            PassportNo: true
        },
        txtNICNo: {
            minlength: 10,
            maxlength: 10,
            nowhitespace: true,
            digits: true,
        },
        submitHandler: function (form) {
            form.submit();
        }
    },
    messages: {
        txtPassportNo: {
            minlength: GetLocalizedString("MinimumLength9"),
            maxlength: GetLocalizedString("MaximumLength9"),
        },
        txtNICNo: {
            minlength: GetLocalizedString("MinimumLength10"),
            maxlength: GetLocalizedString("MaximumLength10"),
        }
    }
});
});