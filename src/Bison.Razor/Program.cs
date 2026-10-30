using System.Linq.Expressions;
using Bison.Core;
using Bison.Razor.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bison.Razor.Models;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(); // tilføjede razor pages til programmet

var envPath  = Environment.GetEnvironmentVariable("BISONDBPATH");   // kan måske være null
var tempPath = Path.Combine(Path.GetTempPath(), "bison.db");        // fallback
var dbPath   = envPath ?? tempPath;

builder.Services.AddDbContext<BisonDBContext>(options => options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<IFilterService, FilterService>();

builder.Services.AddScoped<IObservationService, ObservationService>(); // hvis man kalder dette med en interface giver den en instance as observationservice.
builder.Services.AddSingleton(new DBFacade(dbPath)); //vis den kalder den med en dbfacade

builder.Services.AddDefaultIdentity<Bison.Razor.Models.ApplicationUser>(options => 
{
options.SignIn.RequireConfirmedAccount = false;

options.User.RequireUniqueEmail = true;
options.Password.RequiredLength = 6;

options.Password.RequiredUniqueChars = 1;
options.Password.RequireNonAlphanumeric = false;
options.Password.RequireLowercase = false;
options.Password.RequireUppercase = false;
options.Password.RequireDigit = false;


})
.AddEntityFrameworkStores<BisonDBContext>();




var app = builder.Build(); //building the webapplication itself (metadata)

using (var scope = app.Services.CreateScope())
{
    var context  = scope.ServiceProvider.GetRequiredService<BisonDBContext>();
    context.Database.EnsureCreated(); //opretter kun database, hvis den slet ikke findes
    DbInitializer.SeedDatabase(context);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Bison.Razor.Models.ApplicationUser>>();
    await CreateUser (userManager, "edka@itu.dk", "123456");
    await CreateUser (userManager, "debloh@itu.dk", "password");

}
app.UseStaticFiles(); // kobler browseren til filerne i wwwroot -> altså css styling osv så det ikke bare er tekst
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages(); // kobler URL til razor pages

app.Run();

static async Task CreateUser(UserManager<Bison.Razor.Models.ApplicationUser> userManager, string email, string password)
{
   if (await userManager.FindByEmailAsync(email) != null) return;

   var result = await userManager.CreateAsync(new Bison.Razor.Models.ApplicationUser { UserName = email, Email = email }, password);
if (!result.Succeeded)
{
    throw new Exception( string.Join(", ", result.Errors.Select(e => e.Description)));
}
}

// Gør den (ellers implicit genererede) Program-klasse offentlig og tilgængelig, så
// WebApplicationFactory<Program> kan bruges fra Bison.Razor.Tests til at køre servicen in-memory.
public partial class Program { }
