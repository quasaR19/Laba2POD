// <copyright file="GeoResult.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

namespace GeocodingSystem
{
    /// <summary>
    /// Результат геокодирования — координаты и отформатированный адрес.
    /// </summary>
    public class GeoResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoResult"/> class.
        /// </summary>
        /// <param name="latitude">Широта.</param>
        /// <param name="longitude">Долгота.</param>
        /// <param name="formattedAddress">Отформатированный адрес.</param>
        public GeoResult(double latitude, double longitude, string formattedAddress)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.FormattedAddress = formattedAddress;
        }

        /// <summary>
        /// Gets the latitude.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Gets the longitude.
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Gets the formatted address.
        /// </summary>
        public string FormattedAddress { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.FormattedAddress} -> широта {this.Latitude}, долгота {this.Longitude}";
        }
    }
}
