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

    // Goes back to the main page of a section, closing any pages opened on top of
    // it. The breadcrumb uses this, so clicking "Attendance" from three pages deep
    // lands on the Attendance screen rather than only one step back.
    public static async Task GoToSectionAsync(string menuItem)
    {
        Page? root = NewPageFor(menuItem);
        if (root == null) return;

        var stack = Navigation.NavigationStack;

        // If that section's page is already open underneath us, pop back to it.
        for (int i = stack.Count - 1; i >= 0; i--)
        {
            if (stack[i].GetType() == root.GetType())
            {
                while (Navigation.NavigationStack.Count > i + 1)
                {
                    await Navigation.PopAsync(false);
                }
                return;
            }
        }

        // Otherwise open a fresh one.
        await GoToMenuItemAsync(menuItem);
    }

    // Opens the page that belongs to a sidebar menu item.
    public static async Task GoToMenuItemAsync(string menuItem)
    {
        Page? page = NewPageFor(menuItem);

        if (page != null)
        {
            // Sidebar pages replace each other instead of stacking up,
            // so "Go Back" from a detail page always returns to the right section.
            var previousPage = Navigation.NavigationStack.Last();
            await GoToAsync(page);
            Navigation.RemovePage(previousPage);
        }
    }

    // The main page behind each sidebar item, for whichever role is signed in.
    private static Page? NewPageFor(string menuItem)
    {
        if (CurrentRole == "Admin")
        {
            return menuItem switch
            {
                "Dashboard" => new Pages.Admin.AdminDashboardPage(),
                "Students" => new Pages.Admin.AdminStudentsPage(),
                "Teachers" => new Pages.Admin.AdminTeachersPage(),
                "Payments" => new Pages.Admin.AdminPaymentsPage(),
                "Academics" => new Pages.Admin.AdminAcademicsPage(),
                "Attendance" => new Pages.Admin.MarkAttendancePage(),
                "Upcoming Events" => new Pages.Admin.AdminEventsPage(),
                "My Account" => new Pages.Admin.MyAccountPage(),
                _ => null
            };
        }

        return menuItem switch
        {
            "Dashboard" => new Pages.Teacher.TeacherDashboardPage(),
            "My Subjects" => new Pages.Teacher.TeacherSubjectsPage(),
            "Attendance" => new Pages.Teacher.TeacherAttendancePage(),
            "Students" => new Pages.Teacher.TeacherStudentsPage(),
            "Upcoming Events" => new Pages.Teacher.TeacherEventsPage(),
            _ => null
        };
    }
}
