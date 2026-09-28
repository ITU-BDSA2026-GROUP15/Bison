using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Bison.Razor.Tests;

// Starter Bison.Razor in-memory med en KENDT testdatabase, bygget ud fra
// data/schema.sql + data/dump.sql, i stedet for bison.db i temp-mappen (eller BISONDBPATH).
public class TestWebFactory : WebApplicationFactory<Program>
{
    // Stien til denne instans' egen testdatabase (unik temp-fil).
    private readonly string _testDbPath;

    // Bygger testdatabasen, før appen startes.
    public TestWebFactory()
    {
    }

    // Erstatter den DBFacade, Program.cs registrerer, med én der peger på testdatabasen.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
    }

    // Kører schema.sql og dump.sql mod testdatabasen (svarer til scripts/initDB.sh).
    private void CreateTestDatabase()
    {
    }

    // Finder den mappe der indeholder data/schema.sql.
    // Testene går én mappe op ad gangen, indtil vi finder data/schema.sql.
    private static string FindDataRoot()
    {
        // AppContext.BaseDirectory = mappen hvor test-DLL'en ligger (bin/Debug/net8.0/)
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        // Gå op, så længe vi ikke er ved roden af drevet (dir != null)
        // og den nuværende mappe ikke indeholder data/schema.sql.
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "data", "schema.sql")))
        {
            dir = dir.Parent;
        }

        // Nåede vi helt op til drevets rod uden at finde filen, er der noget galt med
        // mappestrukturen. Så kommer fejlbeskeden med en exception.
        if (dir is null)
            throw new InvalidOperationException(
                $"Kunne ikke finde data/schema.sql i nogen mappe over {AppContext.BaseDirectory}.");

        return dir.FullName;
    }

    // Stopper appen og sletter testdatabasen, når testene er færdige.
    protected override void Dispose(bool disposing)
    {
    }
}
