using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using SimpleDB;

namespace Bison.CLI.Tests;

// [Collection(...)] deler den samme kørende Bison.Razor-instans med UnitTest-klassen,
// i stedet for at starte endnu en service op.
[Collection("Bison.Razor service")]
public class EndToEnd
{
    private readonly HttpClient client;

    public EndToEnd(BisonRazorFixture fixture)
    {
        client = fixture.Client;
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

        return Path.Combine(dir.FullName, "src", "Bison.CLI", "Bison.CLI.csproj");
    }

    private static int RunCli(params string[] cliArgs)
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
        startInfo.ArgumentList.Add("--");
        foreach (string cliArg in cliArgs)
            startInfo.ArgumentList.Add(cliArg);

        using var process = Process.Start(startInfo)!;
        process.StandardOutput.ReadToEnd();
        process.StandardError.ReadToEnd();
        process.WaitForExit();

        return process.ExitCode;
    }

    [Fact]
    public void BisonPrintsCorrectOutput()
    {
        // ARRANGE
        // Guid i observationsteksten sikrer, at vi kan finde netop vores egen observation i
        // read()-outputtet, uanset hvad der ellers allerede ligger i den delte service-instans.
        string uniqueObservation = "A bird at DR Byen " + Guid.NewGuid();
        string location = "DR Byen";

        int observeExitCode = RunCli("observe", uniqueObservation, location);
        Assert.Equal(0, observeExitCode);

        string projectPath = FindProjectPath();

        var readStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        readStartInfo.ArgumentList.Add("run");
        readStartInfo.ArgumentList.Add("--project");
        readStartInfo.ArgumentList.Add(projectPath);
        readStartInfo.ArgumentList.Add("--");
        readStartInfo.ArgumentList.Add("read");

        // ACT
        using var readProcess = Process.Start(readStartInfo)!;
        string actualOutput = readProcess.StandardOutput.ReadToEnd();
        string errorOutput = readProcess.StandardError.ReadToEnd();
        readProcess.WaitForExit();

        // ASSERT
        Assert.Equal(0, readProcess.ExitCode);
        Assert.Contains(uniqueObservation, actualOutput);
        Assert.Contains(location, actualOutput);
    }

    [Fact]
    public void BisonStoresValueInDatabase()
    {
        // ARRANGE
        string uniqueObservation = "Penguin " + Guid.NewGuid();

        // ACT
        int exitCode = RunCli("observe", uniqueObservation, "ITU");

        // ASSERT
        Assert.Equal(0, exitCode);

        var observations = client.GetFromJsonAsync<List<Cheep>>("/observations").GetAwaiter().GetResult() ?? new List<Cheep>();
        Assert.Contains(observations, c => c.Observation == uniqueObservation); //stores new observation
    }
}
