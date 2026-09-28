using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

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
        // Path.GetTempPath() = brugerens temp-mappe (fx C:\Users\<navn>\AppData\Local\Temp\ eller /tmp/).
        // Guid.NewGuid() giver et unikt ID, så to testklasser, der kører samtidig, aldrig
        // deler (eller overskriver) samme databasefil.
        _testDbPath = Path.Combine(Path.GetTempPath(), $"bison_test_{Guid.NewGuid()}.db");

        CreateTestDatabase();
    }

    // Erstatter den DBFacade, Program.cs registrerer, med én der peger på testdatabasen.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // 'services' er listen over alt, der er registreret i DI-containeren. Hver registrering
            // er en ServiceDescriptor. Vi finder dem, der gælder typen DBFacade
            // (.ToList() laver en kopi, så vi ikke ændrer i listen, mens vi løber igennem den).
            var existing = services.Where(d => d.ServiceType == typeof(DBFacade)).ToList();

            // Fjern den, så appen ikke længere har en DBFacade, der peger på den rigtige bison.db.
            foreach (var descriptor in existing)
            {
                services.Remove(descriptor);
            }

            // Registrér en ny DBFacade, der peger på testdatabasen.
            services.AddSingleton(new DBFacade(_testDbPath));
        });
    }

    // Kører schema.sql og dump.sql mod testdatabasen.
    private void CreateTestDatabase()
    {
        // FindDataRoot() returnerer mappen, der INDEHOLDER data/ - derfor lægger vi "data" på selv.
        string dataDir = Path.Combine(FindDataRoot(), "data");

        // "Data Source=<sti>" fortæller SQLite hvilken fil der skal bruges.
        // Findes filen ikke, opretter SQLite den automatisk, når forbindelsen åbnes.
        // 'using' sørger for, at forbindelsen lukkes igen, når metoden er færdig.
        using var connection = new SqliteConnection($"Data Source={_testDbPath}");
        connection.Open();

        // Schema.sql opretter tabellerne (user, observation) og dump.sql indsætter rækker i dem.
        foreach (var sqlFile in new[] { "schema.sql", "dump.sql" })
        {
            using var command = connection.CreateCommand();

            // Hele filens indhold sendes som én kommando. Microsoft.Data.Sqlite kører selv
            // alle SQL-sætningerne efter hinanden (de er adskilt af ;).
            command.CommandText = File.ReadAllText(Path.Combine(dataDir, sqlFile));

            // "NonQuery" = en kommando, der ikke returnerer rækker (CREATE, INSERT, DROP ...)
            command.ExecuteNonQuery();
        }
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
        base.Dispose(disposing);

        SqliteConnection.ClearAllPools();

        // Slet testdatabasen, så der ikke hober sig bison_test_<guid>.db-filer op i temp-mappen.
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }
}
