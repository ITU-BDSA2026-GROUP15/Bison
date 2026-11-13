
namespace Bison.CLI;

class Program {

    private readonly IDatabaseRepository _db;
    public Program(IDatabaseRepository db)
    {
        _db = db;
    }

    public static void Main(string[] args) { //args is what you write in the terminal after the program name, for example: dotnet run observe
        var http = new HttpClient { BaseAddress = new Uri("http://localhost:5273") };
        var db = new HttpDatabaseRepository(http);
        var program = new Program(db);
        program.parseArguments(args);

    }

    public void parseArguments(string[] args){

    //here we give the complete args array to commandlineparser
    //the only two types the parser can produce are either "ReadOptions" or "ObserveOptions"
    Parser.Default.ParseArguments<ReadOptions, ObserveOptions, CommentOptions, DiscussionOptions,LocationOptions>(args)

    //the parser only runs when the user writes "read"

        .WithParsed<ReadOptions>(options =>
        {
            //makes sure that the program does not execute if "read" recives extra arguments
            //with a error message, and returns to the terminal without executing the read() method
            if (options.UnexpectedArguments.Any()){
                Console.WriteLine("Error: Unexpected arguments provided for the 'read' command.");
                return;
            }

            read();
        })

        .WithParsed<CommentOptions>(options =>
        {
            comment(options.Id, options.Comment);
        })

        .WithParsed<DiscussionOptions>(options =>
        {
            discussion(options.ObservationId);
        })

        .WithParsed<ObserveOptions>(options =>
        {
            observe(options.Observation, options.Location);

        })
        .WithParsed<LocationOptions>(options =>
        {
            readLocation(options.Location);
        });

    }
    //added location

    // observe() poster nu til web servicen (POST /observation) i stedet for at skrive til CSV-filen direkte.
    internal void observe(string observation, string location) {
        string author = Environment.UserName;
        DateTimeOffset now = DateTimeOffset.Now;
        long timestamp = now.ToUnixTimeSeconds();

        var cheep = new Cheep(author, 0, observation, timestamp, location);

        UserInterface.PrintObservationAdded(_db.AddObservation(cheep));
    }


    // comment() poster nu til web servicen (POST /comment) i stedet for at skrive til CSV-filen direkte.
    internal void comment(int id, string comment) {
        string author = Environment.UserName;
        DateTimeOffset now = DateTimeOffset.Now;
        long timestamp = now.ToUnixTimeSeconds();

        var observations = _db.GetObservations();

        if (!observations.Any(o => o.ID == id)) {
            //if no observation with the given ID exists on the service
            Console.WriteLine("No observations with ID: (" + id + ") currently exists");
            return;
        }

        var cheep = new Cheep(author, id, comment, timestamp, string.Empty); //Cheep as a comment
        UserInterface.PrintCommentAdded(_db.AddComment(cheep));

    }


    // discussion() henter nu kommentarerne til en given observation fra web servicen (GET /comments?id=),
    // som allerede filtrerer server-side. Klienten skal derfor ikke længere filtrere selv.
    internal void discussion(int obsId){
        UserInterface.PrintObservations(_db.GetComments(obsId));
    }

    internal void read()
    {
        UserInterface.PrintObservations(_db.GetObservations());
    }

    internal void readLocation(string location)
    {
        UserInterface.PrintObservations(_db.GetObservationsByLocation(location));
    }
}
