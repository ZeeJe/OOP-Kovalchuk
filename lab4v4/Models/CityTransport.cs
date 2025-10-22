namespace OOP_Lab4
{
    public class CityTransport : Transport
    {
        public CityTransport(double ratePerKm)
        {
            Name = "Міський транспорт";
            BaseRate = ratePerKm;
        }

        public override double CalculateFare(int passengers, double distance)
        {
            return passengers * distance * BaseRate;
        }
    }
}
