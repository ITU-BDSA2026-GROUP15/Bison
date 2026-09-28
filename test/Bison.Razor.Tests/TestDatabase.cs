using Microsoft.Data.Sqlite;

namespace Bison.Razor.Tests;

// En midlertidig SQLite-testdatabase med KENDTE data, bygget ud fra data/schema.sql + data/dump.sql.
// Bruges både af TestWebFactory (API-tests) og direkte af IntegrationTests (uden webapp).
public class TestDatabase : IDisposable
{
    // Stien til denne instans' egen testdatabase (unik temp-fil).
    // Public, så TestWebFactory og testene kan lave en DBFacade, der peger på den.
    public string DbPath { get; }

    public TestDatabase()
    {
        // Path.GetTempPath() = brugerens temp-mappe (fx C:\Users\<navn>\AppData\Local\Temp\ eller /tmp/).
        // Guid.NewGuid() giver et unikt ID, så to testklasser, der kører samtidig, aldrig
        // deler (eller overskriver) samme databasefil.
        DbPath = Path.Combine(Path.GetTempPath(), $"bison_test_{Guid.NewGuid()}.db");

        CreateTestDatabase();
    }

    // Kører schema.sql og dump.sql mod testdatabasen.
    private void CreateTestDatabase()
    {
        // FindDataRoot() returnerer mappen, der INDEHOLDER data/ - derfor lægger vi "data" på selv.
        string dataDir = Path.Combine(FindDataRoot(), "data");

        // "Data Source=<sti>" fortæller SQLite hvilken fil der skal bruges.
        // Findes filen ikke, opretter SQLite den automatisk, når forbindelsen åbnes.
        // 'using' sørger for, at forbindelsen lukkes igen, når metoden er færdig.
        using var connection = new SqliteConnection($"Data Source={DbPath}");
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

    // Sletter testdatabasen, når den ikke skal bruges mere.
    public void Dispose()
    {
        // Microsoft.Data.Sqlite genbruger forbindelser og holder derfor filen
        // åben i baggrunden. På Windows kan en åben fil ikke slettes, så vi lukker dem først.
        SqliteConnection.ClearAllPools();

        // Slet testdatabasen, så der ikke hober sig bison_test_<guid>.db-filer op i temp-mappen.
        if (File.Exists(DbPath))
        {
            File.Delete(DbPath);
        }
    }
}
