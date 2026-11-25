# ЛР №5 — шпаргалка для отчёта (вариант 14, C# / .NET Framework 4.7.2)

## Результаты статического анализа

| Файл | Предупреждений | Примечание |
| --- | ---: | --- |
| `analysis_before.txt` | 87 | `msbuild ... /t:Rebuild /v:minimal`, до правок |
| `analysis_before_full.txt` | 174 (87 × 2, две сборки подряд) | полный лог «до» |
| `analysis_after.txt` | 0 | после рефакторинга, XML-доков и Serilog |

Группы предупреждений «до» (уникальные, по коду):

| Код | Кол-во | Что исправлено |
| --- | ---: | --- |
| SA1500 | 20 | Фигурные скобки/отступы в инициализаторах |
| SA1101 | 20 | Добавлен префикс `this.` |
| SA1137 | 10 | Элементы вынесены в отдельные файлы, единый порядок |
| SA1309 | 9 | `_apiKey` → `apiKey`, `_database` → `database` |
| SA1402 | 6 | Типы из `Program.cs` разнесены по файлам |
| SA1503 | 5 | `if` без скобок → `if { ... }` |
| SA1413 | 4 | Завершающие запятые в инициализаторах |
| SA1400 / SA1633 / SA1201 / SA1200 | по 2 | Статический `Program`, copyright, порядок элементов, using вне namespace |
| SA0001 / S1075 / S1118 / SA1202 / SA1649 | по 1 | XML-доки включены, URL в App.config, `Program` → static |

## Что нужно заскринить

1. **Error List / Output Visual Studio** до правок — `analysis_before.txt`, видно SA0001, S1075, S1118, SA1101, SA1309, SA1402, SA1503.
2. **`analysis_before.txt`** — конец лога со строкой `Предупреждений: 87` + пример предупреждений.
3. **`analysis_after.txt`** — конец лога со строкой `Предупреждений: 0` (`Сборка успешно завершена`).
4. **Окно консоли с запущенным `GeocodingSystem.exe`** — структурированные логи Serilog `[INF]/[DBG]/[WRN]` с `SourceContext`.
5. **Проводник с `bin\Debug\logs\app_YYYYMMDD.log`** и открытым файлом лога.
6. **`stylecop.json` + `GeocodingSystem.csproj`** — `GenerateDocumentationFile`, `NoWarn 1591`, `AdditionalFiles`, анализаторы в `packages.config`.
7. **`GeocodingService.cs`** — конкретные `catch (TimeoutException)` / `catch (GeocodingException)` и логирование с `ex`.
8. **`Jenkinsfile`** и вкладка Jenkins `Lint & Static Analysis` с артефактами `analysis_after.txt`, `logs/**` (если Jenkins подключён).
9. **Solution Explorer** — дерево из 10 файлов вместо одного `Program.cs`.

## Команды

```powershell
cd E:\Laba1POD\GeocodingSystem\GeocodingSystem
msbuild GeocodingSystem.sln /t:Rebuild /p:Configuration=Debug /v:minimal > analysis_after.txt 2>&1
(Select-String -Path analysis_after.txt -Pattern ': warning ').Count
```

> `msbuild` вызывается из `E:\Visual Studio Community\MSBuild\Current\Bin\MSBuild.exe`
> (в обычном PowerShell он не в PATH — используйте Developer Command Prompt для VS).
