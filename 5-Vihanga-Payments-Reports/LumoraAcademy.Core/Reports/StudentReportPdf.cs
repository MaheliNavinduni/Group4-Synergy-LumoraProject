using LumoraAcademy.Core.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LumoraAcademy.Core.Reports;

// Turns a StudentReport into a PDF file that looks like the "Academic Progress Report" design.
public static class StudentReportPdf
{
    private static readonly string Brown = "#B87F52";
    private static readonly string DarkBrown = "#6B4E31";
    private static readonly string Grey = "#6B7280";
    private static readonly string LightGrey = "#F3F4F6";

    static StudentReportPdf()
    {
        // QuestPDF is free for projects like this one.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // Writes the PDF and returns the path.
    public static string Save(StudentReport report, string filePath)
    {
        string? folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);

        Build(report).GeneratePdf(filePath);
        return filePath;
    }

    public static byte[] ToBytes(StudentReport report)
    {
        return Build(report).GeneratePdf();
    }

    private static Document Build(StudentReport report)
    {
        var s = report.Student;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor("#1C1917"));

                // ---- Header ----
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Lumora Academy").FontSize(18).Bold().FontColor(DarkBrown);
                        col.Item().Text("Excellence in Education").FontSize(9).Italic().FontColor(Grey);
                    });
                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text("ACADEMIC PROGRESS REPORT").FontSize(11).Bold().FontColor(Brown);
                        col.Item().Text($"Academic Year: {DateTime.Today.Year}").FontSize(8).FontColor(Grey);
                        col.Item().Text($"Generated: {DateTime.Today:MMM dd, yyyy}").FontSize(8).FontColor(Grey);
                    });
                });

                page.Content().PaddingTop(14).Column(col =>
                {
                    col.Spacing(14);

                    // ---- Student box ----
                    col.Item().Background(LightGrey).Padding(12).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("STUDENT NAME").FontSize(7).FontColor(Grey);
                            c.Item().Text(s.FullName).Bold();
                            c.Item().PaddingTop(6).Text("GRADE LEVEL").FontSize(7).FontColor(Grey);
                            c.Item().Text(string.IsNullOrWhiteSpace(s.Section) ? s.Grade : $"{s.Grade} (Section {s.Section})");
                        });
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("STUDENT ID").FontSize(7).FontColor(Grey);
                            c.Item().Text(s.StudentId);
                            c.Item().PaddingTop(6).Text("GUARDIAN").FontSize(7).FontColor(Grey);
                            c.Item().Text(s.GuardianName);
                        });
                    });

                    // ---- Results table ----
                    col.Item().Text("Academic Performance").FontSize(12).Bold();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2.2f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(0.8f);
                            columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            foreach (var title in new[] { "SUBJECT", "MONTHLY TEST", "TERM EXAM", "FINAL SCORE", "GRADE", "TEACHER REMARKS" })
                            {
                                header.Cell().Background(Brown).Padding(6).Text(title).FontSize(7).Bold().FontColor("#FFFFFF");
                            }
                        });

                        foreach (var r in report.Rows)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor("#E7E1DA").Padding(6).Text(r.Subject).Bold();
                            table.Cell().BorderBottom(0.5f).BorderColor("#E7E1DA").Padding(6).AlignCenter().Text(r.MonthlyTest.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor("#E7E1DA").Padding(6).AlignCenter().Text(r.TermExam.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor("#E7E1DA").Padding(6).AlignCenter().Text(r.FinalScore.ToString()).Bold();
                            table.Cell().BorderBottom(0.5f).BorderColor("#E7E1DA").Padding(6).AlignCenter().Text(r.Grade).Bold().FontColor(Brown);
                            table.Cell().BorderBottom(0.5f).BorderColor("#E7E1DA").Padding(6).Text(r.Remarks).FontSize(8).FontColor(Grey);
                        }

                        if (report.Rows.Count == 0)
                        {
                            table.Cell().ColumnSpan(6).Padding(6).Text("No marks recorded yet.").FontColor(Grey);
                        }
                    });

                    // ---- Cambridge results ----
                    if (report.CambridgeResults.Count > 0)
                    {
                        col.Item().Text("Cambridge Examination Results").FontSize(12).Bold();
                        foreach (var c in report.CambridgeResults)
                        {
                            col.Item().Text($"{c.SubjectName} - {c.ExamName}: Grade {c.Grade}");
                        }
                    }

                    // ---- Summary ----
                    col.Item().Text("Summary Statistics").FontSize(12).Bold();
                    col.Item().Row(row =>
                    {
                        row.Spacing(10);
                        row.RelativeItem().Background(LightGrey).Padding(10).Column(c =>
                        {
                            c.Item().Text("CUMULATIVE GPA").FontSize(7).FontColor(Grey);
                            c.Item().Text(report.Rows.Count == 0 ? "-" : report.Gpa.ToString("0.00")).FontSize(16).Bold();
                        });
                        row.RelativeItem().Background(LightGrey).Padding(10).Column(c =>
                        {
                            c.Item().Text("TOTAL SCORE").FontSize(7).FontColor(Grey);
                            c.Item().Text(report.Rows.Count == 0 ? "-" : $"{report.TotalPercent:0.0}%").FontSize(16).Bold();
                        });
                        row.RelativeItem().Background(LightGrey).Padding(10).Column(c =>
                        {
                            c.Item().Text("ATTENDANCE").FontSize(7).FontColor(Grey);
                            c.Item().Text(report.DaysTotal == 0 ? "-" : $"{report.DaysPresent}/{report.DaysTotal} days").FontSize(16).Bold();
                        });
                    });

                    // ---- Signatures ----
                    col.Item().PaddingTop(30).Row(row =>
                    {
                        foreach (var (name, role) in new[] { ("M. Jennifer Clasanciya", "Managing Director"), ("", "Class Teacher"), ("", "Parent / Guardian") })
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().BorderTop(0.5f).BorderColor(Grey).PaddingTop(4).AlignCenter().Text(name).FontSize(8);
                                c.Item().AlignCenter().Text(role).FontSize(7).FontColor(Grey);
                            });
                        }
                    });
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Lumora Educational Institute  •  Page ").FontSize(7).FontColor(Grey);
                    t.CurrentPageNumber().FontSize(7).FontColor(Grey);
                });
            });
        });
    }
}
