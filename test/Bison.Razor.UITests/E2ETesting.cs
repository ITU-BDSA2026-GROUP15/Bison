using Microsoft.Playwright;


namespace Bison.Razor.UITests;

public class ObservationCreationTests : BisonPageTest
{

    [SetUp]
    
    public async Task LogIn()
    {
        await LogInAsync();
    }

    [Test]
    public async Task CreatedObservationAppearsInAuthorAndPublicLists()
    {
        // Arrange
        var observationText = $"E2E test observation {Guid.NewGuid()}";

        // Act: open the creation form
        await Page.GotoAsync("/CreateObservation");

        // Fill in the observation
        await Page.GetByLabel("Observation").FillAsync(observationText);

        // Select the first available taxon (index 0 is the placeholder)
        await Page.GetByLabel("Taxon").SelectOptionAsync(
            new SelectOptionValue { Index = 1 });

        // Submit the form
        await Page.GetByRole(AriaRole.Button,
            new() { Name = "Share", Exact = true }).ClickAsync();

        // Assert: observation appears in the author's timeline
        var authorObservation = Page.GetByText(
            observationText,
            new() { Exact = true });

        // Act: navigate to public observations
        await Page.GetByRole(AriaRole.Link,
            new() { Name = "Public Observations", Exact = true }).ClickAsync();

        // Assert: observation also appears in the public list
        var publicObservation = Page.GetByText(
            observationText,
            new() { Exact = true });
    }

}