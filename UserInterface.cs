using System;

static class UserInterface
{
    public static void PrintObservations(IEnumerable<Cheep> cheeps)
    {    
        foreach (var cheep in cheeps) {
        DateTimeOffset time = convertTime(cheep.Timestamp);
        
        Console.WriteLine(cheep.Author.ToUpper() + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + cheep.Observation);
        }
    }

    public static void PrintObservationAdded(string author, string observation, DateTimeOffset time)
    {
        Console.WriteLine("The following observation has been added to the file:");
        Console.WriteLine(author.ToUpper() + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + observation);
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