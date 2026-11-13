using Bison.Infrastructure.Data;
using Bison.Infrastructure.Repositories;
using Bison.Infrastructure.Services;
using Bison.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(o =>
{
    o.Conventions.AddPageRoute("/Public", "/");
}); // tilføjede razor pages til programmet

var envPath  = Environment.GetEnvironmentVariable("BISONDBPATH");   // kan måske være null
var tempPath = Path.Combine(Path.GetTempPath(), "bison.db");        // fallback
var dbPath   = envPath ?? tempPath;

builder.Services.AddDbContext<BisonDBContext>(options => options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<IFilterService, FilterService>();

builder.Services.AddScoped<IObservationService, ObservationService>(); // hvis man kalder dette med en interface giver den en instance as observationservice.
builder.Services.AddSingleton(new DBFacade(dbPath)); //vis den kalder den med en dbfacade

//when a page asks for a IPictureStorage, it gets a LocalPictureStorage in wwwroot/uploads
builder.Services.AddScoped<IObservationService, ObservationService>();
builder.Services.AddScoped<IPictureStorage, LocalPictureStorage>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
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

var gitHubClientId = builder.Configuration["authentication:github:clientId"];
var gitHubClientSecret = builder.Configuration["authentication:github:clientSecret"];

// GitHub-login kræver nøgler, som kun findes i user secrets eller på Azure. Uden denne betingelse
// giver sider med login 500-fejl, hvor nøglerne mangler, fx i GitHub Actions.
if (!string.IsNullOrEmpty(gitHubClientId) && !string.IsNullOrEmpty(gitHubClientSecret))
{
    builder.Services.AddAuthentication()
        .AddGitHub(o =>
        {
            o.ClientId = gitHubClientId;
            o.ClientSecret = gitHubClientSecret;
            o.CallbackPath = "/signin-github";
        });
}

var app = builder.Build(); //building the webapplication itself (metadata)

using (var scope = app.Services.CreateScope())
{
    var context  = scope.ServiceProvider.GetRequiredService<BisonDBContext>();
    context.Database.Migrate(); //opretter kun database, hvis den slet ikke findes
    DbInitializer.SeedDatabase(context);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await CreateUser (userManager, "edka@itu.dk", "123456");
    await CreateUser (userManager, "debloh@itu.dk", "password");

}

app.UseStaticFiles(); // kobler browseren til filerne i wwwroot -> altså css styling osv så det ikke bare er tekst
app.UseAuthentication();// hvem personen er, altså login, og hvad de har adgang til.
app.UseAuthorization(); //hvad personen der er logget ind må gøre, altså hvilke sider de har adgang til.

app.MapRazorPages(); // kobler URL til razor pages

app.Run();

static async Task CreateUser(UserManager<ApplicationUser> userManager, string email, string password)
{
   if (await userManager.FindByEmailAsync(email) != null) return;

   var result = await userManager.CreateAsync(new ApplicationUser { UserName = email, Email = email }, password);
if (!result.Succeeded)
{
    throw new Exception( string.Join(", ", result.Errors.Select(e => e.Description)));
}
}

// Gør den (ellers implicit genererede) Program-klasse offentlig og tilgængelig, så
// WebApplicationFactory<Program> kan bruges fra Bison.Web.Tests til at køre servicen in-memory.
namespace Bison.Web
{
    public partial class Program { }
}
