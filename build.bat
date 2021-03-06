@echo off

REM Build XMLTidy on Windows
REM If the CSC binary is not recognized, you must add it to the System Path
REM or properly set the variable MYCSC below.

if exist .\bin goto BUILD
echo Create build output directory .\bin
md .\bin

:BUILD
copy .\dotnet\Mono.Options.dll .\bin\Mono.Options.dll
copy .\dotnet\Mono.Options.pdb .\bin\Mono.Options.pdb
echo Start program build

csc /t:exe /out:.\bin\xmltidy.exe /debug+ /nologo /w:4 /r:.\dotnet\Mono.Options.dll *.cs

echo DONE! Run .\bin\xmltidy.exe --help for usage
echo .

