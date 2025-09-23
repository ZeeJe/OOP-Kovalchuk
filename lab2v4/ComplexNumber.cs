class ComplexNumber
{
    private double real;
    private double imag;
    public double Real
    {
        get { return real; }
        set { real = value; }
    }
    public double Imag
    {
        get { return imag; }
        set { imag = value; }
    }
    public ComplexNumber(double r, double i)
    {
        real = r;
        imag = i;
    }
    public double this[int index]
    {
        get { return index == 0 ? real : imag; }
        set { if(index == 0) real = value; else imag = value; }
    }
    public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.real + b.real, a.imag + b.imag);
    }
    public static ComplexNumber operator -(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.real - b.real, a.imag - b.imag);
    }
    public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.real * b.real - a.imag * b.imag, a.real * b.imag + a.imag * b.real);
    }
    public static ComplexNumber operator /(ComplexNumber a, ComplexNumber b)
    {
        double denom = b.real * b.real + b.imag * b.imag;
        return new ComplexNumber((a.real * b.real + a.imag * b.imag)/denom,
                                 (a.imag * b.real - a.real * b.imag)/denom);
    }
    public override string ToString()
    {
        return $"{real} + {imag}i";
    }
}