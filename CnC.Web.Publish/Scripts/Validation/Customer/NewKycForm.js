// Wait for the DOM to be ready
$(function () {
    // Initialize form validation on the registration form.
    // It has the name attribute "registration"
    $("form[name='kycForm']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            FirstName: "required",
            LastName: "required", 
            CountryDDL: "required",
            CityDDL: "required",
            DateOfBirth: "required",
            Gender:"required",
            MaritalStatus: "required",
            NIC:"required",
            Nationality: "required",
            PassportNo: "required",
            Address: "required",
            PostalCode: "required",
            NICDoc: "required",
            ProofOfAddressDoc: "required",
            PassportDoc: "required",
            ProofOfSourceOfFunds: "required",
            Email: {
                required: true,
                // Specify that email should be validated
                // by the built-in "email" rule
                email: true
                //},
                //password: {
                //    required: true,
                //    minlength: 5
                //}
            },
            // Specify validation error messages
            messages: {
                FirstName: "Please enter your firstname", 
                CountryDDL: "Please enter your country", CityDDL: "Please enter your city",
                LastName: "Please enter your lastname",
                //password: {
                //    required: "Please provide a password",
                //    minlength: "Your password must be at least 5 characters long"
                //},
                Email: "Please enter a valid email address"
            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                form.submit();
            }
        }
    });
});