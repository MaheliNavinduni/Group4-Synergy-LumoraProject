using System.Text.RegularExpressions;

namespace LumoraAcademy.Core.Services;

// One place for every input rule in the system.
// Each method returns an empty string when the value is fine,
// or a short message to show the user when it is not.
public static class Validation
{
    // ---------- Basic ----------

    // Use for any box the user must fill in.
    public static string Required(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fieldName + " is required.";
        }
        return "";
    }

    // Letters, spaces, apostrophes and dots only - no digits in a person's name.
    public static string Name(string? value, string fieldName = "Full name")
    {
        string required = Required(value, fieldName);
        if (required != "") return required;

        string text = value!.Trim();

        if (text.Length < 3)
        {
            return fieldName + " must be at least 3 characters.";
        }
        if (text.Length > 60)
        {
            return fieldName + " cannot be longer than 60 characters.";
        }
        if (!Regex.IsMatch(text, @"^[A-Za-z][A-Za-z .'-]*$"))
        {
            return fieldName + " can only contain letters, spaces, dots and apostrophes.";
        }
        return "";
    }

    // ---------- Contact details ----------

    // name@example.com
    public static string Email(string? value, bool required = true)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return required ? "Email address is required." : "";
        }

        string text = value.Trim();

        if (!Regex.IsMatch(text, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$"))
        {
            return "Enter a valid email address, for example name@example.com.";
        }
        if (text.Length > 100)
        {
            return "Email address is too long.";
        }
        return "";
    }

    // Sri Lankan numbers: 0771234567 or +94771234567.
    // Spaces and dashes typed by the user are ignored.
    public static string Phone(string? value, bool required = true, string fieldName = "Phone number")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return required ? fieldName + " is required." : "";
        }

        string digits = OnlyDigitsAndPlus(value);

        if (Regex.IsMatch(digits, @"^0\d{9}$")) return "";
        if (Regex.IsMatch(digits, @"^\+94\d{9}$")) return "";

        return fieldName + " must be 10 digits starting with 0, for example 0771234567.";
    }

    // Returns the phone number in one tidy form so the database stays consistent.
    public static string CleanPhone(string? value)
    {
        string digits = OnlyDigitsAndPlus(value);
        if (digits.StartsWith("+94") && digits.Length == 12)
        {
            return "0" + digits.Substring(3);
        }
        return digits;
    }

    private static string OnlyDigitsAndPlus(string? value)
    {
        if (value == null) return "";

        var kept = new System.Text.StringBuilder();
        foreach (char c in value)
        {
            if (char.IsDigit(c) || (c == '+' && kept.Length == 0))
            {
                kept.Append(c);
            }
        }
        return kept.ToString();
    }

    // A real address needs a number and a street, so we ask for a little detail.
    public static string Address(string? value, bool required = true)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return required ? "Address is required." : "";
        }

        string text = value.Trim();

        if (text.Length < 8)
        {
            return "Enter the full address, for example 45 Galle Road, Colombo 03.";
        }
        if (text.Length > 200)
        {
            return "Address cannot be longer than 200 characters.";
        }
        return "";
    }

    // ---------- Dates ----------

    // Students are between 3 and 25 years old, so anything outside that is a typing mistake.
    public static string DateOfBirth(DateTime value, int minAge = 3, int maxAge = 25, DateTime? today = null)
    {
        DateTime now = today ?? DateTime.Today;

        if (value.Date > now.Date)
        {
            return "Date of birth cannot be in the future.";
        }

        int age = AgeOn(value, now);

        if (age < minAge)
        {
            return "The student must be at least " + minAge + " years old.";
        }
        if (age > maxAge)
        {
            return "Please check the date of birth - it gives an age of " + age + ".";
        }
        return "";
    }

    public static int AgeOn(DateTime birthDate, DateTime onDate)
    {
        int age = onDate.Year - birthDate.Year;
        if (birthDate.Date > onDate.AddYears(-age).Date) age--;
        return age;
    }

    public static string NotInFuture(DateTime value, string fieldName, DateTime? today = null)
    {
        DateTime now = today ?? DateTime.Today;
        return value.Date > now.Date ? fieldName + " cannot be in the future." : "";
    }

    public static string NotInPast(DateTime value, string fieldName, DateTime? today = null)
    {
        DateTime now = today ?? DateTime.Today;
        return value.Date < now.Date ? fieldName + " cannot be in the past." : "";
    }

    // ---------- Numbers ----------

    // Reads a whole number such as years of experience.
    public static string WholeNumber(string? value, string fieldName, int min, int max, bool required = true)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return required ? fieldName + " is required." : "";
        }
        if (!int.TryParse(value.Trim(), out int number))
        {
            return fieldName + " must be a number.";
        }
        if (number < min || number > max)
        {
            return fieldName + " must be between " + min + " and " + max + ".";
        }
        return "";
    }

    // Reads an amount of money such as a class fee.
    public static string Money(string? value, string fieldName, decimal min = 0, decimal max = 1000000, bool required = true)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return required ? fieldName + " is required." : "";
        }
        if (!decimal.TryParse(value.Trim(), out decimal amount))
        {
            return fieldName + " must be a number, for example 2500.";
        }
        if (amount < min || amount > max)
        {
            return fieldName + " must be between " + min.ToString("N0") + " and " + max.ToString("N0") + ".";
        }
        return "";
    }

    // Exam marks are out of 100.
    public static string Marks(string? value, string fieldName = "Marks")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fieldName + " is required.";
        }
        if (!double.TryParse(value.Trim(), out double marks))
        {
            return fieldName + " must be a number.";
        }
        if (marks < 0 || marks > 100)
        {
            return fieldName + " must be between 0 and 100.";
        }
        return "";
    }

    // ---------- Login details ----------

    public static string Username(string? value)
    {
        string required = Required(value, "Username");
        if (required != "") return required;

        string text = value!.Trim();

        if (text.Length < 4 || text.Length > 20)
        {
            return "Username must be between 4 and 20 characters.";
        }
        if (!Regex.IsMatch(text, @"^[A-Za-z0-9._]+$"))
        {
            return "Username can only contain letters, numbers, dots and underscores.";
        }
        return "";
    }

    public static string Password(string? value)
    {
        string required = Required(value, "Password");
        if (required != "") return required;

        string text = value!;

        if (text.Length < 6)
        {
            return "Password must be at least 6 characters.";
        }
        if (!text.Any(char.IsLetter) || !text.Any(char.IsDigit))
        {
            return "Password must contain at least one letter and one number.";
        }
        return "";
    }

    // ---------- Putting several checks together ----------

    // Returns the first problem found, or an empty string when everything is fine.
    // Use it so the user is shown one clear message at a time.
    public static string FirstProblem(params string[] results)
    {
        foreach (string result in results)
        {
            if (!string.IsNullOrEmpty(result)) return result;
        }
        return "";
    }
}
