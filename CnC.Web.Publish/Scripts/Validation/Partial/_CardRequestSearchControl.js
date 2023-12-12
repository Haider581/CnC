$(document).ready(function () {
    $("#txtCreatedDateFrom").datepicker({
        dateFormat: 'yy/mm/dd', onSelect: function (selected) {
            $("#txtCreatedDateTo").datepicker("option", "minDate", selected)
        }
    });
    $("#txtCreatedDateTo").datepicker({
        dateFormat: 'yy/mm/dd', onSelect: function (selected) {
            $("#txtCreatedDateFrom").datepicker("option", "maxDate", selected)
        }
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
    $("form[name='SearchCardRequests']").validate({
        rules: {
            txtNICNo: {
                minlength: 10,
                maxlength: 10,
                nowhitespace: true,
                digits: true,
            },
            txtPassportNo: {
                maxlength: 9,
                minlength: 9,
                nowhitespace: true,
                PassportNo: true
            },
            txtEmail: {
                email: true,
                maxlength: 50,
            },
            submitHandler: function (form) {
                form.submit();
            }
        }
    });
    $("form[name='SearchPayments']").validate({
        rules: {
            txtNICNo: {
                minlength: 10,
                maxlength: 10,
                nowhitespace: true,
                digits: true,
            },
            txtPassportNo: {
                minlength: 9,
                maxlength: 9,                
                nowhitespace: true,
                PassportNo: true
            },
            txtEmail: {
                email: true,
                maxlength: 50,
            },
            submitHandler: function (form) {
                form.submit();
            }
        }
    });
    $("form[name='SearchCustomers']").validate({
        rules: {
            txtNICNo: {
                minlength: 10,
                maxlength: 10,
                nowhitespace: true,
                digits: true,
            },
            txtPassportNo: {
                minlength: 9,
                maxlength: 9,                
                nowhitespace: true,
                PassportNo: true
            },
            txtEmail: {
                email: true,
                maxlength: 50,
            },
            submitHandler: function (form) {
                form.submit();
            }
        }
    });
});