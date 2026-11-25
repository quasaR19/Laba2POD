# ЛР №4 — шпаргалка для отчёта (Declarative Pipeline в Jenkins)

Проект: `GeocodingSystem` (ЛР №1, вариант 14) · репозиторий: `https://github.com/quasaR19/Laba2POD.git` (ЛР №2) · Jenkinsfile: корень репозитория.

## 1. Что нужно для отчёта (по §8 задания)

| Пункт | Где взять |
| --- | --- |
| Титульный лист, цель работы | §1 задания ЛР №4; тема: «Построение базового CI/CD конвейера в Jenkins с использованием Groovy DSL» |
| Jenkins и Groovy DSL | `LAB4_overview.html`, раздел 2 |
| Структура pipeline (схема) | `LAB4_overview.html`, раздел 3 — Checkout → Setup → Lint → Test → Archive |
| Листинг Jenkinsfile | `../../Jenkinsfile` (корень репозитория), также раздел 4 обзора |
| Скриншот главной страницы Jenkins | `lab4_screenshots/01_jenkins_home.png` — снят |
| Скриншот этапов pipeline | `lab4_screenshots/02_stage_view.png` — снят |
| Скриншот Console Output | `lab4_screenshots/03_console_output.png` — снят (`Finished: SUCCESS`) |
| Скриншот Artifacts | `lab4_screenshots/04_artifacts.png` — снят |
| Скриншот настройки webhook в GitHub | не снят: нужен публичный адрес Jenkins и доступ к настройкам репозитория (см. ниже) |
| Скриншот установленных плагинов | `lab4_screenshots/06_plugins.png` — снят |
| Анализ результатов | `LAB4_overview.html`, раздел 6 |
| Выводы | `LAB4_overview.html`, раздел 7 |

## 2. Настройка задачи Jenkins

- New Item → `Lab3-Git-Pipeline` → Pipeline → OK.
- Pipeline → **Pipeline script from SCM**.
- SCM: Git; Repository URL: `https://github.com/quasaR19/Laba2POD.git`; Branch: `*/main`; Script Path: `Jenkinsfile`.
- Build Triggers: **GitHub hook trigger for GITScm polling**.
- GitHub → Settings → Webhooks → Add webhook:
  - Payload URL: `http://<jenkins-host>:8080/github-webhook/`
  - Content type: `application/json`
  - Events: **Just the push event**
  - Active: ✔

В этой среде Jenkins поднят локально на `127.0.0.1:8080`, поэтому для приёма вебхука из GitHub нужен публичный адрес (например, `ngrok http 8080`). Без проброса порта webhook создан быть не может — в отчёт берётся инструкция и скриншот формы настроек.

## 3. Этапы конвейера

| Stage | Что делает | Команда / шаг |
| --- | --- | --- |
| Checkout | Получает код из Git, печатает commit | `checkout scm`, `git rev-parse --short HEAD` |
| Setup | Скачивает `nuget.exe` (если нет) и восстанавливает пакеты | `nuget.exe restore GeocodingSystem.sln -PackagesDirectory packages` |
| Lint & Static Analysis | Сборка с анализаторами, подсчёт предупреждений | `MSBuild.exe ... /t:Rebuild /v:minimal > analysis_ci.txt` |
| Test | Smoke-запуск `.exe`, проверка файла лога Serilog | `GeocodingSystem.exe < NUL`, `Get-ChildItem logs/app_*.log` |
| Archive | Артефакт `build.txt` + exe + логи | `archiveArtifacts artifacts: 'artifacts/**, ...'` |

## 3.1. Реальный запуск

- Jenkins 2.580.3 локально: `http://127.0.0.1:8080`, задача `Lab3-Git-Pipeline` из SCM (`*/main`, Script Path `Jenkinsfile`).
- Установлено 64 плагина (включая зависимости): Git 5.11.0, workflow-aggregator 608.v67378e9d3db_1, GitHub 1.47.0, Docker Pipeline 653.v2f2c08eff0ec, Pipeline Model Definition 2.2293.v6e7193cec599.
- Сборка **#2: SUCCESS**, commit `0fe845148427a1372be70bf2bfe6430401f86043` (`0fe8451`), MSBuild warnings: 0, найден лог `app_20261125.log`.
- В артефактах: `analysis_ci.txt`, `artifacts/build.txt`, `GeocodingSystem.exe`, `app_20261125.log`, `run_output.txt`.
- В Jenkins включён триггер `GitHub hook trigger for GITScm polling`. Webhook в GitHub не создан: локальный Jenkins недоступен извне; нужен публичный URL (например, `ngrok http 8080`).

## 4. Полезные команды

```powershell
# Локальная проверка синтаксиса Jenkinsfile
curl.exe -u admin:<password> -X POST --data-urlencode 'jenkinsfile=@Jenkinsfile' `
  http://127.0.0.1:8080/pipeline-model-converter/validate

# Создание задачи через Script Console (Groovy)
# См. LAB4_overview.html, раздел 5.
```

## 5. Скриншоты (чек-лист)

1. Jenkins → главная страница / список задач.
2. Jenkins → задача `Lab3-Git-Pipeline` → Stage View с зелёными этапами.
3. Jenkins → Build #N → Console Output (видно `Finished: SUCCESS`, вывод этапов).
4. Jenkins → Build #N → Artifacts (`build.txt`, `GeocodingSystem.exe`, `logs/app_*.log`, `analysis_ci.txt`).
5. GitHub → Settings → Webhooks (форма Payload URL / Content type / Events).
6. Jenkins → Manage Jenkins → Plugins (Git, Pipeline, GitHub Integration, Docker Pipeline) — `06_plugins.png`.

> Скриншоты, снятые в этой среде, лежат в `lab4_screenshots/`. Пункт 5 (GitHub Webhook) остаётся чек-листом: Jenkins запущен только на `127.0.0.1`, поэтому forms webhook недоступна без публичного адреса. Не заменяйте её выдуманным изображением.
