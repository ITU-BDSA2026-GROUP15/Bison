using Bison.Infrastructure.Data;   
using Microsoft.Data.Sqlite;      
using Microsoft.EntityFrameworkCore; 

namespace Bison.Razor.Tests;

// Én fælles in-memory testdatabase. Lever så længe objektet lever.
public class InMemoryDb : IDisposable
{
    private readonly SqliteConnection _connection;
    public DbContextOptions<BisonDBContext> Options { get; }

    public InMemoryDb()
    {
        // Databasen findes kun, så længe denne forbindelse er åben.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        // Vi giver EF selve forbindelsen (ikke strengen), så alle contexts deler samme database.
        Options = new DbContextOptionsBuilder<BisonDBContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateContext();
        context.Database.Migrate(); // samme skema som den rigtige app
    }

    public BisonDBContext CreateContext() => new BisonDBContext(Options);

    public SqliteConnection Connection => _connection; // til TestWebFactory

    public void Dispose() => _connection.Dispose(); // lukker = databasen forsvinder
}