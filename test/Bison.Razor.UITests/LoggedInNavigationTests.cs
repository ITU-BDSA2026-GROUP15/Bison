using Microsoft.Playwright;

namespace Bison.Razor.UITests;

public class LoggedInNavigationTests : BisonPageTest
{
    // [SetUp] kører før hver test. Hver test får sin egen browsersession og skal derfor logge ind selv.
    [SetUp]
    public async Task LogIn()
    {
        await LogInAsync();
    }

    [Test]
    public async Task LogoutButtonIsShownWhenLoggedIn()
    {
        var logoutButton = Page.GetByRole(AriaRole.Button, new() { Name = $"Logout [{TestUserEmail}]", Exact = true });

        await Expect(logoutButton).ToBeVisibleAsync();
    }

    [Test]
    public async Task MyAndPublicObservationsLinksAreShownWhenLoggedIn()
    {
        var myObservationsLink = Page.GetByRole(AriaRole.Link, new() { Name = "My Observations", Exact = true });
        var publicLink = Page.GetByRole(AriaRole.Link, new() { Name = "Public Observations", Exact = true });

        await Expect(myObservationsLink).ToBeVisibleAsync();
        await Expect(publicLink).ToBeVisibleAsync();
    }
}
