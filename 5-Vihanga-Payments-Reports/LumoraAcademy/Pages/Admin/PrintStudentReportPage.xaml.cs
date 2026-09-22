using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Epic 5 - the printable Academic Progress Report for one student.
public partial class PrintStudentReportPage : ContentPage
{
    private readonly int _studentId;

    public PrintStudentReportPage(int studentId)
    {
        InitializeComponent();
        _studentId = studentId;

        LoadReport();
    }

    private void LoadReport()
    {
        StudentReport report = AppData.Reports.BuildStudentReport(_studentId);
        var student = report.Student;

        YearLabel.Text = $"Academic Year: {DateTime.Today.Year}";
        TermLabel.Text = $"Generated: {DateTime.Today:MMM dd, yyyy}";

        NameLabel.Text = student.FullName;
        IdLabel.Text = student.StudentId;
        GradeLabel.Text = string.IsNullOrWhiteSpace(student.Section) ? student.Grade : $"{student.Grade} (Section {student.Section})";
        Avatar.Initials = student.Initials;
        Avatar.ImagePath = student.PhotoPath;

        BindableLayout.SetItemsSource(RowList, report.Rows);

        GpaLabel.Text = report.Rows.Count == 0 ? "-" : report.Gpa.ToString("0.00");
        TotalLabel.Text = report.Rows.Count == 0 ? "-" : $"{report.TotalPercent:0.0}%";
        AttendanceLabel.Text = report.DaysTotal == 0 ? "-" : $"{report.DaysPresent}/{report.DaysTotal}";

        // Progress chart: the first four subjects' final scores, scaled to the 70px chart height.
        var bars = new[] { Bar1, Bar2, Bar3, Bar4 };
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].HeightRequest = i < report.Rows.Count ? Math.Max(4, report.Rows[i].FinalScore * 0.7) : 4;
        }
    }

    private async void OnGoBackClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    // Builds the PDF, saves it next to the database, and opens it in the default PDF viewer
    // (Edge / Acrobat), where the user can press Print.
    private async void OnPrintClicked(object sender, EventArgs e)
    {
        try
        {
            StudentReport report = AppData.Reports.BuildStudentReport(_studentId);
            string folder = Path.Combine(FileSystem.AppDataDirectory, "Reports");
            string file = Path.Combine(folder, $"{report.Student.StudentId}-progress-report.pdf");

            Core.Reports.StudentReportPdf.Save(report, file);

            await Launcher.Default.OpenAsync(new OpenFileRequest("Academic Progress Report", new ReadOnlyFile(file)));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Print Report", "Could not create the PDF: " + ex.Message, "OK");
        }
    }
}
