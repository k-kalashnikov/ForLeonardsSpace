# Путь к папке с контроллерами
$controllersPath = "..\Masofa.Web.Monolith\Controllers\Dictionaries"
# Путь к папке для тестов
$testsPath = "Controllers"

# Создаем папку для тестов, если её нет
if (!(Test-Path $testsPath)) {
    New-Item -ItemType Directory -Path $testsPath
}

# Получаем все файлы контроллеров
$controllerFiles = Get-ChildItem -Path $controllersPath -Filter "*Controller.cs"

foreach ($file in $controllerFiles) {
    # Получаем имя файла без расширения
    $fileName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
    
    # Убираем "Controller" из имени для названия модели
    $modelName = $fileName -replace "Controller$", ""
    
    # Создаем содержимое файла
    $content = @"
using Masofa.Tests.Controllers;
using Masofa.Web.Monolith.Controllers.Dictionaries;

namespace Masofa.Tests.Controllers.Dictionaries
{
    public class ${modelName}ControllerTests : BaseCrudControllerTests<$modelName, MasofaDictionariesDbContext>
    {

    }
}
"@
    
    # Создаем файл теста
    $testFileName = "${modelName}ControllerTests.cs"
    $content | Out-File -FilePath "$testsPath\$testFileName" -Encoding UTF8
    
    Write-Host "File created: $testFileName"
}

Write-Host "Generation completed! Files created: $($controllerFiles.Count)"