using System.Linq.Expressions;
using Bison.Razor.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(); // tilføjede razor pages til programmet

var envPath  = Environment.GetEnvironmentVariable("BISONDBPATH");   // kan måske være null
var tempPath = Path.Combine(Path.GetTempPath(), "bison.db");        // fallback
var dbPath   = envPath ?? tempPath;

builder.Services.AddDbContext<BisonDBContext>(options => options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<IObservationService, ObservationService>(); // hvis man kalder dette med en interface giver den en instance as observationservice.
builder.Services.AddSingleton(new DBFacade(dbPath)); //vis den kalder den med en dbfacade

var app = builder.Build(); //building the webapplication itself (metadata)

using (var scope = app.Services.CreateScope())
{
    var context  = scope.ServiceProvider.GetRequiredService<BisonDBContext>();
    context.Database.EnsureCreated(); //opretter kun database, hvis den slet ikke findes
    DbInitializer.SeedDatabase(context);
}

app.MapRazorPages(); // kobler URL til razor pages
app.UseStaticFiles(); // kobler browseren til filerne i wwwroot -> altså css styling osv så det ikke bare er tekst

app.Run();

// Gør den (ellers implicit genererede) Program-klasse offentlig og tilgængelig, så
// WebApplicationFactory<Program> kan bruges fra Bison.Razor.Tests til at køre servicen in-memory.
public partial class Program { }
