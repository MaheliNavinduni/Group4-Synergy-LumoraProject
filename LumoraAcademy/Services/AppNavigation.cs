namespace LumoraAcademy.Services;

// One place for moving between pages, so every page does it the same way.
public static class AppNavigation
{
    // Who is logged in right now. Set by the Login page.
    public static string CurrentRole { get; set; } = "";      // "Admin" or "Teacher"
    public static string CurrentUserName { get; set; } = "";

    private static INavigation Navigation => Application.Current!.MainPage!.Navigation;

    // Open a page on top of the current one.
    public static async Task GoToAsync(Page page)
    {
        NavigationPage.SetHasNavigationBar(page, false);
        await Navigation.PushAsync(page);
    }

    // Close the current page and go back to the one before it.
    public static async Task GoBackAsync()
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
    }

    // Log out: forget the user and return to the Home page.
    public static async Task LogoutAsync()
    {
        CurrentRole = "";
        CurrentUserName = "";
        await Navigation.PopToRootAsync();
    }

    // Opens the page that belongs to a sidebar menu item.
    public static async Task GoToMenuItemAsync(string menuItem)
    {
        Page? page = null;

        if (CurrentRole == "Admin")
        {
            page = menuItem switch
            {
                "Dashboard" => new Pages.Admin.AdminDashboardPage(),
                "Students" => new Pages.Admin.AdminStudentsPage(),
                "Teachers" => new Pages.Admin.AdminTeachersPage(),
                "Payments" => new Pages.Admin.AdminPaymentsPage(),
                "Academics" => new Pages.Admin.AdminAcademicsPage(),
                "Attendance" => new Pages.Admin.AdminAttendancePage(),
                "Upcoming Events" => new Pages.Admin.AdminEventsPage(),
                _ => null
            };
        }
        else
        {
            page = menuItem switch
            {
                "Dashboard" => new Pages.Teacher.TeacherDashboardPage(),
                "My Subjects" => new Pages.Teacher.TeacherSubjectsPage(),
                "Attendance" => new Pages.Teacher.TeacherAttendancePage(),
                "Students" => new Pages.Teacher.TeacherStudentsPage(),
                "Upcoming Events" => new Pages.Teacher.TeacherEventsPage(),
                _ => null
            };
        }

        if (page != null)
        {
            // Sidebar pages replace each other instead of stacking up,
            // so "Go Back" from a detail page always returns to the right section.
            var previousPage = Navigation.NavigationStack.Last();
            await GoToAsync(page);
            Navigation.RemovePage(previousPage);
        }
    }
}
