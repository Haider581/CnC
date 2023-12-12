
// Wait for the DOM to be ready
$(document).ready(function () {
    jQuery.validator.addMethod("nowhitespace", function (value, element) {
        return this.optional(element) || /^\S*$/.test(value);
    }, 'Can not contain any spaces.');
    jQuery.validator.addMethod("TotalFee", function (value, element) {
        var TotalFeeIR = parseFloat($("#hidTotalFeeIR").text());
        var TopupAmount = parseFloat($("#txtCardPaidAmount").val());  
        if (TopupAmount < TotalFeeIR) {             
            $("#errorTopupAmount").show();           
            return false;;
        } else {
            $("#errorTopupAmount").hide();
            return true;
        }        
    }, 'Amount Limit: Amount can not be less than Total Fee To Pay');
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
    jQuery.validator.addMethod("pwcheck", function (value) {
        return /^[A-Za-z0-9\d=!/[\@\#\$\%\^\&\*\(\)\_\+\!]/.test(value) // consists of only these
            && /[a-z]/.test(value) // has a lowercase letter 
            && /[A-Z]/.test(value)
            && /[\@\#\$\%\^\&\*\(\)\_\+\!]/.test(value)
            && /\d/.test(value) // has a digit
    }, "choose strong password: At-least one lower-case character, At-least one upper-case character, At-least one digit, At-least one special characters from allowed list, Allowed Characters: A-Z, a-z, 0-9, @ # $ % ^ & * () _ + !");
    
    $('#btnOnlinePaymentId').click(function () {
        var form = $("#NewCustomerCardRequest");
         //form.resetForm();
        $("#hfsubmitActionId").val("Online");      
        var a;
        $('.ddlCheckCardQty').each(function () {
            if ($(this).val() > 0) {
                a = true;
            }
        });
        if (!a) {
            window.scrollTo(0, 0);
            $("#divMsg").show();
            return false;
        }
        else {
            $("#divMsg").hide();
        }
         $("form[name='NewCustomerCardRequests']").validate({
            // Specify validation rules
            rules: {
                // The key name on the left side is the name attribute
                // of an input field. Validation rules are defined on the right side
                txtEngFirstName: {
                    required: true,
                    maxlength: 50,
                    alpha: true
                },
                txtPerFirstName: {
                    required: true,
                    maxlength: 50,
                    //alpha: true
                },
                txtEngLastName: {
                    required: true,
                    maxlength: 50,
                    alpha: true
                },
                txtPerLastName: {
                    required: true,
                    maxlength: 50,
                    //alpha: true
                },
                txtEngDateOfBirth: {
                    required: true,
                    date: true,
                    validDate: true,
                    minlength: 10,
                    maxlength: 10,
                },
                txtPerDateOfBirth: {
                    required: true,
                    //date: true
                    maxlength: 50,
                },
                txtEngAddress: {
                    required: true,
                    minlength: 10,
                    maxlength: 500,
                },
                txtPerAddress: {
                    required: true,
                    minlength: 10,
                    maxlength: 500,
                },
                txtEngAddress2: {
                    required: false,
                },
                txtPerAddress2: {
                    required: false,
                },
                txtPassword: {
                    required: true,
                    minlength: 8,
                    maxlength: 20,
                    pwcheck: true
                },
                txtRePassword: {
                    required: true,
                    equalTo: "#txtPassword",
                    minlength: 8,
                    maxlength: 20,
                    pwcheck: true
                },
                ddlGender: {
                    required: true
                },
                ddlMaritalStatus: {
                    required: true
                },
                txtNIC: {
                    required: true,
                    minlength: 10,
                    maxlength: 10,
                    nowhitespace: true,
                    digits: true
                },
                txtPassportNo: {
                    required: true,
                    minlength: 9,
                    maxlength: 9,
                    nowhitespace: true,
                    PassportNo: true
                },
                txtNationality: {
                    required: true,
                    maxlength: 100
                },
                txtPostalCode: {
                    required: true,
                    minlength: 5,
                    maxlength: 10,
                    digits: true
                },
                ddlCountry: {
                    required: true
                },
                ddlCity: {
                    required: true
                },
                txtEmail: {
                    required: true,
                    email: true,
                    maxlength: 50
                },
                txtContactNo: {
                    required: true,
                    maxlength: 50,
                    digits: true
                },
                CardQty: {
                    required: true
                },
                CustomerSignedCopy: {
                    required: true
                },
                NICDoc: {
                    required: true
                },
                ProofOfAddressDoc: {
                    required: true
                },
                PassportDoc: {
                    required: true
                },
                ProofOfSourceOfFunds: {
                    required: true
                },
                NICDocCustomer: {
                    required: true
                },
                txtOtherSourceofFunds: {
                    required: true
                },
                proofOfAddressDocCutomer: {
                    required: true
                },
                proofOfAddressType: {
                    valueNotEquals: "default"
                },
                sourcceOfFundType: {
                    valueNotEquals: "default"
                },
                PassportDocCustomer: {
                    required: true
                },
                "proofOfSourceOfFundsCl[0]": {
                    required: true,
                },
                "proofOfAddressDocCl[0]": {
                    required: true
                },
                ProofOfSourceOfFundsCustomer: {
                    required: true
                },
                customerSignedCopyCl: {
                    required: true
                },
                customerSignedCopyEng: {
                    required: true
                },
                txtCardTransactionNo: {
                    required: true,
                    minlength: 4,
                    maxlength: 28,
                    digits: true,
                },
                txtCardAccountNo: {
                    required: true,
                    minlength: 5,
                    maxlength: 20,
                    digits: true,
                },
                txtCardReqCustName: {
                    required: true,
                    minlength: 4,
                    alpha: true,
                    maxlength: 40,
                },
                txtCardPaidAmount: {
                    required: true,
                    minlength: 3,
                    maxlength: 13,
                    digits: true,
                    TotalFee: true
                    //: [minAmount, Infinity]

                },
                CaptchaCode: {
                    required: true,
                },
                // Specify validation error messages
            },
            messages: {
                Email: "Please enter a valid email address!",
                digits: "Please enter only digits!",
                date: "Please enter a valid date!",
                required: GetLocalizedString("RequiredField"),
                txtCardPaidAmount: "",
                proofOfAddressType: {
                    valueNotEquals: GetLocalizedString("RequiredField")
                },
                sourcceOfFundType: {
                    valueNotEquals: GetLocalizedString("RequiredField")
                },
                "proofOfAddressDocCl[0]": {
                    required: GetLocalizedString("RequiredField")
                },
                "proofOfSourceOfFundsCl[0]": {
                    required: GetLocalizedString("RequiredField")
                },
               
            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                
                form.submit();
            }
         }).form();
         if (!form.valid()) {
             //alert("YES");
             return false;
         };

    });
    $('#btnSaveInfoStep3').click(function () {
        var form = $("#NewCustomerCardRequest");
        $("#hfsubmitActionId").val("Bank");
        var a;
        $('.ddlCheckCardQty').each(function () {
            if ($(this).val() > 0) {
                a = true;
            }
        });
        if (!a) {
            window.scrollTo(0, 0);
            $("#divMsg").show();
            return false;
        }
        else {
            $("#divMsg").hide();
        }   
        $("form[name='NewCustomerCardRequests']").validate({          
            rules: {             
                txtEngFirstName: {
                    required: true,
                    maxlength: 50,
                    alpha: true
                },
                txtPerFirstName: {
                    required: true,
                    maxlength: 50,
                    //alpha: true
                },
                txtEngLastName: {
                    required: true,
                    maxlength: 50,
                    alpha: true
                },
                txtPerLastName: {
                    required: true,
                    maxlength: 50,
                    //alpha: true
                },
                txtEngDateOfBirth: {
                    required: true,
                    date: true,
                    validDate: true,
                    minlength: 10,
                    maxlength: 10,
                },
                txtPerDateOfBirth: {
                    required: true,
                    //date: true
                    maxlength: 50,
                },
                txtEngAddress: {
                    required: true,
                    minlength: 10,
                    maxlength: 500,
                },
                txtPerAddress: {
                    required: true,
                    minlength: 10,
                    maxlength: 500,
                },
                txtEngAddress2: {
                    required: false,
                },
                txtPerAddress2: {
                    required: false,
                },
                txtPassword: {
                    required: true,
                    minlength: 8,
                    maxlength: 20,
                    pwcheck: true
                },
                txtRePassword: {
                    required: true,
                    equalTo: "#txtPassword",
                    minlength: 8,
                    maxlength: 20,
                    pwcheck: true
                },
                ddlGender: {
                    required: true
                },
                ddlMaritalStatus: {
                    required: true
                },
                txtNIC: {
                    required: true,
                    minlength: 10,
                    maxlength: 10,
                    nowhitespace: true,
                    digits: true
                },
                txtPassportNo: {
                    required: true,
                    minlength: 9,
                    maxlength: 9,
                    nowhitespace: true,
                    PassportNo: true
                },
                txtNationality: {
                    required: true,
                    maxlength: 100
                },
                txtPostalCode: {
                    required: true,
                    minlength: 5,
                    maxlength: 10,
                    digits: true
                },
                ddlCountry: {
                    required: true
                },
                ddlCity: {
                    required: true
                },
                txtEmail: {
                    required: true,
                    email: true,
                    maxlength: 50
                },
                txtContactNo: {
                    required: true,
                    maxlength: 50,
                    digits: true
                },
                CardQty: {
                    required: true
                },
                CustomerSignedCopy: {
                    required: true
                },
                NICDoc: {
                    required: true
                },
                ProofOfAddressDoc: {
                    required: true
                },
                PassportDoc: {
                    required: true
                },
                ProofOfSourceOfFunds: {
                    required: true
                },
                NICDocCustomer: {
                    required: true
                },
                txtOtherSourceofFunds: {
                    required: true
                },
                proofOfAddressDocCutomer: {
                    required: true
                },
                proofOfAddressType: {
                    valueNotEquals: "default"
                },
                sourcceOfFundType: {
                    valueNotEquals: "default"
                },
                PassportDocCustomer: {
                    required: true
                },
                "proofOfSourceOfFundsCl[0]": {
                    required: true,
                },
                "proofOfAddressDocCl[0]": {
                    required: true
                },
                ProofOfSourceOfFundsCustomer: {
                    required: true
                },
                customerSignedCopyCl: {
                    required: true
                },
                customerSignedCopyEng: {
                    required: true
                },
                txtCardTransactionNo: {
                    required: true,
                    minlength: 4,
                    maxlength: 28,
                    digits: true,
                },
                txtCardAccountNo: {
                    required: true,
                    minlength: 5,
                    maxlength: 20,
                    digits: true,
                },
                txtCardReqCustName: {
                    required: true,
                    minlength: 4,
                    alpha: true,
                    maxlength: 40,
                },
                txtCardPaidAmount: {
                    required: true,
                    minlength: 3,
                    maxlength: 13,
                    digits: true,
                    TotalFee: true
                    //: [minAmount, Infinity]

                },
                CaptchaCode: {
                    required: true,
                },
                // Specify validation error messages
            },
            messages: {
                Email: "Please enter a valid email address!",
                digits: "Please enter only digits!",
                date: "Please enter a valid date!",
                required: GetLocalizedString("RequiredField"),
                txtCardPaidAmount: "",
                //proofOfAddressType: {
                //    valueNotEquals: GetLocalizedString("RequiredField")
                //},
                //sourcceOfFundType: {
                //    valueNotEquals: GetLocalizedString("RequiredField")
                //},
                //"proofOfAddressDocCl[0]": {
                //    required: GetLocalizedString("RequiredField")
                //},
                //"proofOfSourceOfFundsCl[0]": {
                //    required: GetLocalizedString("RequiredField")
                //},
                //txtCardTransactionNo: {
                //    required: GetLocalizedString("RequiredField"),
                //    digits: GetLocalizedString("OnlyDigits"),
                //    minlength: GetLocalizedString("MinimumLength8"),
                //    maxlength: GetLocalizedString("MaximumLength28"),
                //},
                //txtCardAccountNo: {
                //    required: GetLocalizedString("RequiredField"),
                //    digits: GetLocalizedString("OnlyDigits"),
                //    minlength: GetLocalizedString("MinimumLength5"),
                //    maxlength: GetLocalizedString("MaximumLength20"),
                //},
                //txtCardReqCustName: {
                //    required: GetLocalizedString("RequiredField"),
                //    minlength: GetLocalizedString("MinimumLength4"),
                //    maxlength: GetLocalizedString("MaximumLength40"),
                //    alpha:GetLocalizedString("AlphabetsOnly")
                //}
            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            //submitHandler: function (form) {
            //    //alert("form is valid" + txtCard);
            //    InstructionDialog();
            //    form.submit();
            //}
        }).form();
       
        if (form.valid()) {
            //alert("YES");
            InstructionDialog();
        }
    else{return false;}

    });
    $('#btnSaveInfoStep2').click(function () {
        var form = $("#NewCustomerCardRequest");
        $("#hfsubmitActionId").val("Bank");
        var a;
        $('.ddlCheckCardQty').each(function () {
            if ($(this).val() > 0) {
                a = true;
            }
        });
        if (!a) {
            window.scrollTo(0, 0);
            $("#divMsg").show();
            return false;
        }
        else {
            $("#divMsg").hide();
        }
        //$("#form").validate({
        //    rules: {
        //        phone: {
        //            required: true,
        //            number: true,
        //            rangelength: [7, 14]
        //        }
        //    }
        //}).form();
        $("form[name='NewCustomerCardRequests']").validate({
            // Specify validation rules
            rules: {
                // The key name on the left side is the name attribute
                // of an input field. Validation rules are defined on the right side
                txtEngFirstName: {
                    required: true,
                    maxlength: 50,
                    alpha: true
                },
                txtPerFirstName: {
                    required: true,
                    maxlength: 50,
                    //alpha: true
                },
                txtEngLastName: {
                    required: true,
                    maxlength: 50,
                    alpha: true
                },
                txtPerLastName: {
                    required: true,
                    maxlength: 50,
                    //alpha: true
                },
                txtEngDateOfBirth: {
                    required: true,
                    date: true,
                    validDate: true,
                    minlength: 10,
                    maxlength: 10,
                },
                txtPerDateOfBirth: {
                    required: true,
                    //date: true
                    maxlength: 50,
                },
                txtEngAddress: {
                    required: true,
                    minlength: 10,
                    maxlength: 500,
                },
                txtPerAddress: {
                    required: true,
                    minlength: 10,
                    maxlength: 500,
                },
                txtEngAddress2: {
                    required: false,
                },
                txtPerAddress2: {
                    required: false,
                },
                txtPassword: {
                    required: true,
                    minlength: 8,
                    maxlength: 20,
                    pwcheck: true
                },
                txtRePassword: {
                    required: true,
                    equalTo: "#txtPassword",
                    minlength: 8,
                    maxlength: 20,
                    pwcheck: true
                },
                ddlGender: {
                    required: true
                },
                ddlMaritalStatus: {
                    required: true
                },
                txtNIC: {
                    required: true,
                    minlength: 10,
                    maxlength: 10,
                    nowhitespace: true,
                    digits: true
                },
                txtPassportNo: {
                    required: true,
                    minlength: 9,
                    maxlength: 9,
                    nowhitespace: true,
                    PassportNo: true
                },
                txtNationality: {
                    required: true,
                    maxlength: 100
                },
                txtPostalCode: {
                    required: true,
                    minlength: 5,
                    maxlength: 10,
                    digits: true
                },
                ddlCountry: {
                    required: true
                },
                ddlCity: {
                    required: true
                },
                txtEmail: {
                    required: true,
                    email: true,
                    maxlength: 50
                },
                txtContactNo: {
                    required: true,
                    maxlength: 50,
                    digits: true
                },
                CardQty: {
                    required: true
                },
                CustomerSignedCopy: {
                    required: true
                },
                NICDoc: {
                    required: true
                },
                ProofOfAddressDoc: {
                    required: true
                },
                PassportDoc: {
                    required: true
                },
                ProofOfSourceOfFunds: {
                    required: true
                },
                NICDocCustomer: {
                    required: true
                },
                txtOtherSourceofFunds: {
                    required: true
                },
                proofOfAddressDocCutomer: {
                    required: true
                },
                proofOfAddressType: {
                    valueNotEquals: "default"
                },
                sourcceOfFundType: {
                    valueNotEquals: "default"
                },
                PassportDocCustomer: {
                    required: true
                },
                "proofOfSourceOfFundsCl[0]": {
                    required: true,
                },
                "proofOfAddressDocCl[0]": {
                    required: true
                },
                ProofOfSourceOfFundsCustomer: {
                    required: true
                },
                customerSignedCopyCl: {
                    required: true
                },
                customerSignedCopyEng: {
                    required: true
                },
                txtCardTransactionNo: {
                    required: true,
                    minlength: 4,
                    maxlength: 28,
                    digits: true,
                },
                txtCardAccountNo: {
                    required: true,
                    minlength: 5,
                    maxlength: 20,
                    digits: true,
                },
                txtCardReqCustName: {
                    required: true,
                    minlength: 4,
                    alpha: true,
                    maxlength: 40,
                },
                txtCardPaidAmount: {
                    required: true,
                    minlength: 3,
                    maxlength: 13,
                    digits: true,
                    TotalFee: true
                    //: [minAmount, Infinity]

                },
                CaptchaCode: {
                    required: true,
                },
                // Specify validation error messages
            },
            messages: {
                Email: "Please enter a valid email address!",
                digits: "Please enter only digits!",
                date: "Please enter a valid date!",
                required: GetLocalizedString("RequiredField"),
                txtCardPaidAmount: "",
                proofOfAddressType: {
                    valueNotEquals: GetLocalizedString("RequiredField")
                },
                sourcceOfFundType: {
                    valueNotEquals: GetLocalizedString("RequiredField")
                },
                "proofOfAddressDocCl[0]": {
                    required: GetLocalizedString("RequiredField")
                },
                "proofOfSourceOfFundsCl[0]": {
                    required: GetLocalizedString("RequiredField")
                },
               
            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                //alert("form is valid" + txtCard);
                //InstructionDialog();
                form.submit();
            }
        }).form();
        if (!form.valid()) {
            //alert("YES");
            return false;
        };

    });

    function InstructionDialog() {
        $('#dialog_instructions').dialog('open');
        return false;
    }


});
(function ($) {
    $.fn.checkFileType = function (options) {
        var defaults = {
            allowedExtensions: [],
            success: function () { },
            error: function () { },
        };
        options = $.extend(defaults, options);
        return this.each(function () {
            $(this).on('change', function () {
                //var filesize = 0;
                var value = $(this).val(),
                    file = value.toLowerCase(),
                    extension = file.substring(file.lastIndexOf('.') + 1);
                var filesize = (this.files.length && this.files[0].size) || '';
                if ($.inArray(extension, options.allowedExtensions) == -1 || filesize > 1048576) {
                    //alert('Please Attach only jpg/png/ format file.');
                    //options.error();
                    $(this).focus();
                    //alert(this.id);
                    this.value = "";
                    if (this.id == "NICDoc") {
                        document.getElementById("forNICDocError").style.display = "";
                    }
                    if (this.id == "ProofOfAddressDoc") {
                        document.getElementById("forProofOfAddressDocError").style.display = "";
                    }
                    if (this.id == "PassportDoc") {
                        document.getElementById("forPassportDocError").style.display = "";
                    }
                    if (this.id == "ProofOfSourceOfFunds") {
                        document.getElementById("forProofOfSourceOfFundsError").style.display = "";
                    }
                    if (this.id == "NICDocCustomer") {
                        document.getElementById("forNICDocCustomerError").style.display = "";
                    }
                    if (this.id == "proofOfAddressDocCutomer") {
                        document.getElementById("forProofOfAddressDocCutomerError").style.display = "";
                        //document.getElementById("proofOfAddressDocCutomer").className += "state-error";
                    }
                    if (this.id == "PassportDocCustomer") {
                        document.getElementById("forPassportDocCustomerError").style.display = "";
                    }
                    if (this.id == "ProofOfSourceOfFundsCustomer") {
                        document.getElementById("forProofOfSourceOfFundsCustomerError").style.display = "";
                    }
                    if (this.id == "proofOfSourceOfFundsCl0") {
                        document.getElementById("forproofOfSourceOfFundsCl0Error").style.display = "";
                    }
                    if (this.id == "proofOfSourceOfFundsCl1") {
                        document.getElementById("forproofOfSourceOfFundsCl1Error").style.display = "";
                    }
                    if (this.id == "proofOfSourceOfFundsCl2") {
                        document.getElementById("forproofOfSourceOfFundsCl2Error").style.display = "";
                    }
                    if (this.id == "proofOfSourceOfFundsCl3") {
                        document.getElementById("forproofOfSourceOfFundsCl3Error").style.display = "";
                    }
                    if (this.id == "proofOfSourceOfFundsCl4") {
                        document.getElementById("forproofOfSourceOfFundsCl4Error").style.display = "";
                    }
                    if (this.id == "customerSignedCopyCl") {
                        document.getElementById("forcustomerSignedCopyClError").style.display = "";
                    }
                    if (this.id == "customerSignedCopyEng") {
                        document.getElementById("forcustomerSignedCopyEngError").style.display = "";
                    }
                    //NewCustomerCardRequests: Proof Of Address, Scanned Copy (Persian)
                    if (this.id == "proofOfAddressDocCl0") {
                        document.getElementById("forproofOfAddressDocCl0Error").style.display = "";
                    }
                    if (this.id == "proofOfAddressDocCl1") {
                        document.getElementById("forproofOfAddressDocCl1Error").style.display = "";
                    }
                    if (this.id == "proofOfAddressDocCl2") {
                        document.getElementById("forproofOfAddressDocCl2Error").style.display = "";
                    }
                    if (this.id == "proofOfAddressDocCl3") {
                        document.getElementById("forproofOfAddressDocCl3Error").style.display = "";
                    }
                    options.error();
                }
                else {
                    if (this.id == "NICDoc") {
                        document.getElementById("forNICDocError").style.display = "none";
                    }
                    if (this.id == "ProofOfAddressDoc") {
                        document.getElementById("forProofOfAddressDocError").style.display = "none";
                    }
                    if (this.id == "PassportDoc") {
                        document.getElementById("forPassportDocError").style.display = "none";
                    }
                    if (this.id == "ProofOfSourceOfFunds") {
                        document.getElementById("forProofOfSourceOfFundsError").style.display = "none";
                    }
                    if (this.id == "NICDocCustomer") {
                        document.getElementById("forNICDocCustomerError").style.display = "none";
                    }
                    if (this.id == "ProofOfAddressDocCutomer") {
                        document.getElementById("forProofOfAddressDocCutomerError").style.display = "none";
                    }
                    if (this.id == "PassportDocCustomer") {
                        document.getElementById("forPassportDocCustomerError").style.display = "none";
                    }
                    if (this.id == "ProofOfSourceOfFundsCustomer") {
                        document.getElementById("forProofOfSourceOfFundsCustomerError").style.display = "none";
                    }
                    if (this.id == "proofOfAddressDocCutomer") {
                        document.getElementById("forProofOfAddressDocCutomerError").style.display = "none";
                    }
                    if (this.id == "proofOfSourceOfFundsCl0") {
                        document.getElementById("forproofOfSourceOfFundsCl0Error").style.display = "none";
                    }
                    if (this.id == "proofOfSourceOfFundsCl1") {
                        document.getElementById("forproofOfSourceOfFundsCl1Error").style.display = "none";
                    }
                    if (this.id == "proofOfSourceOfFundsCl2") {
                        document.getElementById("forproofOfSourceOfFundsCl2Error").style.display = "none";
                    }
                    if (this.id == "proofOfSourceOfFundsCl3") {
                        document.getElementById("forproofOfSourceOfFundsCl3Error").style.display = "none";
                    }
                    if (this.id == "proofOfSourceOfFundsCl4") {
                        document.getElementById("forproofOfSourceOfFundsCl4Error").style.display = "none";
                    }
                    if (this.id == "customerSignedCopyCl") {
                        document.getElementById("forcustomerSignedCopyClError").style.display = "none";
                    }
                    if (this.id == "customerSignedCopyEng") {
                        document.getElementById("forcustomerSignedCopyEngError").style.display = "none";
                    }
                    //NewCustomerCardRequests: Proof Of Address, Scanned Copy (Persian)
                    if (this.id == "proofOfAddressDocCl0") {
                        document.getElementById("forproofOfAddressDocCl0Error").style.display = "none";
                    }
                    if (this.id == "proofOfAddressDocCl1") {
                        document.getElementById("forproofOfAddressDocCl1Error").style.display = "none";
                    }
                    if (this.id == "proofOfAddressDocCl2") {
                        document.getElementById("forproofOfAddressDocCl2Error").style.display = "none";
                    }
                    if (this.id == "proofOfAddressDocCl3") {
                        document.getElementById("forproofOfAddressDocCl3Error").style.display = "none";
                    }
                    options.success();
                }
            });
        });
    };
})(jQuery);
$(function () {
    $('.checkFileType').checkFileType({
        allowedExtensions: ['jpg', 'jpeg', 'png'],
        success: function () {
            //var form = $('.validatform');
            //form.validate();
            //if (!form.valid()) {
            //    return false;
            //};
            //alert('Success');
        },
        error: function () {
            //var form = $('.validatform');
            //form.validate();
            //if (!form.valid()) {
            //    return false;
            //};
            //alert('Only image type jpg/png/jpeg/ is allowed and File size must under 1MB!');
        }
    });
});