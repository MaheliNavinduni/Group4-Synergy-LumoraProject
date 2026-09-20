using LumoraAcademy.Core;

namespace LumoraAcademy.Tests;

// Creates a fresh database file for each test so tests never affect each other.
// Dispose() deletes the file afterwards.
public class TestBackend : IDisposable
{
    public Backend Backend { get; }
    private readonly string _path;

    public TestBackend(bool seedDemoData = true)
    {
        _path = Path.Combine(Path.GetTempPath(), "lumora-test-" + Guid.NewGuid().ToString("N") + ".db");
        Backend = new Backend(_path, seedDemoData);
    }

    public void Dispose()
    {
        Backend.Database.Connection.Close();
        if (File.Exists(_path)) File.Delete(_path);
    }
}
