// Indstillinger, der gælder for HELE testprojektet. 
// Midlertidig løsning. Lige nu fejler testene på Github actions
// pga. race conditions. Paginationtests og apitests starter begge 
// webappen, men i Program.cs linje 11 (if (!File.Exists(dbPath)))
// tjekker hver opstart, om den rigtige bison.db findes
// FØR testene når at bytte DBFacade ud med testdatabasen.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
