namespace Bison.Razor.Tests;

public class UnitTests
{
    [Fact]
    public void UnixTimestamp_IsConvertedToCorrectDateString()
    {
        // ARRANGE
        double knownUnixTimestamp = 1690891760;
        string expected = "08/01/23 12:09:20";

        // ACT
        string actual = DBFacade.UnixTimeStampToDateTimeString(knownUnixTimestamp);

        // ASSERT
        Assert.Equal(expected, actual);
    }
}
