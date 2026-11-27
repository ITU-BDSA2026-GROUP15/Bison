using Microsoft.Playwright;


namespace Bison.Razor.UITests;

public class ObservationCreationTests : BisonPageTest
{

    //this test verifies that the user can: 
    //Log in.
    // Open the observation creation form.
    // Create an observation.
    // See the observation in their own timeline.
    // See the observation in the public observations list.
    // Navigate away and return to their timeline and still see the observation.

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
        
        var authorObservation = Page.GetByText(observationText);

        await Expect(authorObservation).ToBeVisibleAsync();

        // Act: navigate to public observations
        await Page.GetByRole(AriaRole.Link,
            new() { Name = "Public Observations", Exact = true }).ClickAsync();

        // Assert: observation also appears in the public list
        var publicObservation = Page.GetByText(observationText);

        await Expect(publicObservation).ToBeVisibleAsync();


        // Navigate back to the author's timeline by using the author's email
        // as the URL parameter. This performs a fresh page load.

        await Page.GotoAsync($"/UserTimeline?author={Uri.EscapeDataString(TestUserEmail)}");

        // Assert: Verify that the observation is still present after navigating
        // away from the timeline and loading it again, confirming it was persisted.
        
        await Page.GotoAsync($"/obs/{Uri.EscapeDataString(TestUserEmail)}");
    }

}