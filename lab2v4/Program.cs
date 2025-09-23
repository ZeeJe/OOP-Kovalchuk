class Program
{
    static void Main(string[] args)
    {
        ComplexNumber c1 = new ComplexNumber(3, 4);
        ComplexNumber c2 = new ComplexNumber(1, -2);
        Console.WriteLine("c1 = " + c1);
        Console.WriteLine("c2 = " + c2);
        c1.Real = 5;
        Console.WriteLine("c1.Real = " + c1.Real);
        Console.WriteLine("c2[1] (imag) = " + c2[1]);
        c2[0] = 2;
        Console.WriteLine("c2[0] (real) = " + c2[0]);
        ComplexNumber sum = c1 + c2;
        ComplexNumber diff = c1 - c2;
        ComplexNumber prod = c1 * c2;
        ComplexNumber quot = c1 / c2;
        Console.WriteLine("c1 + c2 = " + sum);
        Console.WriteLine("c1 - c2 = " + diff);
        Console.WriteLine("c1 * c2 = " + prod);
        Console.WriteLine("c1 / c2 = " + quot);
    }
}
