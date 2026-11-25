// <copyright file="LoggerConfig.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

using Serilog;

namespace GeocodingSystem
{
    /// <summary>
    /// Конфигурация логирования приложения (Serilog: консоль + файл).
    /// </summary>
    public static class LoggerConfig
    {
        /// <summary>
        /// Инициализирует глобальный логгер.
        /// </summary>
        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/app_.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Логгер инициализирован");
        }

        /// <summary>
        /// Корректно закрывает логгер при завершении приложения.
        /// </summary>
        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }
    }
}
