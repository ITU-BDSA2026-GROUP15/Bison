using System;

static class UserInterface
{
    public static void PrintObservations(IEnumerable<Cheep> cheeps)
    {    
        foreach (var cheep in cheeps) {
        DateTimeOffset time = convertTime(cheep.Timestamp);
        
        Console.WriteLine(cheep.Author.ToUpper() + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + "(" + cheep.ID.ToString() + ") "+ cheep.Observation);
        }
    }

    public static void PrintObservationAdded(Cheep cheep)
    {
        DateTimeOffset time = convertTime(cheep.Timestamp);

        Console.WriteLine("The following observation has been added to the file:");
        Console.WriteLine(cheep.Author.ToUpper() + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + "(" + cheep.ID.ToString() + ") " + cheep.Observation);
    }

    //NEW: added new function for printing output when new comment is added
    public static void PrintCommentAdded(Cheep cheep)
    {
        DateTimeOffset time = convertTime(cheep.Timestamp);

        Console.WriteLine("The following comment has been added to the file:");
        Console.WriteLine(cheep.Author.ToUpper() + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + "(" + cheep.ID.ToString() + ") "+ cheep.Observation);
    }

    
//method to convert time into correct format
    public static DateTimeOffset convertTime(long timestamp) {
        //DTO needs a long, so we need to parse the string into a long
        long unixSeconds = timestamp;
        //Removed long.parse - since it is already long 
        
        //now using the DTO library to convert unix seconds into actual time
        DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).ToLocalTime();
        
        //returning the time back to the formatting
        return time;
    }

}