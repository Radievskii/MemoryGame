@echo off
echo === Сбор покрытия через dotnet-coverage ===
dotnet-coverage collect "dotnet test -f net8.0" -f cobertura -o coverage.cobertura.xml

echo === Генерация HTML-отчёта ===
reportgenerator -reports:coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:Html

echo === Открытие отчёта ===
start coverage-report/index.html