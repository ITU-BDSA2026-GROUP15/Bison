using Microsoft.AspNetCore.Mvc.Testing;

namespace Bison.Razor.Tests;

public class FuzzTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public FuzzTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    [Fact]
    public void PlaceHoldertest()
    {
        var client = _factory.CreateClient();
        Assert.NotNull(client);
        

    }
   
}
//Used for testing - since it cant find Program atm
public partial class Program { }