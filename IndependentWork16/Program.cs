using System;
using System.Collections.Generic;

namespace IndependentWork16
{
    // Модель даних для звіту
    public class ReportData
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    // --- ПОЧАТКОВА РЕАЛІЗАЦІЯ (ПОРУШЕННЯ SRP) ---
    public class ReportGenerator
    {
        public void CreateReport()
        {
            // 1. Отримання даних
            Console.WriteLine("[ReportGenerator] Отримання даних з бази...");
            var data = "Сирі дані звіту";

            // 2. Обробка даних
            Console.WriteLine("[ReportGenerator] Обробка та аналіз даних...");
            var processedData = data.ToUpper();

            // 3. Форматування в PDF
            Console.WriteLine("[ReportGenerator] Форматування даних у PDF формат...");

            // 4. Збереження на диск
            Console.WriteLine("[ReportGenerator] Збереження файлу report.pdf на диск...");
        }
    }

    // --- РЕФАКТОРИНГ ЗА SRP ТА DIP ---

    public interface IDataSource { string GetData(); }
    public interface IDataProcessor { string Process(string rawData); }
    public interface IPdfFormatter { byte[] FormatToPdf(string data); }
    public interface IFileSaver { void Save(byte[] fileContent, string fileName); }

    // Реалізації-"заглушки"
    public class DatabaseSource : IDataSource 
    { 
        public string GetData() => "Дані з бази даних"; 
    }

    public class AnalyticsProcessor : IDataProcessor 
    { 
        public string Process(string rawData) => $"Оброблено: {rawData}"; 
    }

    public class SimplePdfFormatter : IPdfFormatter 
    { 
        public byte[] FormatToPdf(string data) => System.Text.Encoding.UTF8.GetBytes(data); 
    }

    public class DiskFileSaver : IFileSaver 
    { 
        public void Save(byte[] content, string name) => Console.WriteLine($"[FileSaver] Файл {name} успішно збережено."); 
    }

    // Головний сервіс (ReportService)
    public class ReportService
    {
        private readonly IDataSource _source;
        private readonly IDataProcessor _processor;
        private readonly IPdfFormatter _formatter;
        private readonly IFileSaver _saver;

        public ReportService(IDataSource source, IDataProcessor processor, IPdfFormatter formatter, IFileSaver saver)
        {
            _source = source;
            _processor = processor;
            _formatter = formatter;
            _saver = saver;
        }

        public void Generate()
        {
            string raw = _source.GetData();
            string processed = _processor.Process(raw);
            byte[] pdf = _formatter.FormatToPdf(processed);
            _saver.Save(pdf, "FinalReport.pdf");
            
            Console.WriteLine("Генерація звіту завершена успішно.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ТЕСТ: ПОЧАТКОВИЙ КЛАС (ПОРУШЕННЯ SRP) ===");
            var badGen = new ReportGenerator();
            badGen.CreateReport();

            Console.WriteLine("\n=== ТЕСТ: РЕФАКТОРИНГ (SRP + DIP) ===");
            var service = new ReportService(
                new DatabaseSource(),
                new AnalyticsProcessor(),
                new SimplePdfFormatter(),
                new DiskFileSaver()
            );

            service.Generate();
        }
    }
}