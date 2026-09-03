using CommandLine;

[Verb("read", HelpText ="Read and print the observations from the database.")]

public class ReadOptions{
    //Empty because read does not require any additional input from the user.
}
//there is a bug: when i write dotnet run -- read extra it is suppose to give me an error
// it does not.