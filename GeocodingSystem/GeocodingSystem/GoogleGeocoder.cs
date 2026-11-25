// <copyright file="GoogleGeocoder.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Serilog;

namespace GeocodingSystem
{
    /// <summary>
    /// Геокодер Google Maps.
    /// </summary>
    public class GoogleGeocoder : IGeocoder
    {
        private readonly string apiKey;
        private readonly ILogger log = Log.ForContext<GoogleGeocoder>();

        private readonly Dictionary<string, GeoResult> database =
            new Dictionary<string, GeoResult>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "Москва, Красная площадь, 1",
                    new GeoResult(55.753933, 37.620784, "Москва, Красная площадь, 1")
                },
                {
                    "Санкт-Петербург, Невский проспект, 28",
                    new GeoResult(59.934280, 30.335198, "Санкт-Петербург, Невский проспект, 28")
                },
                {
                    "London, Downing Street 10",
                    new GeoResult(51.503363, -0.127625, "10 Downing Street, London, UK")
                },
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleGeocoder"/> class.
        /// </summary>
        /// <param name="apiKey">API-ключ Google.</param>
        public GoogleGeocoder(string apiKey)
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

            this.log.Information("GOOGLE: запрос координат для {Address}", address);
            this.log.Debug("GOOGLE: API-ключ задан: {HasApiKey}", !string.IsNullOrEmpty(this.apiKey));

            GeoResult result;
            if (this.database.TryGetValue(address.Trim(), out result))
            {
                this.log.Debug("GOOGLE: найдено {Lat}, {Lon}", result.Latitude, result.Longitude);
                return result;
            }

            this.log.Warning("GOOGLE: адрес не найден: {Address}", address);
            return null;
        }
    }
}
