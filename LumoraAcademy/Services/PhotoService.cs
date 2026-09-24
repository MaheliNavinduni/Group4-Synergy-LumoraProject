namespace LumoraAcademy.Services;

// Lets the user pick a JPG/PNG from the computer and copies it into the app's Photos folder,
// so the picture is still there even if the original file is moved or deleted.
public static class PhotoService
{
    private const long MaxBytes = 2 * 1024 * 1024;   // 2 MB, as written on the upload box

    // Returns the saved file's full path, or null if the user cancelled or the file was rejected.
    public static async Task<string?> PickAndSaveAsync(string prefix)
    {
        var options = new PickOptions
        {
            PickerTitle = "Choose a photo",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png" } },
            }),
        };

        FileResult? picked = await FilePicker.Default.PickAsync(options);
        if (picked == null) return null;

        string extension = Path.GetExtension(picked.FileName).ToLower();
        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
        {
            await ShowAsync("Please choose a JPG or PNG image.");
            return null;
        }

        using var source = await picked.OpenReadAsync();
        if (source.Length > MaxBytes)
        {
            await ShowAsync("The photo must be 2 MB or smaller.");
            return null;
        }

        string folder = Path.Combine(FileSystem.AppDataDirectory, "Photos");
        Directory.CreateDirectory(folder);

        string target = Path.Combine(folder, $"{prefix}-{Guid.NewGuid():N}{extension}");
        using (var dest = File.Create(target))
        {
            await source.CopyToAsync(dest);
        }

        return target;
    }

    private static Task ShowAsync(string message)
    {
        var page = Application.Current?.MainPage;
        return page == null ? Task.CompletedTask : page.DisplayAlert("Photo", message, "OK");
    }
}
