# Assembler6502

[![Build status](https://ci.appveyor.com/api/projects/status/oid9ll2vl0nbe04d/branch/master?svg=true)](https://ci.appveyor.com/project/MarkWithall/assembler6502)

A 6502 assembler for the C64 to help my learning of 6502 assembly language.

No pull requests for the time being please, as this is currently just a repository for my own learning. Feel free to submit issues if you spot any errors or have suggestions. Thanks.

## To Build etc.

The following commands should be issued from the root of the project folder.

Restore the nuget package:

`dotnet restore`

Build the project:

`dotnet build`

Test the project:

`dotnet test Assembler6502.Tests/`

Run the project:

`dotnet run -p Assembler6502/Assembler6502.csproj test.asm test.prg`

Publish the project:

`dotnet publish -c Release -r osx-x64`

or

`dotnet publish -c Release -r win-x64`

or whatever target from [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).
