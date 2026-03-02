# Папка с контроллерами
$ModelsPath = "..\..\Masofa.Common\Models\Dictionaries\"

# Папка для тестов
$TestsPath = ".\Dictionaries\"

# Убедимся, что папка для тестов существует
if (!(Test-Path $TestsPath)) {
    New-Item -ItemType Directory -Path $TestsPath -Force
}

# Получаем все файлы контроллеров
$ModelsFiles = Get-ChildItem -Path $ModelsPath -Filter "*.cs"

foreach ($File in $ModelsFiles) {
    # Извлекаем имя класса (без .cs)
    $ControllerName = $File.BaseName  # Например: AdministrativeUnit
    $TestClassName = $ControllerName + "ControllerTest"  # Например: AdministrativeUnitControllerTest

    # Содержимое файла
    $Content = @"
namespace Masofa.Tests.ControllerTest.Dictionaries
{
    public class $TestClassName : BaseCrudControllerTest<Masofa.Common.Models.Dictionaries.$ControllerName, MasofaDictionariesDbContext>
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