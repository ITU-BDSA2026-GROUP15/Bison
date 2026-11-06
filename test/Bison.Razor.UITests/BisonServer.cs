using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Bison.Razor.UITests;

// [SetUpFixture] betyder, at NUnit kører [OneTimeSetUp] én gang før alle tests i dette namespace
// og [OneTimeTearDown] én gang efter dem. Bison skal altså kun starte én gang pr. testkørsel.
//
// Bison kører som sin egen proces, fordi browseren skal bruge en rigtig netværksport.
// WebApplicationFactory (som i Bison.Razor.Tests) kører appen uden en port, så Chromium ikke kan nå den.
[SetUpFixture]
public class BisonServer
{
    public static string BaseUrl { get; private set; } = "";

    private static readonly TimeSpan StartupTimeout = TimeSpan.FromSeconds(60);

    private readonly StringBuilder _serverOutput = new();
    private Process? _process;
    private string? _databaseFolder;

    [OneTimeSetUp]
    public async Task StartAsync()
    {
        // Testene får deres egen database, så de aldrig rører din lokale bison.db.
        _databaseFolder = Path.Combine(Path.GetTempPath(), "bison-uitests-" + Guid.NewGuid());
        Directory.CreateDirectory(_databaseFolder);

        BaseUrl = $"http://127.0.0.1:{FindFreePort()}";

        var projectFolder = Path.Combine(FindRepositoryRoot(), "src", "Bison.Razor");
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = projectFolder,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        // --no-build: testprojektet har allerede bygget Bison (se ProjectReference i .csproj).
        // --no-launch-profile: ellers ville launchSettings.json bestemme porten i stedet for os.
        foreach (var argument in new[] { "run", "--no-build", "--no-launch-profile", "--configuration", BuildConfiguration })
        {
            startInfo.ArgumentList.Add(argument);
        }

        startInfo.Environment["ASPNETCORE_URLS"] = BaseUrl;
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        startInfo.Environment["BISONDBPATH"] = Path.Combine(_databaseFolder, "bison.db");

        _process = new Process { StartInfo = startInfo };
        _process.OutputDataReceived += (_, e) => RememberOutput(e.Data);
        _process.ErrorDataReceived += (_, e) => RememberOutput(e.Data);
        _process.Start();

        // Outputtet skal læses løbende, ellers kan processen gå i stå, når dens buffer er fuld.
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

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

        if (_databaseFolder is not null && Directory.Exists(_databaseFolder))
        {
            Directory.Delete(_databaseFolder, recursive: true);
        }
    }

    private async Task WaitUntilServerRespondsAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < StartupTimeout)
        {
            if (_process!.HasExited)
            {
                throw new InvalidOperationException($"Bison stoppede under opstart:\n{ServerOutput()}");
            }

            try
            {
                var response = await client.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Serveren lytter ikke endnu, så vi prøver igen.
            }
            catch (TaskCanceledException)
            {
                // Svaret tog for lang tid, så vi prøver igen.
            }

            await Task.Delay(250);
        }

        throw new TimeoutException($"Bison svarede ikke inden for {StartupTimeout.TotalSeconds} sekunder:\n{ServerOutput()}");
    }

    private void RememberOutput(string? line)
    {
        if (line is null)
        {
            return;
        }

        // Hændelserne kommer fra andre tråde, og StringBuilder tåler ikke samtidig brug.
        lock (_serverOutput)
        {
            _serverOutput.AppendLine(line);
        }
    }

    private string ServerOutput()
    {
        lock (_serverOutput)
        {
            return _serverOutput.ToString();
        }
    }

    // Port 0 får styresystemet til at vælge en ledig port, så testene ikke støder ind i en Bison,
    // du allerede har kørende på 5273.
    private static int FindFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string FindRepositoryRoot()
    {
        var folder = new DirectoryInfo(AppContext.BaseDirectory);

        while (folder is not null && !File.Exists(Path.Combine(folder.FullName, "Bison.sln")))
        {
            folder = folder.Parent;
        }

        return folder?.FullName ?? throw new DirectoryNotFoundException("Kunne ikke finde Bison.sln.");
    }

    // Bison skal køre i samme konfiguration som testene: Debug i PR-workflowet og Release i release-workflowet.
    private const string BuildConfiguration =
#if DEBUG
        "Debug";
#else
        "Release";
#endif
}
