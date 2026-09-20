
public record Cheep(string Author, int ID, string Observation, long Timestamp, string Location);
// OBS: Cheep findes nu 2 steder og er ren kodeduplikation. Skal nok slettes i Bison.CLI,
// da både Bison.CLI og Bison.razor kan bruge cheep fra simpleDB.
