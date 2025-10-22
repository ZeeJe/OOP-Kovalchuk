namespace OOP_Lab4
{
    public class IntercityTransport : Transport
    {
        public IntercityTransport(double ratePerKm)
        {
            Name = "Міжміський транспорт";
            BaseRate = ratePerKm;
        }

        public override double CalculateFare(int passengers, double distance)
        {
            double total = passengers * distance * BaseRate;
            if (distance > 100)
                total *= 0.9; // знижка 10%
            return total;
        }
    }
}
