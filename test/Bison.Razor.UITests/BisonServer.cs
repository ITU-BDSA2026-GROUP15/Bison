using System.Diagnostics;

namespace Bison.Razor.UITests;

// [SetUpFixture] betyder, at NUnit kører [OneTimeSetUp] én gang før alle tests i dette namespace
// og [OneTimeTearDown] én gang efter dem. Bison skal altså kun starte én gang pr. testkørsel.
//
// Bison kører som sin egen proces, fordi browseren skal bruge en rigtig netværksport.
// WebApplicationFactory (som i Bison.Razor.Tests) kører appen uden en port, så Chromium ikke kan nå den.
[SetUpFixture]
public class BisonServer
{
    public const string BaseUrl = "http://127.0.0.1:5299";

    // Bison skal køre i samme konfiguration som testene: Debug i PR-workflowet og Release i release-workflowet.
#if DEBUG
    private const string BuildConfiguration = "Debug";
#else
    private const string BuildConfiguration = "Release";
#endif

    private Process? _process;
    private string? _databaseFolder;

    [OneTimeSetUp]
    public async Task StartAsync()
    {
        // Testene får deres egen database, så de aldrig rører din lokale bison.db.
        _databaseFolder = Path.Combine(Path.GetTempPath(), "bison-uitests-" + Guid.NewGuid());
        Directory.CreateDirectory(_databaseFolder);

        // --no-build: testprojektet har allerede bygget Bison (se ProjectReference i .csproj).
        // --no-launch-profile: ellers ville launchSettings.json bestemme porten i stedet for os.
        var startInfo = new ProcessStartInfo("dotnet", $"run --no-build --no-launch-profile --configuration {BuildConfiguration}")
        {
            WorkingDirectory = Path.Combine(FindRepositoryRoot(), "src", "Bison.Razor")
        };
        startInfo.Environment["ASPNETCORE_URLS"] = BaseUrl;
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        startInfo.Environment["BISONDBPATH"] = Path.Combine(_databaseFolder, "bison.db");

        _process = Process.Start(startInfo);

        await WaitUntilServerRespondsAsync();
    }

    [OneTimeTearDown]
    public void Stop()
    {
        if (_process is { HasExited: false })
        {
            // entireProcessTree, fordi 'dotnet run' starter selve Bison som en underproces.
            _process.Kill(entireProcessTree: true);
            _process.WaitForExit();
        }

        _process?.Dispose();

        if (_databaseFolder is not null)
        {
            Directory.Delete(_databaseFolder, recursive: true);
        }
    }

    // 'dotnet run' vender tilbage med det samme, så vi spørger, indtil Bison er klar til at svare.
    private async Task WaitUntilServerRespondsAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var deadline = DateTime.UtcNow.AddSeconds(60);

        while (DateTime.UtcNow < deadline)
        {
            if (_process!.HasExited)
            {
                throw new InvalidOperationException("Bison stoppede under opstart. Er port 5299 optaget? Kør 'dotnet run' i src/Bison.Razor for at se fejlen.");
            }

            try
            {
                var response = await client.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
            {
                // Bison svarer ikke endnu, så vi prøver igen.
            }

            await Task.Delay(250);
        }

        throw new TimeoutException("Bison svarede ikke inden for 60 sekunder.");
    }

    // Testene kører fra bin/Debug/net8.0, så vi går op gennem mapperne, til vi finder roden af repoet.
    private static string FindRepositoryRoot()
    {
        var folder = new DirectoryInfo(AppContext.BaseDirectory);

        while (folder is not null && !File.Exists(Path.Combine(folder.FullName, "Bison.sln")))
        {
            folder = folder.Parent;
        }

        return folder?.FullName ?? throw new DirectoryNotFoundException("Kunne ikke finde Bison.sln.");
    }
}
