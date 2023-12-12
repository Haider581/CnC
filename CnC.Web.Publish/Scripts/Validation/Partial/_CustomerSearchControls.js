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
        }
    });
});