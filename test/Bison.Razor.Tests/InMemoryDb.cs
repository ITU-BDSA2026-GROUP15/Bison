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