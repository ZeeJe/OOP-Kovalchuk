using System;
using System.Collections.Generic;
using System.Linq;

namespace TripManager
{
    // 🔹 Клас-композиція — зберігає та обробляє поїздки
    public class TripManager
    {
        // Список поїздок (композиція: Transport є частиною TripManager)
        private readonly List<(Transport transport, int passengers, double distance)> _trips = new();

        // Додає поїздку до списку
        public void AddTrip(Transport transport, int passengers, double distance)
        {
            _trips.Add((transport, passengers, distance));
        }

        // Виводить усі поїздки та обчислює загальну/середню вартість
        public void PrintReport()
        {
            Console.WriteLine("🚗 Звіт по поїздках:\n");

            foreach (var trip in _trips)
            {
                double fare = trip.transport.CalculateFare(trip.passengers, trip.distance);
                Console.WriteLine($"{trip.transport}: {trip.passengers} пасажирів, {trip.distance} км → {fare:F2} грн");
            }

            var total = _trips.Sum(t => t.transport.CalculateFare(t.passengers, t.distance));
            var avg = _trips.Average(t => t.transport.CalculateFare(t.passengers, t.distance));

            Console.WriteLine($"\nЗагальна вартість: {total:F2} грн");
            Console.WriteLine($"Середня вартість поїздки: {avg:F2} грн");
        }
    }
}
