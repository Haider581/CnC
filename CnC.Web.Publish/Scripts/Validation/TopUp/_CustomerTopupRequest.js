// Wait for the DOM to be ready
$(document).ready(function () {
    //var minAmount = $("#lblTopupGrandTotalIr").text();
    // Initialized form validation on the NewKYCFormInCustomerLanguage form.
    // It has the name attribute "NewKYCFormInCustomerLanguage"
    //$('#lblTopupGrandTotalIr').on('DOMSubtreeModified', function () {
    //     minAmount = $("#lblTopupGrandTotalIr").text();
    //});

    //$("#txtDeliveryDateFrom").datepicker({
    //    dateFormat: 'yy/mm/dd', onSelect: function (selected) {
    //        $("#txtDeliveryDateTo").datepicker("option", "minDate", selected)
    //    }
    //});
    //$("#txtCreatedDateTo").datepicker({
    //    dateFormat: 'yy/mm/dd', onSelect: function (selected) {
    //        $("#txtCreatedDateFrom").datepicker("option", "maxDate", selected)
    //    }
    //});
    //$("#txtCreatedDateFrom").datepicker({
    //    dateFormat: 'yy/mm/dd', onSelect: function (selected) {
    //        $("#txtCreatedDateTo").datepicker("option", "minDate", selected)
    //    }
    //});
    //$("#txtDeliveryDateTo").datepicker({
    //    dateFormat: 'yy/mm/dd', onSelect: function (selected) {
    //        $("#txtDeliveryDateTo").datepicker("option", "maxDate", selected)
    //    }
    //});

    //$("form[name='SearchTopUpHistory']").submit(function () {
    //    // storing the value in local storage on submit
    //    //window.localStorage['dllCardType_val'] = $("select[name = 'dllCardType']").val();
    //    //window.localStorage['RequestFormStatus_val'] = $("select[name = 'RequestFormStatus']").val();
    //    //window.localStorage['txtCardNo_val'] = $("input[name = 'txtCardNo']").val();
    //    //window.localStorage['txtNICNo_val'] = $("input[name = 'txtNICNo']").val();
    //    window.localStorage['txtCreatedDateFrom_val'] = $("input[name = 'txtCreatedDateFrom']").val();
    //    window.localStorage['txtCreatedDateTo_val'] = $("input[name = 'txtCreatedDateTo']").val();
    //    //window.localStorage['txtDeliveryDateFrom_val'] = $("input[name = 'txtDeliveryDateFrom']").val();
    //    //window.localStorage['txtDeliveryDateTo_val'] = $("input[name = 'txtDeliveryDateTo']").val();

    //});
    //$(window).load(function () {
    //    // retrieveing the values from local storage after submission
    //    //$("select[name = 'dllCardType']").val(window.localStorage['dllCardType_val']);
    //    //window.localStorage['dllCardType_val'] = '';
    //    //$("select[name = 'RequestFormStatus']").val(window.localStorage['RequestFormStatus_val']);
    //    //window.localStorage['RequestFormStatus_val'] = '';
    //    //$("input[name = 'txtCardNo']").val(window.localStorage['txtCardNo_val']);
    //    //window.localStorage['txtCardNo_val'] = '';
    //    //$("input[name = 'txtNICNo']").val(window.localStorage['txtNICNo_val']);
    //    //window.localStorage['txtNICNo_val'] = '';
    //    $("input[name = 'txtCreatedDateFrom']").val(window.localStorage['txtCreatedDateFrom_val']);
    //    window.localStorage['txtCreatedDateFrom_val'] = '';
    //    $("input[name = 'txtCreatedDateTo']").val(window.localStorage['txtCreatedDateTo_val']);
    //    window.localStorage['txtCreatedDateTo_val'] = '';
    //    //$("input[name = 'txtDeliveryDateTo']").val(window.localStorage['txtDeliveryDateTo_val']);
    //    //window.localStorage['txtDeliveryDateTo_val'] = '';
    //});

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
            //txtNICNo: {
            //    minlength: 10,
            //    maxlength: 10,
            //    nowhitespace: true,
            //    digits: true,
            //},

        }
    });

    $("form[name='newTopupRequestForm']").validate({

        // Specify validation rules
        //errorLabelContainer: "#messageBox",
        //wrapper: 'div',
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined on the right side
            txtTopupAmountEUBlack: {
                //required: true,
                //range: [1000, 10000],
                digits: true,
                //minlength: 4,
                //maxlength: 5,
            },
            txtTopupAmountEUPlatinum: {
                //required: true,
                //range: [100, 1000],
                digits: true,
                //minlength: 3,
                //maxlength: 4,
            },
            txtTopupTransactionNo: {
                required: true,
                minlength: 8,
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
                minlength: 4,
                maxlength: 40,
            },
            txtTopupAmount: {
                required: true,
                minlength: 3,
                maxlength: 10,
                digits: true,
                //: [minAmount, Infinity]

            },
            //Specify validation error messages
            messages: {
                Email: "Please enter a valid email address!",
                digits: "Please enter only digits!",
                date: "Please enter a valid date!",
                required: "This field is required!",
                //range: "ReloadLimit(€):50000.00, Max Reload(€):10000.00, Min Reload(€):1000.00",

            },
            //txtTopupAmountEUBlack: {
            //    range: "ReloadLimit(€):50000.00, Max Reload(€):10000.00, Min Reload(€):1000.00",
            //},
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                //alert("form is valid");
                form.submit();
            }
        }
    });
    //$('.TopupAmountEU').each(function () {
    //    var form = $('.validatform');
    //    form.validate();
    //    if (!form.valid()) {
    //        return false;
    //    };
    //});

});