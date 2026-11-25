// <copyright file="Program.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Configuration;
using Serilog;

namespace GeocodingSystem
{
    /// <summary>
    /// Точка входа системы геокодирования (вариант 14).
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Точка входа приложения.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            LoggerConfig.Initialize();
            ILogger log = Log.ForContext(typeof(Program));

            try
            {
                log.Information("Запуск системы геокодирования (вариант 14)");

                IGeocoder google = new GoogleGeocoder(ConfigurationManager.AppSettings["GoogleApiKey"]);
                IGeocoder yandex = new YandexGeocoder(ConfigurationManager.AppSettings["YandexApiKey"]);
                IGeocoder osm = new OsmGeocoder(ConfigurationManager.AppSettings["OsmServerUrl"]);

                var geocoders = new List<IGeocoder>
                {
                    google,
                    yandex,
                    osm,
                };

                var service = new GeocodingService(geocoders);

                log.Information("=== Базовый запуск (Google, Yandex, OSM) ===");
                service.Geocode("Москва, Красная площадь, 1");
                service.Geocode("г. Томск, проспект Ленина, 15");
                service.Geocode("г. Иркутск, ул. Карла Маркса, 1");
                service.Geocode("с. Ромашково, ул. Полевая, 7");

                log.Information("=== Расширение: HereGeocoder (GeocodingService не изменялся) ===");
                IGeocoder here = new HereGeocoder(ConfigurationManager.AppSettings["HereApiKey"]);
                var extendedGeocoders = new List<IGeocoder>(geocoders)
                {
                    here,
                };

                var extendedService = new GeocodingService(extendedGeocoders);
                extendedService.Geocode("Екатеринбург, проспект Ленина, 50");
                extendedService.Geocode("с. Ромашково, ул. Полевая, 7");
            }
            catch (Exception ex)
            {
                log.Fatal(ex, "Критическая ошибка при выполнении приложения");
                throw new GeocodingException("Критическая ошибка при выполнении приложения", ex);
            }
            finally
            {
                LoggerConfig.Shutdown();
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
        }
    }
}
