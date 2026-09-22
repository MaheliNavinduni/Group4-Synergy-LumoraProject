using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;

namespace LumoraAcademy.Tests;

// Epic 4 - Academic Performance Management.
public class AcademicServiceTests
{
    private static int StudentId(TestBackend t, string code) => t.Backend.Students.GetByStudentId(code)!.Id;
    private static int SubjectId(TestBackend t, string code) => t.Backend.Academics.GetSubjects().First(s => s.Code == code).Id;

    // ----- subjects -----

    [Fact]
    public void AddSubject_SavesAndRejectsDuplicateCode()
    {
        using var t = new TestBackend();

        var s = t.Backend.Academics.AddSubject("Physics", "P9001", "Cambridge Curriculum");

        Assert.Equal("P9001", s.Code);
        Assert.Contains(t.Backend.Academics.GetSubjects(), x => x.Id == s.Id);
        Assert.Throws<InvalidOperationException>(() => t.Backend.Academics.AddSubject("Physics again", "p9001", ""));
    }

    // ----- US1: record marks (FR-07) -----

    [Fact]
    public void RecordMark_MonthlyTermAndYearEnd_AreSavedWithGrades()
    {
        using var t = new TestBackend();
        int student = StudentId(t, "STU-8492");
        int maths = SubjectId(t, "M2103");

        t.Backend.Academics.RecordMark(student, maths, AcademicService.ExamMonthly, "Sept 2026", 72);
        t.Backend.Academics.RecordMark(student, maths, AcademicService.ExamTerm, "Term 3 2026", 88);
        t.Backend.Academics.RecordMark(student, maths, AcademicService.ExamYearEnd, "Year End 2026", 95);

        var results = t.Backend.Academics.GetResultsForStudent(student);
        Assert.Equal(3, results.Count);
        Assert.Equal("B-", results.First(r => r.ExamType == AcademicService.ExamMonthly).Grade);
        Assert.Equal("A-", results.First(r => r.ExamType == AcademicService.ExamTerm).Grade);
        Assert.Equal("A+", results.First(r => r.ExamType == AcademicService.ExamYearEnd).Grade);
        Assert.All(results, r => Assert.Equal("Mathematics", r.SubjectName));
    }

    [Fact]
    public void RecordMark_OutOfRangeOrWrongType_Throws()
    {
        using var t = new TestBackend();
        int student = StudentId(t, "STU-8492");
        int maths = SubjectId(t, "M2103");

        Assert.Throws<ArgumentException>(() => t.Backend.Academics.RecordMark(student, maths, AcademicService.ExamMonthly, "x", 101));
        Assert.Throws<ArgumentException>(() => t.Backend.Academics.RecordMark(student, maths, "Quiz", "x", 50));
    }

    // ----- US2: Cambridge results (FR-08) -----

    [Fact]
    public void RecordCambridgeResult_SavesGradeAndAppearsInStudentRecord()
    {
        using var t = new TestBackend();
        int student = StudentId(t, "STU-8492");
        int mathsCam = SubjectId(t, "M1203");

        t.Backend.Academics.RecordCambridgeResult(student, mathsCam, "IGCSE May 2026", "a*");

        var cambridge = t.Backend.Academics.GetResultsForStudent(student).Where(r => r.ExamType == AcademicService.ExamCambridge).ToList();
        Assert.Single(cambridge);
        Assert.Equal("A*", cambridge[0].Grade);
        Assert.Equal("IGCSE May 2026", cambridge[0].ExamName);
    }

    [Fact]
    public void CambridgeResults_DoNotAffectInternalAverage()
    {
        using var t = new TestBackend();
        int student = StudentId(t, "STU-8492");
        t.Backend.Academics.RecordMark(student, SubjectId(t, "M2103"), AcademicService.ExamTerm, "T", 80);
        t.Backend.Academics.RecordCambridgeResult(student, SubjectId(t, "M1203"), "IGCSE", "C", 40);

        Assert.Equal(80, t.Backend.Academics.AverageForStudent(student));
    }

    // ----- US3: weak students (FR-09) -----

    [Fact]
    public void GetAtRiskStudents_FindsStudentsBelow60Percent()
    {
        using var t = new TestBackend();

        var atRisk = t.Backend.Academics.GetAtRiskStudents(60);
        var names = atRisk.Select(a => a.Student.FullName).ToList();

        Assert.Contains("Emma Stone", names);
        Assert.Contains("Mia Patel", names);
        Assert.DoesNotContain("James Chen", names);       // average 62
        Assert.DoesNotContain("Eleanor Vance", names);    // average 93
    }

    [Fact]
    public void GetAtRiskStudents_HigherThreshold_IncludesMoreStudents()
    {
        using var t = new TestBackend();

        var names = t.Backend.Academics.GetAtRiskStudents(70).Select(a => a.Student.FullName).ToList();

        Assert.Contains("James Chen", names);
    }

    [Fact]
    public void GetAtRiskStudents_ListsWeakSubjectsAndCohort()
    {
        using var t = new TestBackend();

        var mia = t.Backend.Academics.GetAtRiskStudents(60).First(a => a.Student.FullName == "Mia Patel");

        Assert.Contains("Science", mia.WeakSubjects);
        Assert.Equal("10th Grade - Alpha", mia.Cohort);
        Assert.EndsWith("%", mia.AverageText);
    }

    [Fact]
    public void GetAtRiskStudents_FilterBySubject()
    {
        using var t = new TestBackend();

        var onlyMaths = t.Backend.Academics.GetAtRiskStudents(60, null, "Mathematics");

        Assert.All(onlyMaths, a => Assert.Contains("Mathematics", a.WeakSubjects));
    }

    [Fact]
    public void ProgressNote_WithExtraClassRecommendation_IsSaved()
    {
        using var t = new TestBackend();
        int emma = StudentId(t, "STU-2024-0891");

        t.Backend.Academics.AddProgressNote(emma, "Attend Saturday revision class.", true);

        var notes = t.Backend.Academics.GetProgressNotes(emma);
        Assert.True(notes.Count >= 2);
        Assert.Contains(notes, n => n.Note == "Attend Saturday revision class." && n.RecommendExtraClass);
    }

    [Fact]
    public void GradeFor_MapsMarksToLetters()
    {
        Assert.Equal("A+", ExamResult.GradeFor(97));
        Assert.Equal("A", ExamResult.GradeFor(90));
        Assert.Equal("B", ExamResult.GradeFor(76));
        Assert.Equal("C", ExamResult.GradeFor(60));
        Assert.Equal("F", ExamResult.GradeFor(30));
    }
}
