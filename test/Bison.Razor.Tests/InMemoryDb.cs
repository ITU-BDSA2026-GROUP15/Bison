using Bison.Core.Models;
using Bison.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore; 
using Bison.Web;                                       
using Microsoft.AspNetCore.Mvc.Testing;                   
using Microsoft.AspNetCore.TestHost;                      
using Microsoft.Extensions.DependencyInjection;           
using Microsoft.Extensions.DependencyInjection.Extensions; 

namespace Bison.Razor.Tests;

public class InMemoryDb : IDisposable
{
    private readonly SqliteConnection _connection;
    private WebApplicationFactory<Program>? _webApp;
    public DbContextOptions<BisonDBContext> Options { get; }

    public InMemoryDb()
    {
        // Databasen findes kun, så længe denne forbindelse er åben.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        // Vi giver EF selve forbindelsen, så alle contexts deler samme database.
        Options = new DbContextOptionsBuilder<BisonDBContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateContext();
        context.Database.Migrate(); // samme skema som den rigtige app
    }

    public BisonDBContext CreateContext() => new BisonDBContext(Options);

    // Fælles testdata for ApiTests og IntegrationTests: Johan og Yuki med én observation hver.
    // Ændres teksterne her, skal asserts i begge testklasser også rettes.
    // Skal kaldes FØR CreateWebApp(), ellers seeder Program.cs sine egne data.
    public void SeedJohanAndYuki()
    {
        using var context = CreateContext();

        // Fixturen deles af alle tests i en klasse, men constructoren kører for hver test.
        // Derfor seeder vi kun, hvis databasen er tom.
        if (context.Authors.Any()) return;

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

    // Starter webappen in-memory. Skal bruges for at kunne gøre ApiTests.cs og PaginationTests.cs uafhængig af forældede hjælpeklasser
   public WebApplicationFactory<Program> CreateWebApp()
    {
        // ??= betyder: lav kun en ny, hvis vi ikke allerede har en.
        _webApp ??= new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                // Fjern appens egen database-registrering
                services.RemoveAll<BisonDBContext>();
                services.RemoveAll<DbContextOptions<BisonDBContext>>();

                // registrér en database, der bruger vores åbne in-memory-forbindelse.
                services.AddDbContext<BisonDBContext>(o => o.UseSqlite(_connection));
            }));

        return _webApp;
    }

    public void Dispose()
    {
        // ? betyder: kald kun Dispose, hvis der faktisk blev lavet en webapp.
        // Ikke alle tests bygger en webapp så den er nødvendigt for at ikke crashe tests
        _webApp?.Dispose();
        _connection.Dispose();
    }
}