REM Add '--no-build' to run the tests without building the project

rmdir /s /q TestResults
rmdir /s /q TestCoverage


dotnet test AllOverIt.sln /p:CoverletOutputFormat=cobertura --collect "XPlat Code Coverage" --results-directory TestResults
ReportGenerator.exe "-reports:.\TestResults\/**/*.cobertura.xml" "-targetdir:.\TestCoverage" "-reporttypes:Badges;Html;Cobertura"              

cd TestCoverage

explorer index.html

