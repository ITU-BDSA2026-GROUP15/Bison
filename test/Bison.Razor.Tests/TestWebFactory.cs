using Bison.Razor.DAL;
using Bison.Razor.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bison.Razor.Tests;

public class TestWebFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public TestWebFactory()
    {
        // Databasen eksisterer, så længe forbindelsen er åben.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<BisonDBContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new BisonDBContext(options);
        context.Database.EnsureCreated();

        var johan = new Author
        {
            Name = "Johan",
            Email = "johan@example.test"
        };

        var yuki = new Author
        {
            Name = "Yuki",
            Email = "yuki@example.test"
        };

        var taxon = new Taxon
        {
            dwc_TaxonID = "test-taxon",
            VernacularName = "Testfugl"
        };

        context.Observations.AddRange(
            new Observation
            {
                Author = johan,
                Taxon = taxon,
                Text = "Great Egret on the pond at the edge of town. Nests in colonies, often in trees near water.",
                TimeStamp = new DateTime(2026, 1, 2, 12, 0, 0)
            },
            new Observation
            {
                Author = yuki,
                Taxon = taxon,
                Text = "A Least Bittern resting on a sandbank. Stays within cover almost all the time.",
                TimeStamp = new DateTime(2026, 1, 1, 12, 0, 0)
            });

        context.SaveChanges();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<BisonDBContext>();
            services.RemoveAll<DbContextOptions<BisonDBContext>>();

            services.AddDbContext<BisonDBContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
/*using Microsoft.AspNetCore.Hosting;
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
*/