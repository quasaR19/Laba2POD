// <copyright file="OsmGeocoder.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Serilog;

namespace GeocodingSystem
{
    /// <summary>
    /// Геокодер OpenStreetMap (Nominatim).
    /// </summary>
    public class OsmGeocoder : IGeocoder
    {
        private readonly string serverUrl;
        private readonly ILogger log = Log.ForContext<OsmGeocoder>();

        private readonly Dictionary<string, GeoResult> database =
            new Dictionary<string, GeoResult>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "г. Иркутск, ул. Карла Маркса, 1",
                    new GeoResult(52.286199, 104.305303, "Иркутск, улица Карла Маркса, 1")
                },
                {
                    "г. Владивосток, улица Светланская, 33",
                    new GeoResult(43.115542, 131.885494, "Владивосток, улица Светланская, 33")
                },
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmGeocoder"/> class.
        /// </summary>
        /// <param name="serverUrl">URL сервера Nominatim из конфигурации.</param>
        public OsmGeocoder(string serverUrl)
        {
            this.serverUrl = serverUrl;
        }

        /// <inheritdoc/>
        public GeoResult GetCoordinates(string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            this.log.Information("OSM: запрос к {ServerUrl} для {Address}", this.serverUrl, address);

            GeoResult result;
            if (this.database.TryGetValue(address.Trim(), out result))
            {
                this.log.Debug("OSM: найдено {Lat}, {Lon}", result.Latitude, result.Longitude);
                return result;
            }

            this.log.Warning("OSM: таймаут при запросе {Address}", address);
            throw new TimeoutException("Превышено время ожидания ответа (Nominatim API)");
        }
    }
}
