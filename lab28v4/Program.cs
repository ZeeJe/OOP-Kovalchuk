using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace lab28v4;

// --- 1. Класи предметної області (Варіант 4) ---

public class Actor
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public List<Actor> Actors { get; set; } = new List<Actor>();
}

// --- 2. Репозиторій з JSON-серіалізацією ---

public class MovieRepository
{
    private List<Movie> _movies = new List<Movie>();

    // Метод додавання
    public void Add(Movie movie)
    {
        if (movie == null) throw new ArgumentNullException(nameof(movie));
        _movies.Add(movie);
    }

    // Метод отримання всіх записів
    public IEnumerable<Movie> GetAll()
    {
        return _movies;
    }

    // Метод отримання за Id
    public Movie? GetById(int id)
    {
        return _movies.FirstOrDefault(m => m.Id == id);
    }

    // Асинхронне збереження у JSON файл
    public async Task SaveToFileAsync(string filename)
    {
        // Налаштування для красивого форматування JSON (з відступами)
        var options = new JsonSerializerOptions { WriteIndented = true };
        
        // Використовуємо FileStream для ефективного запису файлу
        await using FileStream createStream = File.Create(filename);
        await JsonSerializer.SerializeAsync(createStream, _movies, options);
    }

    // Асинхронне завантаження з JSON файлу
    public async Task LoadFromFileAsync(string filename)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine($"Файл {filename} не знайдено.");
            return;
        }

        await using FileStream openStream = File.OpenRead(filename);
        var loadedMovies = await JsonSerializer.DeserializeAsync<List<Movie>>(openStream);
        
        if (loadedMovies != null)
        {
            _movies = loadedMovies;
        }
    }
}

// --- 3. Демонстрація роботи в Main ---

class Program
{
    // Робимо Main асинхронним для використання await
    static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string filename = "movies_data.json";

        Console.WriteLine("=== Етап 1: Створення даних та збереження ===");
        var originalRepo = new MovieRepository();

        // Створюємо фільми з акторами
        originalRepo.Add(new Movie
        {
            Id = 1,
            Title = "Матриця",
            Year = 1999,
            Actors = new List<Actor>
            {
                new Actor { Id = 1, FullName = "Кіану Рівз" },
                new Actor { Id = 2, FullName = "Керрі-Енн Мосс" }
            }
        });

        originalRepo.Add(new Movie
        {
            Id = 2,
            Title = "Інтерстеллар",
            Year = 2014,
            Actors = new List<Actor>
            {
                new Actor { Id = 3, FullName = "Меттью Макконахі" },
                new Actor { Id = 4, FullName = "Енн Гетевей" }
            }
        });

        // Асинхронно зберігаємо дані у файл
        await originalRepo.SaveToFileAsync(filename);
        Console.WriteLine($"Дані успішно збережено у файл '{filename}'.\n");

        
        Console.WriteLine("=== Етап 2: Завантаження даних з файлу ===");
        
        // Створюємо НОВИЙ репозиторій, щоб довести, що дані читаються саме з файлу
        var newRepo = new MovieRepository();
        await newRepo.LoadFromFileAsync(filename);

        // Виводимо завантажені дані
        var loadedMovies = newRepo.GetAll();
        foreach (var movie in loadedMovies)
        {
            Console.WriteLine($"Фільм: {movie.Title} ({movie.Year})");
            Console.WriteLine("  Актори:");
            foreach (var actor in movie.Actors)
            {
                Console.WriteLine($"   - {actor.FullName}");
            }
            Console.WriteLine();
        }

        // Перевірка пошуку за ID
        Console.WriteLine("=== Етап 3: Пошук за ID ===");
        var foundMovie = newRepo.GetById(1);
        if (foundMovie != null)
        {
            Console.WriteLine($"Знайдено фільм з ID=1: {foundMovie.Title}");
        }
    }
}