// Wait for the DOM to be ready
$(document).ready(function () {
    jQuery.validator.addMethod("alpha", function (value, element) {
        return this.optional(element) || value == value.match(/^[a-zA-Z\s]+$/);
    }, "Please enter a valid Name, only alphabets and spaces allowed");
    $("form[name='SearchTopUpHistory']").validate({
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
                nowhitespace: true
            },

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
    $('#btnTopupNotify').click(function () {
        $("form[name='newTopupRequestForm']").validate({

            rules: {
                txtTopupAmountEUBlack: {
                    digits: true,
                },
                txtTopupAmountEUPlatinum: {
                    digits: true,
                },
                txtTopupTransactionNo: {
                    required: true,
                    minlength: 4,
                    maxlength: 28,
                    digits: true,
                },
                txtTopupAccountNo: {
                    required: true,
                    minlength: 5,
                    maxlength: 20,
                    digits: true,
                },
                txtTopupName: {
                    required: true,
                    minlength: 3,
                    maxlength: 40,
                    alpha: true
                },
                txtTopupAmount: {
                    required: true,
                    minlength: 2,
                    maxlength: 13,
                    digits: true,
                    //: [minAmount, Infinity]

                }
            },
            //Specify validation error messages
            messages: {
                Email: "Please enter a valid email address!",
                digits: "Please enter only digits!",
                date: "Please enter a valid date!",
                required: "This field is required!",
                //range: "ReloadLimit(€):50000.00, Max Reload(€):10000.00, Min Reload(€):1000.00",
                txtTopupTransactionNo: {
                    required: GetLocalizedString("RequiredField"),
                    digits: GetLocalizedString("OnlyDigits"),
                    minlength: GetLocalizedString("MinimumLength4"),
                    maxlength: GetLocalizedString("MaximumLength28")
                },
                txtTopupAccountNo: {
                    required: GetLocalizedString("RequiredField"),
                    digits: GetLocalizedString("OnlyDigits"),
                    minlength: GetLocalizedString("MinimumLength5"),
                    maxlength: GetLocalizedString("MaximumLength20")
                },
                txtTopupName: {
                    required: GetLocalizedString("RequiredField"),
                    minlength: GetLocalizedString("MinimumLength4"),
                    maxlength: GetLocalizedString("MaximumLength40")
                },
                txtTopupAmount: {
                    required: GetLocalizedString("RequiredField"),
                    digits: GetLocalizedString("OnlyDigits"),
                    minlength: GetLocalizedString("MinimumLength3"),
                    maxlength: GetLocalizedString("MaximumLength13")
                }
            },

            //submitHandler: function (form) {
            //    //alert("form is valid");
            //    form.submit();
            //}

        }).form();
        //$('.TopupAmountEU').each(function () {
        //    var form = $('.validatform');
        //    form.validate();
        //    if (!form.valid()) {
        //        return false;
        //    };
        //});
    });

});