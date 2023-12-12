
window.onload = function () {
    var input = document.getElementById("SecurityQuestionDDL").focus();
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

                SecurityQuestionDDL: {
                    required: true,
                },
                answer: {
                    required: true,
                },
                Email: {
                    required: true,

                    email: true

                }
            },
            messages: {

                SecurityQuestionDDL: {
                    required: GetLocalizedString("RequiredField")
                },
                answer: {
                    required: GetLocalizedString("RequiredField")
                },

                Email: "Please enter a valid email address"
            },

            submitHandler: function (form) {
                form.submit();
            }

        });
    });
});


