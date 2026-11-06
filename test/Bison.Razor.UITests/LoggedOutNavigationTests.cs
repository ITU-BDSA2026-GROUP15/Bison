using Microsoft.Playwright;

namespace Bison.Razor.UITests;

public class LoggedOutNavigationTests : BisonPageTest
{
    [Test]
    public async Task LoginLinkIsShownWhenNotLoggedIn()
    {
        await Page.GotoAsync("/");

        // Exact = true, så testen kun rammer et link, der hedder præcis "Login".
        var loginLink = Page.GetByRole(AriaRole.Link, new() { Name = "Login", Exact = true });

        await Expect(loginLink).ToBeVisibleAsync();
    }

    [Test]
    public async Task PublicObservationsLinkIsShownWhenNotLoggedIn()
    {
        await Page.GotoAsync("/");

        var publicLink = Page.GetByRole(AriaRole.Link, new() { Name = "Public Observations", Exact = true });

        await Expect(publicLink).ToBeVisibleAsync();
    }

    [Test]
    public async Task MyObservationsLinkIsNotShownWhenNotLoggedIn()
    {
        await Page.GotoAsync("/");

        // Tjekkes først, for ellers ville testen også bestå på en fejlside uden navigation.
        var loginLink = Page.GetByRole(AriaRole.Link, new() { Name = "Login", Exact = true });
        await Expect(loginLink).ToBeVisibleAsync();

        var myObservationsLink = Page.GetByRole(AriaRole.Link, new() { Name = "My Observations", Exact = true });

        await Expect(myObservationsLink).ToBeHiddenAsync();
    }
}
