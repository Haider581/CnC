function GetLocalizedString(key) {
    if (key == "RequiredField") {
        return "This field is required";
    }
    else if (key == "MinimumLength8") {
        return "Please enter at least 8 characters";
    }
    else if (key == "MinimumLength7") {
        return "Please enter at least 7 characters";
    }
    else if (key == "MinimumLength4") {
        return "Please enter at least 4 characters";
    }
    else if (key == "MinimumLength9") {
        return "Please enter at least 9 characters";
    }
    else if (key == "MinimumLength10") {
        return "Please enter at least 10 characters";
    }
    else if (key == "MaximumLength10") {
        return "Please enter no more than 10 characters";
    }
    else if (key == "MaximumLength9") {
        return "Please enter no more than 9 characters";
    }
    else if (key == "MaximumLength20") {
        return "Please enter no more than 20 characters";
    }
    else if (key == "PasswordCheck") {
        return "Please enter the same value again";
    }
    else if (key == "CheckDigits") {
        return "CheckDigitspersian";
    }
    else if (key == "StrongPasswordCheck") {
        return "Choose strong password: At-least one lower-case character, At-least one upper-case character, At-least one digit, At-least one special characters from allowed list, Allowed Characters: A-Z, a-z, 0-9, @ # $ % ^ & * () _ + !"
    }
    else if (key == "OnlyDigits") {
        return "Only Digits are allowed"
    }
    else if (key == "AlphabetsOnly") {
        return "Please enter a valid Name, only alphabets and spaces allowed"
    }
}