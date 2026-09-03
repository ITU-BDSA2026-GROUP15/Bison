using CommandLine;

[Verb("read", HelpText ="Read and print the observations from the database.")]

public class ReadOptions{
    //Empty because read does not require any additional input from the user.

     [Value(0, MetaName = "unexpected arguments", Required = false)]
    public IEnumerable<string> UnexpectedArguments { get; set; }
        = Array.Empty<string>();
}
//there is a bug: when i write dotnet run -- read extra it is suppose to give me an error
// it does not.