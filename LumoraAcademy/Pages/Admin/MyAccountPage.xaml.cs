using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Lets whoever is signed in change their own username and password.
// Changing the password needs the current one, so that a machine left
// unattended cannot be used to take over the account.
public partial class MyAccountPage : ContentPage
{
    private User? _user;

    public MyAccountPage()
    {
        InitializeComponent();
        LoadAccount();
    }

    private void LoadAccount()
    {
        _user = AppData.Auth.GetById(AppData.CurrentUserId);

        if (_user == null)
        {
            ShowProblem("Could not find your account. Please sign out and sign in again.");
            return;
        }

        UsernameEntry.Text = _user.Username;
        DisplayNameLabel.Text = string.IsNullOrWhiteSpace(_user.DisplayName) ? _user.Username : _user.DisplayName;
        RoleLabel.Text = _user.Role == "Admin" ? "Administrator" : "Teacher";
        SignedInAsLabel.Text = $"Signed in as {_user.Username}";

        ClearPasswordBoxes();
    }

    private void ClearPasswordBoxes()
    {
        CurrentPasswordEntry.Text = "";
        NewPasswordEntry.Text = "";
        ConfirmPasswordEntry.Text = "";
    }

    private void OnTogglePasswordTapped(object sender, EventArgs e)
    {
        NewPasswordEntry.IsPassword = !NewPasswordEntry.IsPassword;
        ShowPasswordIcon.Text = NewPasswordEntry.IsPassword ? "" : "";
    }

    // ---------- Username ----------

    private async void OnSaveUsernameClicked(object sender, EventArgs e)
    {
        if (_user == null) return;

        string newUsername = (UsernameEntry.Text ?? "").Trim();

        string problem = Validation.Username(newUsername);
        if (ShowProblem(problem)) return;

        if (newUsername.Equals(_user.Username, StringComparison.OrdinalIgnoreCase))
        {
            ShowProblem("That is already your username.");
            return;
        }

        bool confirm = await DisplayAlert("Change Username",
            $"You will sign in as '{newUsername.ToLower()}' from now on. Change it?", "Change", "Cancel");
        if (!confirm) return;

        try
        {
            AppData.Auth.ChangeUsername(_user.Id, newUsername);
            LoadAccount();
            ShowProblem("");
            await DisplayAlert("Change Username",
                $"Done. Use '{newUsername.ToLower()}' the next time you sign in.", "OK");
        }
        catch (Exception ex)
        {
            ShowProblem(ex.Message);
            LoadAccount();
        }
    }

    // ---------- Password ----------

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        if (_user == null) return;

        string current = CurrentPasswordEntry.Text ?? "";
        string newPassword = NewPasswordEntry.Text ?? "";
        string confirmPassword = ConfirmPasswordEntry.Text ?? "";

        string problem = Validation.FirstProblem(
            Validation.Required(current, "Current password"),
            Validation.Password(newPassword),
            newPassword != confirmPassword ? "The two new passwords do not match." : "");

        if (ShowProblem(problem)) return;

        try
        {
            AppData.Auth.ChangeOwnPassword(_user.Id, current, newPassword);
            ClearPasswordBoxes();
            ShowProblem("");
            await DisplayAlert("Change Password",
                "Your password has been changed. Use the new one the next time you sign in.", "OK");
        }
        catch (Exception ex)
        {
            ShowProblem(ex.Message);
        }
    }

    // Shows the message in the red bar. Returns true when there was a problem.
    private bool ShowProblem(string message)
    {
        bool hasProblem = !string.IsNullOrEmpty(message);

        ErrorLabel.Text = message;
        ErrorBox.IsVisible = hasProblem;

        return hasProblem;
    }
}
