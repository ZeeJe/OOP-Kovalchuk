using System;
using System.Collections.Generic;

namespace lab21
{
    // --- СТРАТЕГІЇ (OCP) ---

    public interface ITariffStrategy
    {
        decimal CalculateCost(int minutes, int gigabytes);
    }

    // Передплата: низька фіксована ціна за одиницю
    public class PrepaidTariffStrategy : ITariffStrategy
    {
        public decimal CalculateCost(int minutes, int gigabytes) => (minutes * 0.5m) + (gigabytes * 10m);
    }

    // Контракт: абонплата + дешевші хвилини
    public class ContractTariffStrategy : ITariffStrategy
    {
        public decimal CalculateCost(int minutes, int gigabytes) => 100m + (minutes * 0.2m) + (gigabytes * 5m);
    }

    // Безліміт: висока фіксована вартість
    public class UnlimitedTariffStrategy : ITariffStrategy
    {
        public decimal CalculateCost(int minutes, int gigabytes) => 500m;
    }

    // НОВИЙ ТАРИФ (Демонстрація OCP): Нічний безліміт (Night)
    public class NightTariffStrategy : ITariffStrategy
    {
        public decimal CalculateCost(int minutes, int gigabytes) => 250m + (minutes * 0.1m);
    }

    // --- ФАБРИКА (Factory Method) ---

    public static class TariffStrategyFactory
    {
        public static ITariffStrategy CreateStrategy(string tariffType)
        {
            return tariffType.ToLower() switch
            {
                "prepaid" => new PrepaidTariffStrategy(),
                "contract" => new ContractTariffStrategy(),
                "unlimited" => new UnlimitedTariffStrategy(),
                "night" => new NightTariffStrategy(), // Додано без зміни DeliveryService
                _ => throw new ArgumentException("Невідомий тип тарифу")
            };
        }
    }

    // --- СЕРВІС (Закритий для змін, відкритий для розширення стратегій) ---

    public class BillingService
    {
        public decimal CalculateTotal(int minutes, int gigabytes, ITariffStrategy strategy)
        {
            return strategy.CalculateCost(minutes, gigabytes);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var billingService = new BillingService();

            try
            {
                Console.WriteLine("Введіть тип тарифу (Prepaid, Contract, Unlimited, Night):");
                string type = Console.ReadLine();

                Console.WriteLine("Введіть кількість використаних хвилин:");
                int mins = int.Parse(Console.ReadLine());

                Console.WriteLine("Введіть кількість використаних ГБ:");
                int gbs = int.Parse(Console.ReadLine());

                ITariffStrategy strategy = TariffStrategyFactory.CreateStrategy(type);
                decimal cost = billingService.CalculateTotal(mins, gbs, strategy);

                Console.WriteLine($"\nРезультат розрахунку для тарифу '{type}':");
                Console.WriteLine($"Загальна вартість: {cost} грн.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}