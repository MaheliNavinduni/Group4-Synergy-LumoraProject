using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// "Teacher" here means the database entity, not the Pages.Teacher folder.
using Teacher = LumoraAcademy.Core.Entities.Teacher;

// Epic 1, User Story 2 - edit teacher account information, including the
// username and password the teacher signs in with.
public partial class EditTeacherPage : ContentPage
{
    private readonly Teacher _teacher;

    // The teacher's login account, or null when they do not have one.
    private User? _login;

    public EditTeacherPage(int teacherId)
    {
        InitializeComponent();

        _teacher = AppData.Teachers.GetById(teacherId) ?? new Teacher();

        SubtitleLabel.Text = $"Update information for {_teacher.FullName}.";
        FullNameEntry.Text = _teacher.FullName;
        TeacherIdEntry.Text = _teacher.TeacherId;
        ExperienceEntry.Text = _teacher.YearsOfExperience.ToString();
        JoiningDatePicker.Date = _teacher.JoiningDate == default ? DateTime.Today : _teacher.JoiningDate;
        EmailEntry.Text = _teacher.Email;
        PhoneEntry.Text = _teacher.Phone;
        AddressEditor.Text = _teacher.Address;

        LoadLogin();
    }

    // ---------- Login details ----------

    private void LoadLogin()
    {
        _login = AppData.Auth.GetByTeacherId(_teacher.Id);

        bool hasLogin = _login != null;
        LoginSection.IsVisible = hasLogin;
        NoLoginSection.IsVisible = !hasLogin;

        if (hasLogin)
        {
            UsernameEntry.Text = _login!.Username;
            NewPasswordEntry.Text = "";
            LoginNoteLabel.Text = _login.IsActive
                ? "The teacher uses these to sign in. Tell them if you change anything here."
                : "This login is deactivated, so the teacher cannot sign in at the moment.";
        }
    }

    // Lets the admin check the password they are setting.
    private void OnTogglePasswordTapped(object sender, EventArgs e)
    {
        NewPasswordEntry.IsPassword = !NewPasswordEntry.IsPassword;
        ShowPasswordIcon.Text = NewPasswordEntry.IsPassword ? "" : "";
    }

    // Creates a login for a teacher who does not have one yet.
    private async void OnCreateLoginClicked(object sender, EventArgs e)
    {
        string suggested = SuggestUsername(_teacher.FullName);

        string username = await DisplayPromptAsync("Create Login",
            "Username for this teacher:", "Create", "Cancel", initialValue: suggested, maxLength: 20);
        if (string.IsNullOrWhiteSpace(username)) return;

        string password = await DisplayPromptAsync("Create Login",
            "Password (at least 6 characters, with one letter and one number):", "Create", "Cancel", maxLength: 40);
        if (string.IsNullOrWhiteSpace(password)) return;

        string problem = Validation.FirstProblem(
            Validation.Username(username),
            Validation.Password(password));

        if (problem != "")
        {
            await DisplayAlert("Create Login", problem, "OK");
            return;
        }

        try
        {
            AppData.Auth.CreateUser(username.Trim(), password, "Teacher", _teacher.FullName, _teacher.Id);
            LoadLogin();
            await DisplayAlert("Create Login",
                $"{_teacher.FullName} can now sign in as '{username.Trim().ToLower()}'.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Create Login", ex.Message, "OK");
        }
    }

    // Turns "K. Fernando" into "k.fernando" as a starting point.
    private static string SuggestUsername(string fullName)
    {
        var parts = (fullName ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "";

        string first = new string(parts[0].Where(char.IsLetter).ToArray());
        string last = new string(parts[^1].Where(char.IsLetter).ToArray());

        string suggestion = parts.Length == 1 ? first : first.Substring(0, 1) + "." + last;
        return suggestion.ToLower();
    }

    // ---------- Saving ----------

    private async void OnPhotoClicked(object sender, EventArgs e)
    {
        string? path = await PhotoService.PickAndSaveAsync("teacher");
        if (path == null) return;

        AppData.Teachers.SetPhoto(_teacher.Id, path);
        _teacher.PhotoPath = path;
        await DisplayAlert("Photo", "Photo updated.", "OK");
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string newPassword = NewPasswordEntry.Text ?? "";

        string problem = Validation.FirstProblem(
            Validation.Name(FullNameEntry.Text, "Full name"),
            Validation.Required(TeacherIdEntry.Text, "Teacher ID"),
            Validation.WholeNumber(ExperienceEntry.Text, "Years of experience", 0, 60),
            Validation.NotInFuture(JoiningDatePicker.Date, "Date of joining"),
            Validation.Email(EmailEntry.Text),
            Validation.Phone(PhoneEntry.Text),
            Validation.Address(AddressEditor.Text),
            // The login boxes are only checked when the teacher has an account.
            _login == null ? "" : Validation.Username(UsernameEntry.Text),
            // A blank password box means "leave the password as it is".
            _login == null || newPassword == "" ? "" : Validation.Password(newPassword));

        if (problem != "")
        {
            await DisplayAlert("Edit Teacher", problem, "OK");
            return;
        }

        int.TryParse(ExperienceEntry.Text, out int years);

        _teacher.FullName = (FullNameEntry.Text ?? "").Trim();
        _teacher.YearsOfExperience = years;
        _teacher.JoiningDate = JoiningDatePicker.Date;
        _teacher.Email = (EmailEntry.Text ?? "").Trim();
        _teacher.Phone = Validation.CleanPhone(PhoneEntry.Text);
        _teacher.Address = (AddressEditor.Text ?? "").Trim();

        try
        {
            AppData.Teachers.Update(_teacher);

            string changed = "Changes saved.";

            if (_login != null)
            {
                string newUsername = (UsernameEntry.Text ?? "").Trim();
                bool usernameChanged = !newUsername.Equals(_login.Username, StringComparison.OrdinalIgnoreCase);

                // The username is changed first, so that if it is already taken
                // nothing else about the login has been touched.
                if (usernameChanged) AppData.Auth.ChangeUsername(_login.Id, newUsername);
                if (newPassword != "") AppData.Auth.ChangePassword(_login.Id, newPassword);

                if (usernameChanged && newPassword != "")
                {
                    changed = $"Changes saved. {_teacher.FullName} now signs in as '{newUsername.ToLower()}' with the new password.";
                }
                else if (usernameChanged)
                {
                    changed = $"Changes saved. {_teacher.FullName} now signs in as '{newUsername.ToLower()}'.";
                }
                else if (newPassword != "")
                {
                    changed = $"Changes saved. {_teacher.FullName} has a new password.";
                }
            }

            await DisplayAlert("Edit Teacher", changed, "OK");
            await AppNavigation.GoBackAsync();
        }
        catch (Exception ex)
        {
            // The teacher details may already be saved, so the login is reloaded
            // to show exactly what is in the database now.
            LoadLogin();
            await DisplayAlert("Edit Teacher", ex.Message, "OK");
        }
    }
}
