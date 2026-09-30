using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Bison.Razor.Tests;

// Starter Bison.Razor in-memory med en KENDT testdatabase, bygget ud fra
// data/schema.sql + data/dump.sql.
// Selve databasen (oprettelse og sletning) ligger i TestDatabase - denne klasse sørger kun for
// at starte appen og få den til at bruge testdatabasen.
public class TestWebFactory : WebApplicationFactory<Program>
{
    private readonly TestDatabase _testDb = new();

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
            services.AddSingleton(new DBFacade(_testDb.DbPath));
        });
    }

    // Stopper appen og sletter testdatabasen, når testene er færdige.
    protected override void Dispose(bool disposing)
    {
        // Først appen (den bruger databasen), derefter databasen.
        base.Dispose(disposing);
        _testDb.Dispose();
    }
}
