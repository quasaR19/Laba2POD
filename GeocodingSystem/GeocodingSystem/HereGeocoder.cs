// <copyright file="HereGeocoder.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Serilog;

namespace GeocodingSystem
{
    /// <summary>
    /// Геокодер HERE Maps.
    /// </summary>
    public class HereGeocoder : IGeocoder
    {
        private readonly string apiKey;
        private readonly ILogger log = Log.ForContext<HereGeocoder>();

        private readonly Dictionary<string, GeoResult> database =
            new Dictionary<string, GeoResult>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "Екатеринбург, проспект Ленина, 50",
                    new GeoResult(56.837430, 60.614490, "Екатеринбург, проспект Ленина, 50")
                },
                {
                    "с. Ромашково, ул. Полевая, 7",
                    new GeoResult(55.560130, 37.440420, "Ромашково, улица Полевая, 7")
                },
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="HereGeocoder"/> class.
        /// </summary>
        /// <param name="apiKey">API-ключ HERE.</param>
        public HereGeocoder(string apiKey)
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

            this.log.Information("HERE: запрос координат для {Address}", address);
            this.log.Debug("HERE: API-ключ задан: {HasApiKey}", !string.IsNullOrEmpty(this.apiKey));

            GeoResult result;
            if (this.database.TryGetValue(address.Trim(), out result))
            {
                this.log.Debug("HERE: найдено {Lat}, {Lon}", result.Latitude, result.Longitude);
                return result;
            }

            this.log.Warning("HERE: адрес не найден: {Address}", address);
            return null;
        }
    }
}
