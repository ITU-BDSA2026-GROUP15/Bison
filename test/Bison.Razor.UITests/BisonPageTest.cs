using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Bison.Razor.UITests;

// Fælles basisklasse for vores UI-tests. Med BaseURL kan en test nøjes med at skrive
// Page.GotoAsync("/") i stedet for at kende den port, Bison tilfældigvis kører på.
public abstract class BisonPageTest : PageTest
{
    // Oprettes af Program.cs ved opstart. GitHub-login kan en test ikke klikke sig igennem,
    // så vi bruger almindeligt login med e-mail og adgangskode.
    protected const string TestUserEmail = "edka@itu.dk";
    private const string TestUserPassword = "123456";

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions { BaseURL = BisonServer.BaseUrl };
    }

    protected async Task LogInAsync()
    {
        await Page.GotoAsync("/Identity/Account/Login");

        await Page.GetByLabel("Email").FillAsync(TestUserEmail);
        await Page.GetByLabel("Password").FillAsync(TestUserPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in", Exact = true }).ClickAsync();
    }
}
