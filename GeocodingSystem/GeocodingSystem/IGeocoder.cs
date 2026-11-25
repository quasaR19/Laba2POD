// <copyright file="IGeocoder.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

namespace GeocodingSystem
{
    /// <summary>
    /// Абстракция геокодера. Центральная вершина звёздообразной сетевой структуры.
    /// </summary>
    public interface IGeocoder
    {
        /// <summary>
        /// Возвращает координаты по адресу.
        /// </summary>
        /// <param name="address">Адрес в свободной форме.</param>
        /// <returns>Координаты либо <c>null</c>, если адрес не найден.</returns>
        /// <exception cref="GeocodingException">Если сервис недоступен.</exception>
        GeoResult GetCoordinates(string address);
    }
}
