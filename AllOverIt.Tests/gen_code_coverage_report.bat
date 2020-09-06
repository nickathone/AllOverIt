rem Requires coverlet.msbuild
dotnet test --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=results/

rem Requires https://github.com/danielpalme/ReportGenerator
C:\Data\ReportGenerator\v4.6.6\netcoreapp3.0\ReportGenerator.exe "-reports:.\results\coverage.opencover.xml" "-targetdir:.\results\output"

pause