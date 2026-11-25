// <copyright file="YandexGeocoder.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Serilog;

namespace GeocodingSystem
{
    /// <summary>
    /// Геокодер Yandex Maps.
    /// </summary>
    public class YandexGeocoder : IGeocoder
    {
        private readonly string apiKey;
        private readonly ILogger log = Log.ForContext<YandexGeocoder>();

        private readonly Dictionary<string, GeoResult> database =
            new Dictionary<string, GeoResult>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "Москва, Красная площадь, 1",
                    new GeoResult(55.753930, 37.620800, "Москва, Красная площадь, 1")
                },
                {
                    "г. Томск, проспект Ленина, 15",
                    new GeoResult(56.462800, 84.951400, "Томск, проспект Ленина, 15")
                },
                {
                    "г. Казань, улица Баумана, 1",
                    new GeoResult(55.787893, 49.106414, "Казань, улица Баумана, 1")
                },
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="YandexGeocoder"/> class.
        /// </summary>
        /// <param name="apiKey">API-ключ Yandex.</param>
        public YandexGeocoder(string apiKey)
        {
            this.apiKey = apiKey;
        }

        /// <inheritdoc/>
        public GeoResult GetCoordinates(string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            this.log.Information("YANDEX: запрос координат для {Address}", address);
            this.log.Debug("YANDEX: API-ключ задан: {HasApiKey}", !string.IsNullOrEmpty(this.apiKey));

            GeoResult result;
            if (this.database.TryGetValue(address.Trim(), out result))
            {
                this.log.Debug("YANDEX: найдено {Lat}, {Lon}", result.Latitude, result.Longitude);
                return result;
            }

            this.log.Warning("YANDEX: адрес не найден: {Address}", address);
            return null;
        }
    }
}
