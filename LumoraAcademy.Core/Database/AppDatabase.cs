using LumoraAcademy.Core.Entities;
using SQLite;

namespace LumoraAcademy.Core.Database;

// Opens (or creates) the SQLite database file and makes sure all tables exist.
// Every service talks to the database through this class.
public class AppDatabase
{
    public SQLiteConnection Connection { get; }
    public string FilePath { get; }

    public AppDatabase(string filePath)
    {
        FilePath = filePath;

        string? folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(folder))
        {
            Directory.CreateDirectory(folder);
        }

        Connection = new SQLiteConnection(filePath);
        CreateTables();
    }

    private void CreateTables()
    {
        Connection.CreateTable<User>();
        Connection.CreateTable<Student>();
        Connection.CreateTable<Teacher>();
        Connection.CreateTable<Subject>();
        Connection.CreateTable<Payment>();
        Connection.CreateTable<ExamResult>();
        Connection.CreateTable<AttendanceEntry>();
        Connection.CreateTable<ProgressNote>();
        Connection.CreateTable<SchoolEvent>();
        Connection.CreateTable<ClassSession>();
    }

    // True when the database has no data yet (used to decide whether to seed).
    public bool IsEmpty()
    {
        return Connection.Table<User>().Count() == 0;
    }

    // Removes every row from every table. Used by tests and by "reset demo data".
    public void ClearAll()
    {
        Connection.DeleteAll<User>();
        Connection.DeleteAll<Student>();
        Connection.DeleteAll<Teacher>();
        Connection.DeleteAll<Subject>();
        Connection.DeleteAll<Payment>();
        Connection.DeleteAll<ExamResult>();
        Connection.DeleteAll<AttendanceEntry>();
        Connection.DeleteAll<ProgressNote>();
        Connection.DeleteAll<SchoolEvent>();
        Connection.DeleteAll<ClassSession>();
    }

    // Makes a simple backup copy of the database file (proposal objective 12).
    public string Backup(string backupFolder)
    {
        Directory.CreateDirectory(backupFolder);
        string target = Path.Combine(backupFolder, $"lumora-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db");
        Connection.Backup(target);
        return target;
    }
}
