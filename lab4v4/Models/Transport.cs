namespace OOP_Lab4
{
    public abstract class Transport : IFareCalculator
    {
        public string Name { get; protected set; }
        public double BaseRate { get; protected set; }
        public abstract double CalculateFare(int passengers, double distance);

        public override string ToString() => $"{Name} (тариф {BaseRate:F2} грн/км)";
    }
}
