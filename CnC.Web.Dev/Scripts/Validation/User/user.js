    // Initialize form validation on the registration form.
    // It has the name attribute "registration"
$(function () {

    $("form[name='myform']").validate({
        // Specify validation rules
           rules: {
            FirstName: "required",
            LastName: "required",
            Username: "required",
            Password:{
                required: true,
                minlength: 8
            },
            Email: "required",
            ddlRoles: "required"       
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