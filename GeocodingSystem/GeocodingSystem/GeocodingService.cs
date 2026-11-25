// <copyright file="GeocodingService.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Serilog;

namespace GeocodingSystem
{
    /// <summary>
    /// Основной модуль геокодирования. Зависит только от абстракции <see cref="IGeocoder"/>.
    /// </summary>
    public class GeocodingService
    {
        private readonly List<IGeocoder> geocoders;
        private readonly ILogger log = Log.ForContext<GeocodingService>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodingService"/> class.
        /// </summary>
        /// <param name="geocoders">Узлы сети геокодеров (внедрение зависимостей).</param>
        public GeocodingService(List<IGeocoder> geocoders)
        {
            this.geocoders = geocoders ?? throw new ArgumentNullException(nameof(geocoders));
        }

        /// <summary>
        /// Пробует все сервисы по очереди и возвращает первый успешный результат.
        /// </summary>
        /// <param name="address">Адрес в свободной форме.</param>
        /// <returns>Координаты либо <c>null</c>, если ни один сервис не справился.</returns>
        public GeoResult Geocode(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Адрес не может быть пустым", nameof(address));
            }

            this.log.Information("Запрос координат: {Address}", address);

            foreach (IGeocoder geocoder in this.geocoders)
            {
                string name = geocoder.GetType().Name;
                try
                {
                    GeoResult result = geocoder.GetCoordinates(address);
                    if (result != null)
                    {
                        this.log.Information("{Geocoder} определил координаты: {Result}", name, result);
                        return result;
                    }

                    this.log.Debug("{Geocoder}: адрес не найден, пробуем следующий сервис", name);
                }
                catch (TimeoutException ex)
                {
                    this.log.Warning(ex, "{Geocoder}: таймаут, пробуем следующий сервис", name);
                }
                catch (GeocodingException ex)
                {
                    this.log.Warning(ex, "{Geocoder}: ошибка геокодирования, пробуем следующий сервис", name);
                }
            }

            this.log.Error("Ни один сервис не смог определить координаты для {Address}", address);
            return null;
        }
    }
}
