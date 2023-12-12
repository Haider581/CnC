$(document).ready(function () {

    //$("form[name='SearchTopUpHistory']").submit(function () {
    //    // storing the value in local storage on submit
    //    window.localStorage['dllCardType_val'] = $("select[name = 'dllCardType']").val();
    //});
    //$(window).load(function () {
    //    // retrieveing the values from local storage after submission
    //    $("select[name = 'dllCardType']").val(window.localStorage['dllCardType_val']);
    //    window.localStorage['dllCardType_val'] = '';
    //});

    //$("form[name='SearchTopUpRequestsProcessing']").submit(function () {
    //    // storing the value in local storage on submit
    //    window.localStorage['dllCardType_val1'] = $("select[name = 'dllCardType']").val();
    //});
    //$(window).load(function () {
    //    // retrieveing the values from local storage after submission
    //    $("select[name = 'dllCardType']").val(window.localStorage['dllCardType_val1']);
    //    window.localStorage['dllCardType_val1'] = '';
    //});
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
                minlength: 9,
                maxlength: 9,
                nowhitespace: true,
                PassportNo: true
            },
            submitHandler: function (form) {
                //alert("form is valid");
                form.submit();
            }
        }
    });
    $("form[name='SearchTopUpRequestsProcessing']").validate({
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
            }
        },
        messages: {
            txtNICNo: {
                minlength: GetLocalizedString("MinimumLength10"),
                maxlength: GetLocalizedString("MaximumLength10")
            },
            txtPassportNo: {
                minlength: GetLocalizedString("MinimumLength9"),
                maxlength: GetLocalizedString("MaximumLength9")
            },

        },
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='SearchCards']").validate({
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

            submitHandler: function (form) {
                //alert("form is valid");
                form.submit();
            }
        }
    });
    $("form[name='SearchTopUpRequests']").validate({
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
            submitHandler: function (form) {
                //alert("form is valid");
                form.submit();
            }
        }
    });
    $("form[name='fmPaymentConfirmation']").validate({
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
            submitHandler: function (form) {
                //alert("form is valid");
                form.submit();
            }
        }
    });
    $("form[name='SearchTopupProcess']").validate({
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
            submitHandler: function (form) {
                //alert("form is valid");
                form.submit();
            }
        }
    });
});