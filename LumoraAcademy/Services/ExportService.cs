using System.Text;

namespace LumoraAcademy.Services;

// Saves a list of rows as a CSV file and opens it, so the office can
// keep a copy or print it from Excel.
public static class ExportService
{
    // headings: the first line of the file, e.g. "Student ID,Name,Grade"
    // rows:     one string list per line
    public static async Task SaveAndOpenCsvAsync(string fileName, IEnumerable<string> headings, IEnumerable<IEnumerable<string>> rows)
    {
        var text = new StringBuilder();
        text.AppendLine(string.Join(",", headings.Select(Escape)));

        foreach (var row in rows)
        {
            text.AppendLine(string.Join(",", row.Select(Escape)));
        }

        string folder = Path.Combine(FileSystem.AppDataDirectory, "Exports");
        Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, fileName);
        File.WriteAllText(path, text.ToString(), Encoding.UTF8);

        await Launcher.Default.OpenAsync(new OpenFileRequest(fileName, new ReadOnlyFile(path)));
    }

    // A value with a comma or a quote in it has to be wrapped, or the
    // columns would not line up when the file is opened.
    private static string Escape(string value)
    {
        value ??= "";

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        return value;
    }
}
