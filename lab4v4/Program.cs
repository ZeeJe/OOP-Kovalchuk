using System;

namespace Program
{
    class Program
    {
        static void Main()
        {
            var manager = new TripManager(); // створення керівника поїздок

            // Додаємо різні типи поїздок
            manager.AddTrip(new CityTransport(5.0), 3, 12.5);       // міська
            manager.AddTrip(new IntercityTransport(2.5), 2, 150.0); // міжміська
            manager.AddTrip(new CityTransport(5.0), 1, 8.0);        // міська

            // Вивід результатів
            manager.PrintReport();
        }
    }
}
