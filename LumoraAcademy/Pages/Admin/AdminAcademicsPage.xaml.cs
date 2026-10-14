using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Subject management (supports Epic 4 - marks are recorded per subject).
public partial class AdminAcademicsPage : ContentPage
{
    public AdminAcademicsPage()
    {
        InitializeComponent();

        LoadSubjects();
        Loaded += (s, e) => LoadSubjects();
    }

    private void LoadSubjects()
    {
        var subjects = AppData.Academics.GetSubjects();
        var classes = AppData.Classes.GetAll();

        // How many classes use each subject, so the card can say so.
        var classesPerSubject = classes
            .GroupBy(c => c.SubjectId)
            .ToDictionary(g => g.Key, g => g.Count());

        var cards = subjects.Select(s => new SubjectCard
        {
            Name = s.Name,
            Code = s.Code,
            Curriculum = string.IsNullOrWhiteSpace(s.Curriculum) ? "No curriculum set" : s.Curriculum,
            ClassCount = classesPerSubject.TryGetValue(s.Id, out int n) ? n : 0,
        }).ToList();

        BindableLayout.SetItemsSource(SubjectList, cards);
        NoSubjectsLabel.IsVisible = cards.Count == 0;

        SubjectsCard.Value = subjects.Count.ToString();
        ClassesCard.Value = classes.Count.ToString();
        EnrolmentsCard.Value = classes.Sum(c => c.StudentCount).ToString();
        CurriculaCard.Value = subjects
            .Select(s => s.Curriculum)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .Count()
            .ToString();
    }

    // What one subject card shows.
    private class SubjectCard
    {
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string Curriculum { get; set; } = "";
        public int ClassCount { get; set; }

        public string ClassCountText => ClassCount == 1 ? "1 class" : $"{ClassCount} classes";
    }

    private async void OnClassesClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new ClassesPage());
    }

    private async void OnAddSubjectClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new AddSubjectPage());
    }
}
