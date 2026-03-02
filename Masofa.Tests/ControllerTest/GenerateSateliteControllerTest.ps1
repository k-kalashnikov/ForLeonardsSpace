# Папка с контроллерами
$ModelsPath = "..\..\Masofa.Web.Monolith\Controllers\Satellite\Landsat"

# Папка для тестов
$TestsPath = ".\Satellite\Landsat"

# Убедимся, что папка для тестов существует
if (!(Test-Path $TestsPath)) {
    New-Item -ItemType Directory -Path $TestsPath -Force
}

# Получаем все файлы контроллеров
$ModelsFiles = Get-ChildItem -Path $ModelsPath -Filter "*.cs"

foreach ($File in $ModelsFiles) {
    # Извлекаем имя класса (без .cs)
    $ControllerName = $File.BaseName  # Например: AdministrativeUnitController
    $TestClassName = $ControllerName + "Test"  # Например: AdministrativeUnitControllerTest
    
    # Имя модели (удаляем "Controller")
    $ModelName = $ControllerName -replace "Controller$", ""  # AdministrativeUnit
    $ModelName = $ModelName + "Entity"
    # Содержимое файла
    $Content = @"
using Masofa.Common.Models.Satellite.Landsat;

namespace Masofa.Tests.ControllerTest.Satellite.Landsat
{
    public class $TestClassName : BaseCrudControllerTest<$ModelName, MasofaLandsatDbContext>
    {

    }
}
"@

    # Путь к файлу теста
    $TestFilePath = Join-Path $TestsPath "$TestClassName.cs"

    # Записываем в файл
    $Content | Out-File -FilePath $TestFilePath -Encoding UTF8

    Write-Host "Создан тест: $TestFilePath"
}

Write-Host "Генерация завершена."