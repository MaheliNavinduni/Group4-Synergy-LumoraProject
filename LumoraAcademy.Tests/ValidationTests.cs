using LumoraAcademy.Core.Services;

namespace LumoraAcademy.Tests;

// The rules every form in the system uses to check what the user typed.
public class ValidationTests
{
    // ----- required -----

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Required_RejectsEmptyValues(string? value)
    {
        Assert.NotEqual("", Validation.Required(value, "Full name"));
    }

    [Fact]
    public void Required_AcceptsAValue()
    {
        Assert.Equal("", Validation.Required("Nimali", "Full name"));
    }

    // ----- names -----

    [Theory]
    [InlineData("Nimali Perera")]
    [InlineData("A.B. Fernando")]
    [InlineData("O'Brien")]
    public void Name_AcceptsRealNames(string value)
    {
        Assert.Equal("", Validation.Name(value));
    }

    [Theory]
    [InlineData("Jo")]            // too short
    [InlineData("Student 123")]   // digits
    [InlineData("<script>")]      // symbols
    public void Name_RejectsBadNames(string value)
    {
        Assert.NotEqual("", Validation.Name(value));
    }

    // ----- email -----

    [Theory]
    [InlineData("nimali@example.com")]
    [InlineData("first.last+tag@mail.co.uk")]
    public void Email_AcceptsValidAddresses(string value)
    {
        Assert.Equal("", Validation.Email(value));
    }

    [Theory]
    [InlineData("nimali")]
    [InlineData("nimali@")]
    [InlineData("nimali@example")]
    [InlineData("nimali example@mail.com")]
    public void Email_RejectsInvalidAddresses(string value)
    {
        Assert.NotEqual("", Validation.Email(value));
    }

    [Fact]
    public void Email_CanBeOptional()
    {
        Assert.Equal("", Validation.Email("", required: false));
        Assert.NotEqual("", Validation.Email("", required: true));
    }

    // ----- phone -----

    [Theory]
    [InlineData("0771234567")]
    [InlineData("077 123 4567")]
    [InlineData("077-123-4567")]
    [InlineData("+94771234567")]
    public void Phone_AcceptsSriLankanNumbers(string value)
    {
        Assert.Equal("", Validation.Phone(value));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("07712345678")]   // too many digits
    [InlineData("1771234567")]    // does not start with 0
    [InlineData("abcdefghij")]
    public void Phone_RejectsBadNumbers(string value)
    {
        Assert.NotEqual("", Validation.Phone(value));
    }

    [Fact]
    public void CleanPhone_StoresOneTidyForm()
    {
        Assert.Equal("0771234567", Validation.CleanPhone("077 123 4567"));
        Assert.Equal("0771234567", Validation.CleanPhone("+94771234567"));
    }

    // ----- address -----

    [Fact]
    public void Address_NeedsEnoughDetail()
    {
        Assert.NotEqual("", Validation.Address("Colombo"));
        Assert.Equal("", Validation.Address("45 Galle Road, Colombo 03"));
    }

    // ----- NIC -----

    [Theory]
    [InlineData("901234567V")]
    [InlineData("901234567x")]
    [InlineData("200145601234")]
    public void Nic_AcceptsBothFormats(string value)
    {
        Assert.Equal("", Validation.Nic(value));
    }

    [Theory]
    [InlineData("9012345")]
    [InlineData("901234567A")]
    public void Nic_RejectsBadNumbers(string value)
    {
        Assert.NotEqual("", Validation.Nic(value));
    }

    // ----- dates -----

    [Fact]
    public void DateOfBirth_RejectsFutureDates()
    {
        var today = new DateTime(2026, 9, 24);
        Assert.NotEqual("", Validation.DateOfBirth(today.AddDays(1), today: today));
    }

    [Fact]
    public void DateOfBirth_AcceptsASchoolAgeStudent()
    {
        var today = new DateTime(2026, 9, 24);
        Assert.Equal("", Validation.DateOfBirth(new DateTime(2010, 5, 3), today: today));
    }

    [Fact]
    public void DateOfBirth_RejectsAnUnlikelyAge()
    {
        var today = new DateTime(2026, 9, 24);
        Assert.NotEqual("", Validation.DateOfBirth(new DateTime(1950, 5, 3), today: today));
        Assert.NotEqual("", Validation.DateOfBirth(new DateTime(2025, 5, 3), today: today));
    }

    [Fact]
    public void AgeOn_CountsTheBirthdayCorrectly()
    {
        var today = new DateTime(2026, 9, 24);
        Assert.Equal(16, Validation.AgeOn(new DateTime(2010, 9, 24), today));   // birthday today
        Assert.Equal(15, Validation.AgeOn(new DateTime(2010, 9, 25), today));   // birthday tomorrow
    }

    [Fact]
    public void NotInFuture_And_NotInPast()
    {
        var today = new DateTime(2026, 9, 24);
        Assert.NotEqual("", Validation.NotInFuture(today.AddDays(1), "Joining date", today));
        Assert.Equal("", Validation.NotInFuture(today, "Joining date", today));
        Assert.NotEqual("", Validation.NotInPast(today.AddDays(-1), "Event date", today));
        Assert.Equal("", Validation.NotInPast(today, "Event date", today));
    }

    // ----- numbers -----

    [Fact]
    public void WholeNumber_ChecksTheRange()
    {
        Assert.Equal("", Validation.WholeNumber("5", "Experience", 0, 50));
        Assert.NotEqual("", Validation.WholeNumber("60", "Experience", 0, 50));
        Assert.NotEqual("", Validation.WholeNumber("five", "Experience", 0, 50));
    }

    [Fact]
    public void Money_ChecksTheAmount()
    {
        Assert.Equal("", Validation.Money("2500", "Monthly fee", 1));
        Assert.NotEqual("", Validation.Money("0", "Monthly fee", 1));
        Assert.NotEqual("", Validation.Money("free", "Monthly fee", 1));
    }

    [Fact]
    public void Marks_AreOutOfOneHundred()
    {
        Assert.Equal("", Validation.Marks("75"));
        Assert.NotEqual("", Validation.Marks("101"));
        Assert.NotEqual("", Validation.Marks("-1"));
        Assert.NotEqual("", Validation.Marks(""));
    }

    // ----- login details -----

    [Fact]
    public void Username_NeedsFourToTwentyPlainCharacters()
    {
        Assert.Equal("", Validation.Username("n.perera"));
        Assert.NotEqual("", Validation.Username("abc"));
        Assert.NotEqual("", Validation.Username("has space"));
    }

    [Fact]
    public void Password_NeedsALetterAndANumber()
    {
        Assert.Equal("", Validation.Password("lumora1"));
        Assert.NotEqual("", Validation.Password("lumora"));
        Assert.NotEqual("", Validation.Password("12345678"));
        Assert.NotEqual("", Validation.Password("ab1"));
    }

    // ----- putting checks together -----

    [Fact]
    public void FirstProblem_ReportsOneMessageAtATime()
    {
        string message = Validation.FirstProblem(
            Validation.Name("Nimali Perera"),
            Validation.Email("wrong"),
            Validation.Phone("12345"));

        Assert.Contains("email", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FirstProblem_IsEmptyWhenEverythingIsValid()
    {
        string message = Validation.FirstProblem(
            Validation.Name("Nimali Perera"),
            Validation.Email("nimali@example.com"),
            Validation.Phone("0771234567"));

        Assert.Equal("", message);
    }
}
