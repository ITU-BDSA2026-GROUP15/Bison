using System.Diagnostics;
using System.Linq;
using SimpleDB;

namespace Bison.CLI.Tests;

public class EndToEnd
{
    private const string ExampleDataFile = "E2ETest.csv";
    private const string ObserveFile = "bison_observe_cli_db.csv";

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

    [Fact]
    public void BisonPrintsCorrectOutput()
    {
        // ARRANGE
        string? original = File.Exists(ObserveFile) ? File.ReadAllText(ObserveFile) : null;
        string exampleData = File.ReadAllText(ExampleDataFile);
        File.WriteAllText(ObserveFile, exampleData);

        // OBS: Klokkeslættene er beregnet ud fra dansk tidszone.
        // Testen antager derfor, at den kører på en maskine sat til dansk tid.
        // Hvis testen begynder at fejle i GitHub Actions (som typisk kører UTC),
        // er det højst sandsynligt derfor
        string[] forventedeOutputLinjer =
        {
            "ROPF @ 08-01-23 14:09:20: (0) A bird at DR Byen",
            "ADHO @ 08-02-23 14:19:38: (1) I think that is a Heron at DR Byen",
            "EDKA @ 08-02-23 14:37:38: (2) Ardea cinerea at DR Byen",
            "LIBRNI @ 09-04-26 20:03:12: (3) Test"
        };

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
        startInfo.ArgumentList.Add("read");

        try
        {
            // ACT
            using var process = Process.Start(startInfo)!;
            string actualOutput = process.StandardOutput.ReadToEnd();
            string errorOutput = process.StandardError.ReadToEnd();
            process.WaitForExit();

            // Midlertidig diagnostik - fjern igen når fejlen er fundet
            Console.WriteLine("STDOUT:\n" + actualOutput);
            Console.WriteLine("STDERR:\n" + errorOutput);

            // ASSERT
            Assert.Equal(0, process.ExitCode);
            foreach (string linje in forventedeOutputLinjer)
            {
                Assert.Contains(linje, actualOutput);
            }
        }
        finally
        {
            // CLEANUP
            if (original is null)
                File.Delete(ObserveFile);
            else
                File.WriteAllText(ObserveFile, original);
        }
    }

    [Fact]
    public void BisonStoresValueInDatabase()
    {
        // ARRANGE
        string? original = File.Exists(ObserveFile) ? File.ReadAllText(ObserveFile) : null;
        if (File.Exists(ObserveFile))
            File.Delete(ObserveFile);

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
        startInfo.ArgumentList.Add("observe");
        startInfo.ArgumentList.Add("Penguin");

        try
        {
            // ACT
            using var process = Process.Start(startInfo)!;
            string actualOutput = process.StandardOutput.ReadToEnd();
            string errorOutput = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine("STDOUT:\n" + actualOutput);
            Console.WriteLine("STDERR:\n" + errorOutput);

            // ASSERT
            Assert.Equal(0, process.ExitCode);

            var db = CSVDatabase<Cheep>.Instance;
            var cheeps = db.Read(ObserveFile).ToList();

            Assert.Contains(cheeps, c => c.Observation == "Penguin");
        }
        finally
        {
            // CLEANUP
            if (original is null)
                File.Delete(ObserveFile);
            else
                File.WriteAllText(ObserveFile, original);
        }
    }
}
