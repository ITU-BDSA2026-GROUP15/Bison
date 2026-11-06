using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Bison.Razor.UITests;

// Fælles basisklasse for vores UI-tests. Med BaseURL kan en test nøjes med at skrive
// Page.GotoAsync("/") i stedet for at kende den port, Bison tilfældigvis kører på.
public abstract class BisonPageTest : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions { BaseURL = BisonServer.BaseUrl };
    }
}
