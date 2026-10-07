using System.Linq.Expressions;
using Bison.Razor.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(); // tilføjede razor pages til programmet

var envPath  = Environment.GetEnvironmentVariable("BISONDBPATH");   // kan måske være null
var tempPath = Path.Combine(Path.GetTempPath(), "bison.db");        // fallback
var dbPath   = envPath ?? tempPath;

//NEW CHECK FOR PATH IF IT DOESN'T EXIST YET
    if (!File.Exists(dbPath))
    {
        //makes a connection to the sqlite file on the dbPath
        using var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
        //lets us edit and send SQL to the database
        conn.Open();

        //runs through the two sql files, first schema, then dump
        foreach (var file in new[] { "schema.sql", "dump.sql" })
        {
            using var cmd = conn.CreateCommand();
            //reads everything from data-folder and use it as SQL-text
            cmd.CommandText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data", file));
            //non-quer means we don't expect rows returned, but only updated or inserted
            cmd.ExecuteNonQuery();
        }
    }

builder.Services.AddDbContext<PostContext>(options => options.UseSqlite(dbPath));
builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<IObservationService, ObservationService>(); // hvis man kalder dette med en interface giver den en instance as observationservice.
builder.Services.AddSingleton(new DBFacade(dbPath)); //vis den kalder den med en dbfacade

var app = builder.Build(); //building the webapplication itself (metadata)

app.MapRazorPages(); // kobler URL til razor pages
app.UseStaticFiles(); // kobler browseren til filerne i wwwroot -> altså css styling osv så det ikke bare er tekst

app.Run();

// Gør den (ellers implicit genererede) Program-klasse offentlig og tilgængelig, så
// WebApplicationFactory<Program> kan bruges fra Bison.Razor.Tests til at køre servicen in-memory.
public partial class Program { }
