using System;

namespace IndependentWork19
{
    // 1. Інтерфейс (замість ILogger)
    public interface IRenderer
    {
        void Render(string objectToRender);
    }

    // 2. Конкретні реалізації рендерерів (замість ConsoleLogger, FileLogger тощо)
    public class OpenGLRenderer : IRenderer
    {
        public void Render(string objectToRender)
        {
            Console.WriteLine($"[OpenGL] Рендеринг об'єкта: {objectToRender}");
        }
    }

    public class DirectXRenderer : IRenderer
    {
        public void Render(string objectToRender)
        {
            Console.WriteLine($"[DirectX] Рендеринг об'єкта: {objectToRender}");
        }
    }

    public class VulkanRenderer : IRenderer
    {
        public void Render(string objectToRender)
        {
            Console.WriteLine($"[Vulkan] Рендеринг об'єкта: {objectToRender}");
        }
    }

    // 3. Абстрактний клас фабрики (замість LoggerFactory)
    public abstract class RendererFactory
    {
        // Абстрактний метод, який реалізують підкласи (Factory Method)
        protected abstract IRenderer CreateRenderer();

        // Конкретний метод, що використовує створений об'єкт
        public void RenderShape(string objectToRender)
        {
            IRenderer renderer = CreateRenderer();
            renderer.Render(objectToRender);
        }
    }

    // 4. Конкретні фабрики для кожного типу рендерера
    public class OpenGLRendererFactory : RendererFactory
    {
        protected override IRenderer CreateRenderer()
        {
            return new OpenGLRenderer();
        }
    }

    public class DirectXRendererFactory : RendererFactory
    {
        protected override IRenderer CreateRenderer()
        {
            return new DirectXRenderer();
        }
    }

    public class VulkanRendererFactory : RendererFactory
    {
        protected override IRenderer CreateRenderer()
        {
            return new VulkanRenderer();
        }
    }

    // 5. Singleton клас для управління графікою (замість LoggerManager)
    public class GraphicsEngine
    {
        // Статичне поле для зберігання єдиного екземпляра
        private static GraphicsEngine? _instance;
        private RendererFactory? _currentFactory;
        
        // Приватний конструктор блокує створення через 'new'
        private GraphicsEngine() { }

        // Глобальна точка доступу до екземпляра
        public static GraphicsEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GraphicsEngine();
                }
                return _instance;
            }
        }

        // Метод для встановлення поточної фабрики
        public void SetRendererFactory(RendererFactory factory)
        {
            _currentFactory = factory;
            Console.WriteLine($"\n--- Двигун переключено на {factory.GetType().Name} ---");
        }

        // Делегування рендерингу поточній фабриці
        public void Render(string objectToRender)
        {
            if (_currentFactory == null)
            {
                Console.WriteLine("Помилка: Фабрика рендерингу не встановлена!");
                return;
            }
            _currentFactory.RenderShape(objectToRender);
        }
    }

    // 6. Демонстрація роботи
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Отримуємо єдиний екземпляр GraphicsEngine (Singleton)
            GraphicsEngine engine = GraphicsEngine.Instance;

            // Встановлюємо OpenGL і робимо кілька рендерів
            engine.SetRendererFactory(new OpenGLRendererFactory());
            engine.Render("3D Куб");
            engine.Render("Сфера");

            // Змінюємо фабрику на DirectX і робимо ще кілька рендерів
            engine.SetRendererFactory(new DirectXRendererFactory());
            engine.Render("Модель Гравця");
            engine.Render("Освітлення сцени");

            // Змінюємо фабрику на Vulkan і робимо ще кілька рендерів
            engine.SetRendererFactory(new VulkanRendererFactory());
            engine.Render("Система частинок (Вогонь)");
            engine.Render("Вода та віддзеркалення");

            Console.ReadLine();
        }
    }
}