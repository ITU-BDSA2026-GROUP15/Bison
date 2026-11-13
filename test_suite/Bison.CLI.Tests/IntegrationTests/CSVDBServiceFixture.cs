using System.Diagnostics;
using System.Net.Http.Json;

namespace Bison.CLI.Tests;

// Starter web servicen som en rigtig proces, én gang, delt mellem alle tests
// i en collection (test-klasse).
public class CSVDBServiceFixture : IDisposable
{
    public HttpClient Client { get; }

    private readonly Process process;
    private readonly string tempDir = Directory.CreateTempSubdirectory("bison-test").FullName;

    public CSVDBServiceFixture()
    {

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(FindProjectPath());

        //alt efter -- sendes videre til selve webservicen
        startInfo.ArgumentList.Add("--");
        startInfo.ArgumentList.Add("--urls");
        startInfo.ArgumentList.Add("http://localhost:5273");
        startInfo.ArgumentList.Add("--DataDir");
        startInfo.ArgumentList.Add(tempDir);

        process = Process.Start(startInfo)!;

        Client = new HttpClient { BaseAddress = new Uri("http://localhost:5273") };

        WaitUntilReady();
    }

    // Poller servicen, indtil den rent faktisk svarer, i stedet for en fast Thread.Sleep.
    private void WaitUntilReady()
    {
        const int maxAttempts = 40;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                var response = Client.GetAsync("/observations").GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
                if (!response.IsSuccessStatusCode)
                {
                    var body = response.Content.ReadAsStringAsync()
                        .GetAwaiter().GetResult();
                }
            }
            catch (HttpRequestException)
            {
                // Servicen lytter endnu ikke - prøv igen.
            }

            Thread.Sleep(500);
        }

        throw new InvalidOperationException("The webservice did not launch in expected time frame.");
    }

    private static string FindProjectPath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !dir.GetFiles("*.slnx").Any())
        {
            dir = dir.Parent;
        }

        if (dir is null)
            throw new InvalidOperationException("Kunne ikke finde solution-roden.");

        return Path.Combine(dir.FullName, "src", "Bison.CSVDBService", "Bison.CSVDBService.csproj");
    }

    public void Dispose()
    {
        Client.Dispose();

        if (!process.HasExited)
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
        }

        process.Dispose();

        try { Directory.Delete(tempDir, true); } catch { /* ligegyldigt */ }
    
    }
}

[CollectionDefinition("Webservice")]
public class CSVDBServiceCollection : ICollectionFixture<CSVDBServiceFixture>
{
}
