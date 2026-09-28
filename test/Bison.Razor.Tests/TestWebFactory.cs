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

    // Finder mappen med Bison.slnx, så vi kan finde data/-mappen, men sagde Nanna ikke noget med at slnx-filen skulle væk??
    private static string FindSolutionRoot()
    {
        throw new NotImplementedException();
    }

    // Stopper appen og sletter testdatabasen, når testene er færdige.
    protected override void Dispose(bool disposing)
    {
    }
}
