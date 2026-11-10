using Microsoft.Playwright;

namespace Bison.Razor.UITests;

public class ObservationPictureTests : BisonPageTest
{
    [Test]
    public async Task PictureIsShownForObservationWithPicture()
    {
        await Page.GotoAsync("/");

        // Vi leder kun inde i den observation, der har et billede i DbInitializer,
        // så testen også fanger, hvis billedet havner ved den forkerte observation.
        var observation = Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = "About 30 spoonbills" });
        var picture = observation.GetByRole(AriaRole.Img, new() { Name = "Picture of the observed animal" });

        await Expect(picture).ToBeVisibleAsync();

        // Et ødelagt billede kan stadig være "synligt" på grund af alt-teksten.
        // naturalWidth er billedfilens bredde og er kun større end 0, hvis filen faktisk blev indlæst.
        var naturalWidth = await picture.EvaluateAsync<int>("image => image.naturalWidth");

        Assert.That(naturalWidth, Is.GreaterThan(0));
    }
}
