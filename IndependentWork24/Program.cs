using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace IndependentWork24
{
    // =========================================================================
    // 1. ПАТЕРН COMPOSITE (Компонувальник)
    // =========================================================================
    public interface IComponent
    {
        decimal GetPrice();
        string GetName();
    }

    public class SingleProduct : IComponent
    {
        private readonly string _name;
        private readonly decimal _price;

        public SingleProduct(string name, decimal price)
        {
            if (price < 0) throw new ArgumentException("Ціна не може бути від'ємною.");
            _name = name;
            _price = price;
        }

        public decimal GetPrice()
        {
            // Імітація важкого розрахунку або запиту до БД (50 мс)
            Thread.Sleep(50); 
            return _price;
        }

        public string GetName() => _name;
    }

    public class ProductBundle : IComponent
    {
        private readonly string _name;
        private readonly List<IComponent> _components = new List<IComponent>();

        public ProductBundle(string name)
        {
            _name = name;
        }

        public void Add(IComponent component) => _components.Add(component);
        public void Remove(IComponent component) => _components.Remove(component);

        public decimal GetPrice()
        {
            decimal total = 0;
            foreach (var component in _components)
            {
                total += component.GetPrice();
            }
            return total;
        }

        public string GetName() => _name;
    }

    // =========================================================================
    // 2. ПАТЕРН DECORATOR (Декоратор)
    // =========================================================================
    public abstract class ProductDecorator : IComponent
    {
        protected readonly IComponent _component;

        protected ProductDecorator(IComponent component)
        {
            _component = component ?? throw new ArgumentNullException(nameof(component));
        }

        public virtual decimal GetPrice() => _component.GetPrice();
        public virtual string GetName() => _component.GetName();
    }

    public class DiscountDecorator : ProductDecorator
    {
        private readonly decimal _discountPercentage;

        public DiscountDecorator(IComponent component, decimal discountPercentage) : base(component)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentException("Знижка повинна бути в межах від 0 до 100%.");
            _discountPercentage = discountPercentage;
        }

        public override decimal GetPrice()
        {
            decimal basePrice = base.GetPrice();
            return basePrice - (basePrice * (_discountPercentage / 100));
        }

        public override string GetName() => $"{base.GetName()} (Знижка {_discountPercentage}%)";
    }

    // =========================================================================
    // 3. ПАТЕРН PROXY (Кешуючий заступник)
    // =========================================================================
    public class CachedComponentProxy : IComponent
    {
        private readonly IComponent _realComponent;
        private decimal? _cachedPrice;
        private bool _isCacheInvalid = true;

        public CachedComponentProxy(IComponent realComponent)
        {
            _realComponent = realComponent ?? throw new ArgumentNullException(nameof(realComponent));
        }

        public void InvalidateCache()
        {
            _isCacheInvalid = true;
            _cachedPrice = null;
        }

        public decimal GetPrice()
        {
            // Якщо кеш невалідний або порожній — робимо реальний обхід дерева
            if (_isCacheInvalid || !_cachedPrice.HasValue)
            {
                _cachedPrice = _realComponent.GetPrice();
                _isCacheInvalid = false;
            }
            return _cachedPrice.Value;
        }

        public string GetName() => _realComponent.GetName();
        public bool IsCacheValid() => !_isCacheInvalid; // Для тестування
    }

    // =========================================================================
    // ДЕМОНСТРАЦІЯ ТА ОЦІНКА ПРОДУКТИВНОСТІ
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №24: Варіант 4 ===");

            // Створення дерева товарів (Composite)
            var laptop = new SingleProduct("Ноутбук", 42000m);
            var mouse = new SingleProduct("Мишка", 1500m);
            var keyboard = new SingleProduct("Клавіатура", 2500m);

            var peripherals = new ProductBundle("Периферія");
            peripherals.Add(mouse);
            peripherals.Add(keyboard);

            var mainCart = new ProductBundle("Головний кошик");
            mainCart.Add(laptop);
            mainCart.Add(peripherals);

            // Накладання акції (Decorator)
            var promoCart = new DiscountDecorator(mainCart, 10); // 10% акція

            // Огортання в оптимізаційний проксі (Proxy)
            var secureCachedCart = new CachedComponentProxy(promoCart);

            Console.WriteLine($"\nСтруктура: {secureCachedCart.GetName()}");

            // Замір 1: Перший прохід (Кеш пустий)
            var sw = Stopwatch.StartNew();
            decimal price1 = secureCachedCart.GetPrice();
            sw.Stop();
            Console.WriteLine($"\n[Виклик 1]: Ціна = {price1} грн. Час: {sw.ElapsedMilliseconds} мс (Повний прохід)");

            // Замір 2: Повторний прохід (Працює Proxy-кеш)
            sw.Restart();
            decimal price2 = secureCachedCart.GetPrice();
            sw.Stop();
            Console.WriteLine($"[Виклик 2]: Ціна = {price2} грн. Час: {sw.ElapsedMilliseconds} мс (Миттєво з кешу)");

            // Замір 3: Скидання кешу
            secureCachedCart.InvalidateCache();
            sw.Restart();
            decimal price3 = secureCachedCart.GetPrice();
            sw.Stop();
            Console.WriteLine($"[Виклик 3 після скидання кешу]: Ціна = {price3} грн. Час: {sw.ElapsedMilliseconds} мс");

            Console.ReadLine();
        }
    }
}