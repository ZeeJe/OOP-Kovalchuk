using System;

namespace OOP_Lab4
{
    class Program
    {
        static void Main()
        {
            var manager = new TripManager();

            manager.AddTrip(new CityTransport(5.0), 3, 12.5);
            manager.AddTrip(new IntercityTransport(2.5), 2, 150.0);
            manager.AddTrip(new CityTransport(5.0), 1, 8.0);

            manager.PrintReport();
        }
    }
}
