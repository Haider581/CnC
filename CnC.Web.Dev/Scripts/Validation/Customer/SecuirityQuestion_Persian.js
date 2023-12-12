
window.onload = function () {
    var input = document.getElementById("SecurityQuestionDDL").focus();
    alert();
}
// submit form data
$(document).ready(function () {
    $("#SecurityQuestionDDL").change(function () {
        $("#questionText").val("");
        $("#questionText").val($("#SecurityQuestionDDL option:selected").text());
    });

    // Wait for the DOM to be ready
    $(function () {
        // Initialize form validation on the registration form.
        // It has the name attribute "registration"
        $("form[name='addSecurityQuestionForm']").validate({
            // Specify validation rules
            rules: {
                // The key name on the left side is the name attribute
                // of an input field. Validation rules are defined
                // on the right side
                SecurityQuestionDDL: "required",
                answer: "required",
                //CountryDDL: "required",
                //CityDDL: "required",
                //DateOfBirth: "required",
                //Gender: "required",
                //MaritalStatus: "required",
                //NIC: "required",
                //Nationality: "required",
                //PassportNo: "required",
                //Address: "required",
                //PostalCode: "required",
                //NICDoc: "required",
                //ProofOfAddressDoc: "required",
                //PassportDoc: "required",
                //ProofOfSourceOfFunds: "required",
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
                    SecurityQuestionDDL: "Please select your Security Question",
                    answer: "Please enter your answer",
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
});


