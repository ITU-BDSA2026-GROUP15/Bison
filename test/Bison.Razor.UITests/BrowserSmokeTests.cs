using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Bison.Razor.UITests;

// Arver fra PageTest: Playwright starter en browser og giver hver test sin egen, friske 'Page'.
public class BrowserSmokeTests : PageTest
{
    // Tjekker kun selve opsætningen (NUnit -> Playwright -> Chromium). Bison er ikke involveret endnu.
    [Test]
    public async Task BrowserCanShowAPage()
    {
        await Page.SetContentAsync("<h1>Hello Bison</h1>");

        var heading = Page.GetByRole(AriaRole.Heading, new() { Name = "Hello Bison" });

        // Expect(...) venter og prøver igen i op til 5 sekunder, før testen fejler.
        await Expect(heading).ToBeVisibleAsync();
    }
}
