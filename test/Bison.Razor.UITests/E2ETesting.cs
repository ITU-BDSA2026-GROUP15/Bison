using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;


public class E2ETesting : BisonPageTest
{
 [SetUp]
    public async Task LogIn()
    {
        await LogInAsync();
    }

    [Test]
    public async Task CreateObservationAndSeeItInMyObservations()
    {
        
    }

}

