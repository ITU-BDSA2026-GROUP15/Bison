#Bison

Bison is a Razor Pages web application for observing and commenting on bird observations. It is  built as the group project in the course *Analysis, Design and Software Architecture* (ITU-BDSA2026).

## Getting started

Clone the repository and run the web application from the repository root:

```bash
dotnet run --project src/Bison.Razor
```

## Generating the Dafny wrapper

The filtering logic is verified in Dafny (`Bison.Dafny/`) and compiled to C#.
`Wrapper.cs` is **generated**, so do not edit it by hand. Change the `.dfy` files and regenerate instead.

**Requirements:** Dafny 4.x (`dotnet tool install --global dafny`) and .NET.

**Steps:**

1. Go to the Dafny folder:
```
   cd Bison.Dafny
```
2. Generate the C# code (includes the Dafny runtime):
```
   dafny translate cs --include-runtime Wrapper.dfy
```
3. Replace the old `Wrapper.cs` in `Bison.Razor/Dafny/` with the newly generated file.
4. Build the project.

**Usage from C#:**
```csharp
Bison.Dafny.wrapper.Api.Filter(root, observations);
```
Note that `observations` must be a `Dafny.ISequence<DafnyObservation>`, not a `List`.