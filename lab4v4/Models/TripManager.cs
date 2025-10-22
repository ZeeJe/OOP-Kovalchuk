using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Lab4
{
    public class TripManager
    {
        private readonly List<(Transport transport, int passengers, double distance)> _trips = new();

        public void AddTrip(Transport transport, int passengers, double distance)
        {
            _trips.Add((transport, passengers, distance));
        }

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
