using System;
using System.Collections.Generic;

namespace GeocodingSystem
{
    // =====================================================================
    // ВАРИАНТ 14. Система геокодирования
    // Определение координат по адресу с использованием API:
    // Google Maps, Yandex Maps, OpenStreetMap, Here Maps.
    // =====================================================================

    // Результат геокодирования — пара координат для адреса
    public class GeoResult
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public string FormattedAddress { get; }

        public GeoResult(double latitude, double longitude, string formattedAddress)
        {
            Latitude = latitude;
            Longitude = longitude;
            FormattedAddress = formattedAddress;
        }

        public override string ToString()
        {
            return $"{FormattedAddress} -> широта {Latitude}, долгота {Longitude}";
        }
    }

    // =====================================================================
    // ШАГ 1. Определение абстракции (интерфейса)
    // IGeocoder — центральная вершина звёздообразной сетевой структуры.
    // Основной модуль GeocodingService зависит ТОЛЬКО от этого интерфейса.
    // =====================================================================
    public interface IGeocoder
    {
        // Возвращает координаты по адресу, либо null, если адрес не найден.
        // Может бросить исключение, если сервис недоступен.
        GeoResult GetCoordinates(string address);
    }

    // =====================================================================
    // ШАГ 2. Реализация конкретных узлов сети
    // =====================================================================

    // Узел 1: Google Maps
    public class GoogleGeocoder : IGeocoder
    {
        private readonly string _apiKey;

        // База известных Google'у адресов (имитация ответа API)
        private readonly Dictionary<string, GeoResult> _database =
            new Dictionary<string, GeoResult>(StringComparer.OrdinalIgnoreCase)
            {
                { "Москва, Красная площадь, 1",
                  new GeoResult(55.753933, 37.620784, "Москва, Красная площадь, 1") },
                { "Санкт-Петербург, Невский проспект, 28",
                  new GeoResult(59.934280, 30.335198, "Санкт-Петербург, Невский проспект, 28") },
                { "London, Downing Street 10",
                  new GeoResult(51.503363, -0.127625, "10 Downing Street, London, UK") }
            };

        public GoogleGeocoder(string apiKey)
        {
            _apiKey = apiKey;
        }

        public GeoResult GetCoordinates(string address)
        {
            Console.WriteLine($"  [GOOGLE] Запрос к Google Maps Geocoding API (Key: {_apiKey})...");

            GeoResult result;
            if (_database.TryGetValue(address.Trim(), out result))
                return result;

            // "ZERO_RESULTS" — сервис работает, но адреса нет в базе
            return null;
        }
    }

    // Узел 2: Yandex Maps
    public class YandexGeocoder : IGeocoder
    {
        private readonly string _apiKey;

        private readonly Dictionary<string, GeoResult> _database =
            new Dictionary<string, GeoResult>(StringComparer.OrdinalIgnoreCase)
            {
                { "Москва, Красная площадь, 1",
                  new GeoResult(55.753930, 37.620800, "Москва, Красная площадь, 1") },
                { "г. Томск, проспект Ленина, 15",
                  new GeoResult(56.462800, 84.951400, "Томск, проспект Ленина, 15") },
                { "г. Казань, улица Баумана, 1",
                  new GeoResult(55.787893, 49.106414, "Казань, улица Баумана, 1") }
            };

        public YandexGeocoder(string apiKey)
        {
            _apiKey = apiKey;
        }

        public GeoResult GetCoordinates(string address)
        {
            Console.WriteLine($"  [YANDEX] Запрос к Geocoder API Яндекс.Карт (Key: {_apiKey})...");

            GeoResult result;
            if (_database.TryGetValue(address.Trim(), out result))
                return result;

            return null;
        }
    }

    // Узел 3: OpenStreetMap (публичный API, может отвечать с задержкой)
    public class OsmGeocoder : IGeocoder
    {
        private readonly string _serverUrl;

        private readonly Dictionary<string, GeoResult> _database =
            new Dictionary<string, GeoResult>(StringComparer.OrdinalIgnoreCase)
            {
                { "г. Иркутск, ул. Карла Маркса, 1",
                  new GeoResult(52.286199, 104.305303, "Иркутск, улица Карла Маркса, 1") },
                { "г. Владивосток, улица Светланская, 33",
                  new GeoResult(43.115542, 131.885494, "Владивосток, улица Светланская, 33") }
            };

        public OsmGeocoder(string serverUrl)
        {
            _serverUrl = serverUrl;
        }

        public GeoResult GetCoordinates(string address)
        {
            Console.WriteLine($"  [OSM] Запрос к {_serverUrl}...");

            GeoResult result;
            if (_database.TryGetValue(address.Trim(), out result))
                return result;

            // Имитация таймаута публичного Nominatim API при неизвестном адресе
            throw new TimeoutException("Превышено время ожидания ответа (Nominatim API)");
        }
    }

    // =====================================================================
    // ШАГ 3. Основной модуль (слабая связность)
    // GeocodingService зависит только от абстракции IGeocoder.
    // Он не знает ни о GoogleGeocoder, ни о YandexGeocoder, ни о OsmGeocoder —
    // конкретные сервисы подставляются извне через конструктор (DI).
    // =====================================================================
    public class GeocodingService
    {
        private readonly List<IGeocoder> _geocoders;

        // Внедрение зависимостей через конструктор
        public GeocodingService(List<IGeocoder> geocoders)
        {
            _geocoders = geocoders;
        }

        // Пробует все сервисы по очереди и возвращает результат первого успеха
        public GeoResult Geocode(string address)
        {
            Console.WriteLine($"\nЗапрос координат: \"{address}\"");

            foreach (IGeocoder geocoder in _geocoders)
            {
                try
                {
                    GeoResult result = geocoder.GetCoordinates(address);

                    if (result != null)
                    {
                        string name = geocoder.GetType().Name;
                        Console.WriteLine($"  [OK] {name} определил координаты: {result}");
                        return result;
                    }

                    Console.WriteLine($"  [...] {geocoder.GetType().Name}: адрес не найден, пробуем следующий сервис");
                }
                catch (Exception ex)
                {
                    // Недоступность одного сервиса не ломает всю систему
                    Console.WriteLine($"  [!] {geocoder.GetType().Name}: сервис недоступен ({ex.Message}), пробуем следующий");
                }
            }

            Console.WriteLine("  [FAIL] Ни один сервис не смог определить координаты");
            return null;
        }
    }

    // =====================================================================
    // ШАГ 4. Сборка сети (композиция) — точка входа, здесь строится граф
    // =====================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine(" СИСТЕМА ГЕОКОДИРОВАНИЯ (Вариант 14) \n");

            // 1. Создаём узлы сети (конкретные реализации IGeocoder)
            IGeocoder google = new GoogleGeocoder("AIza_SY_GOOGLE_KEY");
            IGeocoder yandex = new YandexGeocoder("trnru_YYANDEX_KEY");
            IGeocoder osm = new OsmGeocoder("https://nominatim.openstreetmap.org");

            // 2. Формируем сеть: передаём все узлы в основной модуль
            var geocoders = new List<IGeocoder> { google, yandex, osm };

            var geocodingService = new GeocodingService(geocoders);

            // 3. Используем систему: пробуем сервисы до первого успеха
            Console.WriteLine("\n=== БАЗОВЫЙ ЗАПУСК (Google, Yandex, OSM) ===");

            // Адрес знает только Google — успех с первого сервиса
            geocodingService.Geocode("Москва, Красная площадь, 1");

            // Google не знает — пробуем Yandex, он находит
            geocodingService.Geocode("г. Томск, проспект Ленина, 15");

            // Google и Yandex не знают — успех с третьего сервиса
            geocodingService.Geocode("г. Иркутск, ул. Карла Маркса, 1");

            // Ни один сервис не знает (OSM отвечает таймаутом, ошибка перехвачена)
            geocodingService.Geocode("с. Ромашково, ул. Полевая, 7");

            // 4. Демонстрация расширяемости: новый узел сети БЕЗ изменения
            //    основного модуля GeocodingService (принцип Open/Closed)
            Console.WriteLine("\n=== РАСШИРЕНИЕ: добавляем HereGeocoder (GeocodingService не изменялся!) ===");

            IGeocoder here = new HereGeocoder("HERE_API_KEY");
            var extendedGeocoders = new List<IGeocoder>(geocoders) { here };
            var extendedService = new GeocodingService(extendedGeocoders);

            // Адрес, которого нет ни у одного из старых сервисов — находит HERE
            extendedService.Geocode("Екатеринбург, проспект Ленина, 50");

            // Ранее не найденный адрес теперь находится через новый компонент
            extendedService.Geocode("с. Ромашково, ул. Полевая, 7");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            if (!Console.IsInputRedirected)
                Console.ReadKey();
        }
    }

    // =====================================================================
    // ШАГ 5. Новый узел сети — добавлен БЕЗ изменения GeocodingService.
    // Достаточно реализовать интерфейс IGeocoder и подключить его в Program.
    // =====================================================================
    public class HereGeocoder : IGeocoder
    {
        private readonly string _apiKey;

        private readonly Dictionary<string, GeoResult> _database =
            new Dictionary<string, GeoResult>(StringComparer.OrdinalIgnoreCase)
            {
                { "Екатеринбург, проспект Ленина, 50",
                  new GeoResult(56.837430, 60.614490, "Екатеринбург, проспект Ленина, 50") },
                { "с. Ромашково, ул. Полевая, 7",
                  new GeoResult(55.560130, 37.440420, "Ромашково, улица Полевая, 7") }
            };

        public HereGeocoder(string apiKey)
        {
            _apiKey = apiKey;
        }

        public GeoResult GetCoordinates(string address)
        {
            Console.WriteLine($"  [HERE] Запрос к HERE Geocoding API (Key: {_apiKey})...");

            GeoResult result;
            if (_database.TryGetValue(address.Trim(), out result))
                return result;

            return null;
        }
    }
}
