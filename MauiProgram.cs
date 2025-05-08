using Microsoft.Extensions.Logging;
using EmbedIO;
using EmbedIO.Files;
using System.Diagnostics;

namespace Server;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		CopyFromRawToStaticFolder();
		StartWebServer();

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	private static void CopyFromRawToStaticFolder()
	{
		var fileName = "index.html";
		var targetFolder = Path.Combine(FileSystem.AppDataDirectory, "Static");
		var targetPath = Path.Combine(targetFolder, fileName);

		if (!Directory.Exists(targetFolder))
			Directory.CreateDirectory(targetFolder);

		using var stream = FileSystem.OpenAppPackageFileAsync(fileName).Result;
		if (stream == null)
			throw new FileNotFoundException("Could not open the embedded file: " + fileName);

		using var reader = new StreamReader(stream);
		var content = reader.ReadToEnd();
		File.WriteAllText(targetPath, content);
	}

	public static void StartWebServer()
	{
		Task.Run(() =>
		{
			try
			{
				var staticPath = Path.Combine(FileSystem.AppDataDirectory, "Static");

				if (!Directory.Exists(staticPath))
					Directory.CreateDirectory(staticPath);

				var indexPath = Path.Combine(staticPath, "index.html");
				if (!File.Exists(indexPath))
					File.WriteAllText(indexPath, "<html><body><h1>Files not found!</h1></body></html>");

				var server = new WebServer(o => o
						.WithUrlPrefix("http://127.0.0.1:9696/")
						.WithMode(HttpListenerMode.EmbedIO))
					.WithModule(new FileModule("/", new FileSystemProvider(staticPath, true))
					{
						DefaultDocument = "index.html"
					});

				server.StateChanged += (s, e) =>
					Debug.WriteLine($"[Server] State changed: {e.NewState}");

				server.RunAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Server error: " + ex.Message);
			}
		});
	}
}
