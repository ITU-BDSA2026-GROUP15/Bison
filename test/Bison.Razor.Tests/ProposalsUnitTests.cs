public class ProposalsUnitTests
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
    public void OnPost_InvalidModelState_ReturnsPageLoadsTaxonsAndDoesNotCallService()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void OnPost_ServiceReturnsFalse_ReturnsPageWithModelErrorAndLoadsTaxons()
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