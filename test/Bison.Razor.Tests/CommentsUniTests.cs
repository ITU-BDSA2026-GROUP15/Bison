namespace Bison.Razor.Tests;

public class CommentsUnitTests
{
    [Fact]
    public void OnPost_ValidInput_RedirectsToObservationDetails()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void OnPost_ValidInput_CallsServiceWithCorrectArguments()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void OnPost_MissingEmailClaim_PassesEmptyStringToService()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void OnPost_NoUsername_ReturnsForbidAndDoesNotCallService()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void OnPost_InvalidModelState_ReturnsPageAndDoesNotCallService()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void OnPost_ServiceReturnsFalse_ReturnsPageWithModelError()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void OnPost_NoUsernameAndInvalidModelState_ReturnsForbid()
    {
        // arrange

        // act

        // assert
    }
}

