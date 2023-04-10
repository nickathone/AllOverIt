REM Add '--no-build' to run the tests without building the project

rmdir /s /q TestResults
rmdir /s /q TestCoverage

dotnet test AllOverIt.sln /p:CoverletOutputFormat=cobertura --collect "XPlat Code Coverage" --results-directory TestResults --no-build
ReportGenerator.exe "-reports:.\TestResults\/**/*.cobertura.xml" "-targetdir:.\TestCoverage" "-reporttypes:Cobertura;Badges;Html;HtmlSummary;MarkdownSummary"              

rmdir /s /q TestResults

copy ".\TestCoverage\summary.html" ".\Docs\Code Coverage\summary.html"
copy ".\TestCoverage\Summary.md" ".\Docs\Code Coverage\Summary.md"
copy ".\TestCoverage\badge_linecoverage.png" ".\Docs\Code Coverage\badge_linecoverage.png"
copy ".\TestCoverage\badge_branchcoverage.png" ".\Docs\Code Coverage\badge_branchcoverage.png"
copy ".\TestCoverage\badge_methodcoverage.png" ".\Docs\Code Coverage\badge_methodcoverage.png"

cd TestCoverage

explorer index.html

