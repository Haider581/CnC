
$(document).ready(function () {
    $("#txtDeliveryDateFrom").datepicker({
        dateFormat: 'yy/mm/dd', onSelect: function (selected) {
            $("#txtDeliveryDateTo").datepicker("option", "minDate", selected)
        }
    });      
    $("#txtDeliveryDateTo").datepicker({
        dateFormat: 'yy/mm/dd', onSelect: function (selected) {
            $("#txtDeliveryDateTo").datepicker("option", "maxDate", selected)
        }
    });
    //$("form[name='SearchCustomers']").submit(function () {
    //    // storing the value in local storage on submit
    //    window.localStorage['CustomerStatus_val'] = $("select[name = 'CustomerStatus']").val();
    //    window.localStorage['txtNICNo_val'] = $("input[name = 'txtNICNo']").val();
    //    window.localStorage['txtPassportNo_val'] = $("input[name = 'txtPassportNo']").val();

    //});
    //$(window).load(function () {
    //    // retrieveing the values from local storage after submission
    //    $("select[name = 'CustomerStatus']").val(window.localStorage['CustomerStatus_val']);
    //    window.localStorage['CustomerStatus_val'] = '';
    //    $("input[name = 'txtNICNo']").val(window.localStorage['txtNICNo_val']);
    //    window.localStorage['txtNICNo_val'] = '';
    //    $("input[name = 'txtPassportNo']").val(window.localStorage['txtPassportNo_val']);
    //    window.localStorage['txtPassportNo_val'] = '';
    //});

    //$("form[name='SearchCardRequests']").submit(function () {
    //    // storing the value in local storage on submit
    //    window.localStorage['txtEmail_val'] = $("input[name = 'txtEmail']").val();
    //    window.localStorage['txtNICNo_val1'] = $("input[name = 'txtNICNo']").val();
    //    window.localStorage['txtCreatedDateFrom_val'] = $("input[name = 'txtCreatedDateFrom']").val();
    //    window.localStorage['txtCreatedDateTo_val'] = $("input[name = 'txtCreatedDateTo']").val();
    //    window.localStorage['txtDeliveryDateFrom_val'] = $("input[name = 'txtDeliveryDateFrom']").val();
    //    window.localStorage['txtDeliveryDateTo_val'] = $("input[name = 'txtDeliveryDateTo']").val();
    //    window.localStorage['RequestFormStatus_val'] = $("select[name = 'RequestFormStatus']").val();
    //    window.localStorage['dllCardType_val'] = $("select[name = 'dllCardType']").val();
    //    //window.localStorage['txtPassportNo_val'] = $("input[name = 'txtPassportNo']").val();

    //});
    //$(window).load(function () {
    //    // retrieveing the values from local storage after submission
    //    $("input[name = 'txtCreatedDateFrom']").val(window.localStorage['txtCreatedDateFrom_val']);
    //    window.localStorage['txtCreatedDateFrom_val'] = '';
    //    $("input[name = 'txtCreatedDateTo']").val(window.localStorage['txtCreatedDateTo_val']);
    //    window.localStorage['txtCreatedDateTo_val'] = '';
    //    $("input[name = 'txtDeliveryDateFrom']").val(window.localStorage['txtDeliveryDateFrom_val']);
    //    window.localStorage['txtDeliveryDateFrom_val'] = '';
    //    $("input[name = 'txtDeliveryDateTo']").val(window.localStorage['txtDeliveryDateTo_val']);
    //    window.localStorage['txtDeliveryDateTo_val'] = '';
    //    $("input[name = 'txtNICNo']").val(window.localStorage['txtNICNo_val1']);
    //    window.localStorage['txtNICNo_val1'] = '';
    //    $("input[name = 'txtEmail']").val(window.localStorage['txtEmail_val']);
    //    window.localStorage['txtEmail_val'] = '';
    //    $("select[name = 'RequestFormStatus']").val(window.localStorage['RequestFormStatus_val']);
    //    window.localStorage['RequestFormStatus_val'] = '';
    //    $("select[name = 'dllCardType']").val(window.localStorage['dllCardType_val']);
    //    window.localStorage['dllCardType_val'] = '';
    //    //$("input[name = 'txtPassportNo']").val(window.localStorage['txtPassportNo_val']);
    //    //window.localStorage['txtPassportNo_val'] = '';
    //});




    $("#btnSearch").click(function () {
        $("#myform").validate();

        if (!$("#myform").valid()) {
            return;
        }

        // var cardNo = $("#CardNo").val();
        var nic = $("#NIC").val();
        var securityQuestion = $("#securityQuestion").val();
        var ans = $("#Answer").val();

        $.ajax({
            url: '/Customer/GetCustomer/',// '@Url.Action("GetCustomer", "Customer")',
            contentType: "application/json; charset=utf-8",
            data: { NIC: nic, SecurityQuestion: securityQuestion, Answer: ans },
            dataType: "json",
            success: function (data) {
                if (data.message != null) {
                    $("#tblCustomer").hide();
                    $("#divMsg").remove();
                    $("#divTable").append(
                       '<div id="divMsg" class="alert alert-danger" role="alert"><strong>Search! </strong>' + data.message + '</div>'
                        );
                }
                else {
                    var tblCustomer = $("#tblCustomer");
                    $("#tblCustomer tr:gt(0)").remove();

                    var tr = $("<tr></tr>");
                    tr.html(
                        ("<td>" + data[0] + "</td>")
                    + " " + ("<td>" + data[1] + "</td>")
                    + " " + ("<td>" + data[2] + "</td>")
                    + " " + ("<td><a href='CustomerDetails?Id=" + data[3] + "'>View Details</a></td>")
                 );
                    tblCustomer.append(tr);
                    $("#divMsg").remove();
                    $("#tblCustomer").show();
                }
            },
            error: function (data) {

            }
        });
    });

    $("form[name='CardInfo']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined on the right side

            txtNICNo: {
                minlength: 10,
                maxlength: 10,
                nowhitespace: true,
                digits: true,
            },
            txtEmail: {
                email: true,
                maxlength: 50,
            },
            submitHandler: function (form) {
                //alert("form is valid");
                form.submit();
            }
        }
    });

});
// Wait for the DOM to be ready
$(function () {
    // Set Card No format

    // Set NIC format
    $('#NIC').formatter({
        'pattern': '{{999}}-{{999999}}-{{9}}'
    });


    // Initialize form validation on the registration form.
    // It has the name attribute "registration"
    $("form[name='myform']").validate({
        // Specify validation rules
        rules: {
            NIC: "required",
            securityQuestion: "required",
            Answer: "required"
        },


        // Specify validation error messages
        messages: {

        },

        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        //submitHandler: function (form) {
        //    form.submit();
        //}
    });
});