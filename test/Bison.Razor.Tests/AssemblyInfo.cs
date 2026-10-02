// Indstillinger, der gælder for HELE testprojektet. 
// Midlertidig løsning. Lige nu fejler testene på Github actions
// pga. race conditions. Paginationtests og apitests starter begge 
// webappen, men i Program.cs linje 11 (if (!File.Exists(dbPath)))
// tjekker hver opstart, om den rigtige bison.db findes
// FØR testene når at bytte DBFacade ud med testdatabasen.

// xUnit kører testklasser samtidig. På GitHub Actions
// findes bison.db ikke, så begge apps får "nej" og kører schema.sql mod
// samme fil på samme tid. Den ene når at oprette tabellen user først,
// og den anden fejler med "table user already exists".
[assembly: CollectionBehavior(DisableTestParallelization = true)]
