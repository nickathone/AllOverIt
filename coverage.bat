
dotnet test ".\AllOverIt.Tests\AllOverIt.Tests.csproj" --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=coverage\

"%USERPROFILE%\.nuget\packages\reportgenerator\4.6.6\tools\netcoreapp3.0\ReportGenerator.exe" "-reports:.\AllOverIt.Tests\coverage\coverage.cobertura.xml" "-targetdir:.\AllOverIt.Tests\coverage\output"                  

pause
