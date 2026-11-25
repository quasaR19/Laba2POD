// ЛР №4 — базовый CI/CD конвейер в Jenkins (Declarative Pipeline, Groovy DSL)
// Проект: GeocodingSystem (ЛР №1), репозиторий: Laba2POD (ЛР №2), вариант 14.
// Script Path в задаче Jenkins: Jenkinsfile (корень репозитория).

pipeline {
    agent any

    environment {
        PROJECT_NAME = 'Laba2POD'
        WORK_DIR = 'GeocodingSystem/GeocodingSystem'
        SOLUTION = 'GeocodingSystem/GeocodingSystem/GeocodingSystem.sln'
        MSBUILD = 'E:\\Visual Studio Community\\MSBuild\\Current\\Bin\\MSBuild.exe'
    }

    stages {
        stage('Checkout') {
            steps {
                echo "Клонирование проекта ${env.PROJECT_NAME} из ${env.GIT_URL}"
                checkout scm
                bat 'git rev-parse --short HEAD'
                echo "Рабочая папка: ${env.WORKSPACE}"
            }
        }

        stage('Setup') {
            steps {
                echo 'Установка зависимостей (packages.config) ...'
                bat 'if not exist "%WORK_DIR%\\nuget.exe" curl.exe -L -o "%WORK_DIR%\\nuget.exe" https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
                bat 'cd /d "%WORK_DIR%" && nuget.exe restore GeocodingSystem.sln -PackagesDirectory packages'
            }
        }

        stage('Lint & Static Analysis') {
            steps {
                echo 'Сборка со статическими анализаторами (StyleCop / Sonar / SecurityCodeScan) ...'
                bat '"%MSBUILD%" "%SOLUTION%" /t:Rebuild /p:Configuration=Debug /v:minimal > analysis_ci.txt 2>&1'
                bat 'powershell -NoProfile -Command "$w = (Select-String -Path analysis_ci.txt -Pattern \': warning \').Count; Write-Host (\"Предупреждений сборки: \" + $w)"'
            }
            post {
                always {
                    archiveArtifacts artifacts: 'analysis_ci.txt', allowEmptyArchive: true
                }
            }
        }

        stage('Test') {
            steps {
                echo 'Smoke-тест: запуск GeocodingSystem.exe и проверка логов Serilog ...'
                bat 'cd /d "%WORK_DIR%\\bin\\Debug" && (GeocodingSystem.exe < NUL > run_output.txt 2>&1)'
                bat 'powershell -NoProfile -Command "if (-not (Get-ChildItem \'%WORK_DIR%/bin/Debug/logs/app_*.log\' -ErrorAction SilentlyContinue)) { throw \'Файл лога Serilog не создан\' }; Get-ChildItem \'%WORK_DIR%/bin/Debug/logs/app_*.log\' | Select-Object -ExpandProperty Name"'
            }
            post {
                always {
                    archiveArtifacts artifacts: 'GeocodingSystem/GeocodingSystem/bin/Debug/run_output.txt, GeocodingSystem/GeocodingSystem/bin/Debug/logs/**', allowEmptyArchive: true
                }
            }
        }

        stage('Archive') {
            steps {
                echo 'Сохранение артефакта сборки ...'
                bat 'if not exist artifacts mkdir artifacts && echo Build %BUILD_NUMBER% commit %GIT_COMMIT% > artifacts\\build.txt'
                archiveArtifacts artifacts: 'artifacts/**, GeocodingSystem/GeocodingSystem/bin/Debug/GeocodingSystem.exe', allowEmptyArchive: true
            }
        }
    }

    post {
        always {
            echo "Pipeline завершён. Номер сборки: ${env.BUILD_NUMBER}"
        }
        success {
            echo 'Все этапы прошли успешно!'
        }
        failure {
            echo 'Один из этапов завершился с ошибкой'
        }
    }
}
