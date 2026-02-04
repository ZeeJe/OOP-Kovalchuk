using System;

namespace lab22
{
    // --- Початкова ієрархія (Порушення LSP) ---
    public class OldFile
    {
        public virtual void Read() => Console.WriteLine("Читання даних з файлу...");
        public virtual void Write(string content) => Console.WriteLine("Запис даних у файл...");
    }

    public class OldReadOnlyFile : OldFile
    {
        public override void Write(string content)
        {
            throw new NotSupportedException("Помилка: Неможливо записати в файл тільки для читання!");
        }
    }

    // --- Рефакторинг (Дотримання LSP) ---
    public interface IReadable
    {
        void Read();
    }

    public interface IWritable : IReadable
    {
        void Write(string content);
    }

    public class SafeReadOnlyFile : IReadable
    {
        public void Read() => Console.WriteLine("SafeReadOnlyFile: Читання завершено.");
    }

    public class SafeWritableFile : IWritable
    {
        public void Read() => Console.WriteLine("SafeWritableFile: Читання завершено.");
        public void Write(string content) => Console.WriteLine($"SafeWritableFile: Дані '{content}' записано.");
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ЧАСТИНА 1: ПОРУШЕННЯ LSP ===");
            OldFile badFile = new OldReadOnlyFile();
            try
            {
                badFile.Read();
                badFile.Write("Тест");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine("\n=== ЧАСТИНА 2: ДОТРИМАННЯ LSP ===");
            
            IReadable reader = new SafeReadOnlyFile();
            IWritable writer = new SafeWritableFile();

            DemonstrateReading(reader);
            DemonstrateWriting(writer);

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }

        static void DemonstrateReading(IReadable file) => file.Read();

        static void DemonstrateWriting(IWritable file)
        {
            file.Write("Новий вміст");
            file.Read();
        }
    }
}