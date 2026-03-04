using System;

namespace BeforeRefactoring
{
    // Порушення ISP: загальний інтерфейс змушує реалізовувати всі методи
    public interface IMachine
    {
        void Print();
        void Scan();
        void Fax();
    }

    public class PrinterModule
    {
        public void Print() => Console.WriteLine("Друк документа...");
    }

    public class ScannerModule
    {
        public void Scan() => Console.WriteLine("Сканування документа...");
    }

    public class FaxModule
    {
        public void Fax() => Console.WriteLine("Відправка факсу...");
    }

    public class SmartMachine : IMachine
    {
        private PrinterModule _printer;
        private ScannerModule _scanner;
        private FaxModule _fax;

        public SmartMachine()
        {
            // Порушення DIP: жорстке створення об'єктів низького рівня через new
            _printer = new PrinterModule(); 
            _scanner = new ScannerModule(); 
            _fax = new FaxModule();         
        }

        public void Print() => _printer.Print();
        public void Scan() => _scanner.Scan();
        public void Fax() => _fax.Fax();
    }

    public class SimplePrinter : IMachine
    {
        private PrinterModule _printer = new PrinterModule();

        public void Print() => _printer.Print();

        // Порушення ISP: реалізація непотрібних методів через "товстий" інтерфейс
        public void Scan() 
        {
            throw new NotImplementedException("Звичайний принтер не вміє сканувати.");
        }

        public void Fax() 
        {
            throw new NotImplementedException("Звичайний принтер не має факсу.");
        }
    }
}

namespace AfterRefactoring
{
    // Вирішення ISP: розділення на вузькоспеціалізовані інтерфейси
    public interface IPrinter
    {
        void Print();
    }

    public interface IScanner
    {
        void Scan();
    }

    public interface IFax
    {
        void Fax();
    }

    // Модулі реалізують лише необхідні їм інтерфейси
    public class PrinterModule : IPrinter
    {
        public void Print() => Console.WriteLine("Друк документа...");
    }

    public class ScannerModule : IScanner
    {
        public void Scan() => Console.WriteLine("Сканування документа...");
    }

    public class FaxModule : IFax
    {
        public void Fax() => Console.WriteLine("Відправка факсу...");
    }

    // SmartMachine підтримує всі функції, реалізуючи відповідні інтерфейси
    public class SmartMachine : IPrinter, IScanner, IFax
    {
        private readonly IPrinter _printer;
        private readonly IScanner _scanner;
        private readonly IFax _fax;

        // Вирішення DIP: залежності (абстракції) передаються через конструктор
        public SmartMachine(IPrinter printer, IScanner scanner, IFax fax)
        {
            _printer = printer;
            _scanner = scanner;
            _fax = fax;
        }

        public void Print() => _printer.Print();
        public void Scan() => _scanner.Scan();
        public void Fax() => _fax.Fax();
    }

    // SimplePrinter реалізує лише друк, позбувшись зайвих методів
    public class SimplePrinter : IPrinter
    {
        private readonly IPrinter _printer;

        // Залежність лише від принтера
        public SimplePrinter(IPrinter printer)
        {
            _printer = printer;
        }

        public void Print() => _printer.Print();
    }
}

class Program
{
    static void Main()
    {
        // Коректне відображення кирилиці в консолі
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("--- Демонстрація роботи після рефакторингу ---");

        // 1. Створення залежностей (модулів нижчого рівня)
        AfterRefactoring.IPrinter printerModule = new AfterRefactoring.PrinterModule();
        AfterRefactoring.IScanner scannerModule = new AfterRefactoring.ScannerModule();
        AfterRefactoring.IFax faxModule = new AfterRefactoring.FaxModule();

        // 2. Впровадження залежностей (DI) у багатофункціональний пристрій
        AfterRefactoring.SmartMachine smartMachine = new AfterRefactoring.SmartMachine(printerModule, scannerModule, faxModule);
        Console.WriteLine("\n[SmartMachine]:");
        smartMachine.Print();
        smartMachine.Scan();
        smartMachine.Fax();

        // 3. SimplePrinter отримує лише той модуль, який йому потрібен для роботи
        AfterRefactoring.SimplePrinter simplePrinter = new AfterRefactoring.SimplePrinter(printerModule);
        Console.WriteLine("\n[SimplePrinter]:");
        simplePrinter.Print();
    }
}