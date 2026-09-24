using Microsoft.Extensions.Logging;

namespace LumoraAcademy;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Windows draws its own box around every Entry. We remove it so that
		// only our own rounded border from the design is visible.
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoNativeBorder", (handler, view) =>
		{
#if WINDOWS
			handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
			handler.PlatformView.Background = null;
			handler.PlatformView.Padding = new Microsoft.UI.Xaml.Thickness(0);
#endif
		});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
