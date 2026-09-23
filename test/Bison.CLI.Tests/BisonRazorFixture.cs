using System.Diagnostics;
using System.Net.Http.Json;

namespace Bison.CLI.Tests;

// Starter Bison.Razor (web servicen) som en rigtig proces, én gang, delt mellem alle tests
// i en collection (test-klasse).
public class BisonRazorFixture : IDisposable
{
    public HttpClient Client { get; }

    private readonly Process process;

    public BisonRazorFixture()
    {
        string projectPath = FindProjectPath();

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--urls");
        startInfo.ArgumentList.Add("http://localhost:5273");

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
                    return;
            }
            catch (HttpRequestException)
            {
                // Servicen lytter endnu ikke - prøv igen.
            }

            Thread.Sleep(500);
        }

        throw new InvalidOperationException("Bison.Razor startede ikke inden for den forventede tid.");
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

        return Path.Combine(dir.FullName, "src", "Bison.Razor", "Bison.Razor.csproj");
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
    }
}

[CollectionDefinition("Bison.Razor service")]
public class BisonRazorCollection : ICollectionFixture<BisonRazorFixture>
{
}
