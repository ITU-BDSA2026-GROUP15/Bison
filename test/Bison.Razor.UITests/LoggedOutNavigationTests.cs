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
}
