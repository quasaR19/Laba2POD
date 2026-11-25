// <copyright file="GeocodingException.cs" company="BSUIR">
// Copyright (c) BSUIR. All rights reserved.
// </copyright>

using System;
using System.Runtime.Serialization;

namespace GeocodingSystem
{
    /// <summary>
    /// Исключение уровня геокодирования.
    /// </summary>
    [Serializable]
    public class GeocodingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodingException"/> class.
        /// </summary>
        public GeocodingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodingException"/> class.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public GeocodingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodingException"/> class.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="inner">Внутреннее исключение.</param>
        public GeocodingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodingException"/> class.
        /// </summary>
        /// <param name="info">Сведения о сериализации.</param>
        /// <param name="context">Контекст потока.</param>
        protected GeocodingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
