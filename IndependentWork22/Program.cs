using System;
using System.Collections.Generic;

namespace IndependentWork22
{
    // =========================================================================
    // 1. ПАТЕРН COMPOSITE (Компонувальник)
    // =========================================================================

    // Спільний інтерфейс для поодиноких товарів та комплексних наборів
    public interface IComponent
    {
        decimal GetPrice();
        string GetName(); // Додатковий метод для інформативного виводу в консоль
    }

    // Клас Leaf (Лист) — Окремий продукт без підкомпонентів
    public class SingleProduct : IComponent
    {
        private string _name;
        private decimal _price;

        public SingleProduct(string name, decimal price)
        {
            _name = name;
            _price = price;
        }

        public decimal GetPrice() => _price;
        public string GetName() => _name;
    }

    // Клас Composite (Композит) — Комплект продуктів (набір, що містить інші IComponent)
    public class ProductBundle : IComponent
    {
        private string _name;
        private readonly List<IComponent> _components = new List<IComponent>();

        public ProductBundle(string name)
        {
            _name = name;
        }

        // Методи керування дочірніми елементами
        public void Add(IComponent component)
        {
            _components.Add(component);
        }

        public void Remove(IComponent component)
        {
            _components.Remove(component);
        }

        // Рекурсивно підраховує суму цін усіх вкладених елементів
        public decimal GetPrice()
        {
            decimal totalPrice = 0;
            foreach (var component in _components)
            {
                totalPrice += component.GetPrice();
            }
            return totalPrice;
        }

        public string GetName() => _name;
    }

    // =========================================================================
    // 2. ПАТЕРН DECORATOR (Декоратор)
    // =========================================================================

    // Абстрактний декоратор, що реалізує спільний інтерфейс та зберігає посилання на компонент
    public abstract class ProductDecorator : IComponent
    {
        protected IComponent _component;

        protected ProductDecorator(IComponent component)
        {
            _component = component;
        }

        public virtual decimal GetPrice() => _component.GetPrice();
        public virtual string GetName() => _component.GetName();
    }

    // Конкретний декоратор: Знижка (зменшує ціну на відсоток)
    public class DiscountDecorator : ProductDecorator
    {
        private decimal _discountPercentage;

        public DiscountDecorator(IComponent component, decimal discountPercentage) : base(component)
        {
            _discountPercentage = discountPercentage;
        }

        public override decimal GetPrice()
        {
            decimal originalPrice = base.GetPrice();
            return originalPrice - (originalPrice * (_discountPercentage / 100));
        }

        public override string GetName() => $"{base.GetName()} (зі знижкою {_discountPercentage}%)";
    }

    // Конкретний декоратор: Податок (збільшує ціну на податок / ПДВ)
    public class TaxDecorator : ProductDecorator
    {
        private decimal _taxPercentage;

        public TaxDecorator(IComponent component, decimal taxPercentage) : base(component)
        {
            _taxPercentage = taxPercentage;
        }

        public override decimal GetPrice()
        {
            decimal originalPrice = base.GetPrice();
            return originalPrice + (originalPrice * (_taxPercentage / 100));
        }

        public override string GetName() => $"{base.GetName()} (+ ПДВ {_taxPercentage}%)";
    }

    // =========================================================================
    // 3. ТОЧКА ВХОДУ (Демонстрація взаємодії)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // Встановлюємо кодування для коректного виводу гривні в консоль
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Самостійна робота №22: Варіант 4 (Продукти та комплекти) ===\n");

            // а) Створення базових продуктів (Leaf)
            IComponent laptop = new SingleProduct("Ноутбук геймерський", 45000m);
            IComponent mouse = new SingleProduct("Мишка бездротова", 1500m);
            IComponent keyboard = new SingleProduct("Механічна клавіатура", 3000m);
            IComponent headphones = new SingleProduct("Навушники з мікрофоном", 3500m);

            Console.WriteLine("--- Базові поодинокі товари ---");
            Console.WriteLine($"{laptop.GetName()} -> Ціна: {laptop.GetPrice()} грн");
            Console.WriteLine($"{mouse.GetName()} -> Ціна: {mouse.GetPrice()} грн\n");

            // б) Створення деревоподібної структури (Composite)
            ProductBundle peripheralsSet = new ProductBundle("Комплект периферії 'Базовий'");
            peripheralsSet.Add(mouse);
            peripheralsSet.Add(keyboard);

            ProductBundle workstationBundle = new ProductBundle("Супер-Пак 'Робоче місце під ключ'");
            workstationBundle.Add(laptop);
            workstationBundle.Add(peripheralsSet); // Додаємо композит всередину іншого композиту
            workstationBundle.Add(headphones);

            Console.WriteLine("--- Складені комплекти (Composite) ---");
            Console.WriteLine($"{peripheralsSet.GetName()} -> Загальна вартість: {peripheralsSet.GetPrice()} грн");
            Console.WriteLine($"{workstationBundle.GetName()} -> Загальна вартість: {workstationBundle.GetPrice()} грн\n");

            // в) Застосування динамічних обгорток (Decorator)
            Console.WriteLine("--- Модифікація цін за допомогою декораторів ---");

            // 1. Декоруємо один конкретний товар знижкою 10%
            IComponent discountedLaptop = new DiscountDecorator(laptop, 10);
            Console.WriteLine($"{discountedLaptop.GetName()} -> Нова ціна: {discountedLaptop.GetPrice()} грн");

            // 2. Декоруємо цілий комплект податком (наприклад, ПДВ 20%)
            IComponent taxedPeripherals = new TaxDecorator(peripheralsSet, 20);
            Console.WriteLine($"{taxedPeripherals.GetName()} -> Вартість з ПДВ: {taxedPeripherals.GetPrice()} грн");

            // 3. Комбінація декораторів (накладаємо спочатку знижку 15% на Супер-Пак, а потім зверху рахуємо ПДВ 20%)
            IComponent promoBundle = new DiscountDecorator(workstationBundle, 15);
            IComponent finalTaxedPromoBundle = new TaxDecorator(promoBundle, 20);

            Console.WriteLine("\n--- Комбіноване декорування всього кошика (Знижка + ПДВ) ---");
            Console.WriteLine($"{finalTaxedPromoBundle.GetName()} \n-> Фінальна сума до сплати: {finalTaxedPromoBundle.GetPrice()} грн");

            Console.ReadLine();
        }
    }
}