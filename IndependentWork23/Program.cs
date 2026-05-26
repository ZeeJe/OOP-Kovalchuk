using System;

namespace IndependentWork23
{
    // =========================================================================
    // 1. ПАТЕРН ADAPTER (Адаптер)
    // =========================================================================

    // Target: Інтерфейс, який очікує сучасний клієнт
    public interface IUserAuthenticator
    {
        bool Authenticate(string username, string password);
    }

    // Adaptee: Існуючий застарілий клас із несумісним інтерфейсом
    public class LdapAuthenticator
    {
        public bool AuthenticateUser(string user, string pass)
        {
            if (user == "admin" && pass == "secret123")
            {
                return true;
            }
            return user == "manager" && pass == "pass456";
        }
    }

    // Adapter: Реалізує Target та адаптує Adaptee
    public class LdapAuthAdapter : IUserAuthenticator
    {
        private readonly LdapAuthenticator _ldapAuthenticator;

        public LdapAuthAdapter(LdapAuthenticator ldapAuthenticator)
        {
            _ldapAuthenticator = ldapAuthenticator;
        }

        public bool Authenticate(string username, string password)
        {
            Console.WriteLine("Адаптація запиту: перенаправлення до застарілої системи LDAP...");
            return _ldapAuthenticator.AuthenticateUser(username, password);
        }
    }

    // =========================================================================
    // 2. ПАТЕРН FACADE (Фасад)
    // =========================================================================

    public class UserService
    {
        public void CreateUser(string username)
        {
            Console.WriteLine($"UserService: Обліковий запис для '{username}' успішно створено.");
        }
    }

    public class RoleService
    {
        public void AssignRole(string username, string role)
        {
            Console.WriteLine($"RoleService: Користувачу '{username}' надано роль {role}.");
        }
    }

    public class PermissionService
    {
        public void GrantPermission(string username, string permission)
        {
            Console.WriteLine($"PermissionService: Для '{username}' активовано дозвіл {permission}.");
        }
    }

    // Facade: Надає спрощений інтерфейс до підсистеми
    public class SecurityFacade
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly PermissionService _permissionService;

        public SecurityFacade()
        {
            _userService = new UserService();
            _roleService = new RoleService();
            _permissionService = new PermissionService();
        }

        public void SetupNewUser(string username, string role, string permission)
        {
            Console.WriteLine($"Початок комплексного налаштування профілю для '{username}':");
            _userService.CreateUser(username);
            _roleService.AssignRole(username, role);
            _permissionService.GrantPermission(username, permission);
            Console.WriteLine("Налаштування нового користувача успішно завершено.\n");
        }
    }

    // =========================================================================
    // 3. ПАТЕРН PROXY (Проксі)
    // =========================================================================

    // Subject: Спільний інтерфейс
    public interface IUserAccess
    {
        string GetUser(string username);
    }

    // RealSubject: Об'єкт, що виконує реальну роботу з БД
    public class RealUserAccess : IUserAccess
    {
        public string GetUser(string username)
        {
            Console.WriteLine($"Виконується прямий SQL-запит до БД для пошуку '{username}'...");
            return $"Дані користувача {username}: Регіон - Україна, Статус - Активний.";
        }
    }

    // Proxy: Реалізує Subject та перевіряє дозволи користувачів
    public class SecurityUserAccessProxy : IUserAccess
    {
        private readonly RealUserAccess _realUserAccess;
        private readonly string _currentUserRole;

        public SecurityUserAccessProxy(RealUserAccess realUserAccess, string currentUserRole)
        {
            _realUserAccess = realUserAccess;
            _currentUserRole = currentUserRole;
        }

        public string GetUser(string username)
        {
            Console.WriteLine($"Перевірка прав доступу. Поточна роль запитувача: {_currentUserRole}");

            if (string.Equals(_currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Доступ дозволено. Перенаправлення до реальної бази даних.");
                return _realUserAccess.GetUser(username);
            }

            Console.WriteLine("ВІДМОВЛЕНО В ДОСТУПІ: Недостатньо прав для перегляду БД.");
            return $"Помилка: Користувач із роллю '{_currentUserRole}' не має права читати дані {username}.";
        }
    }

    // =========================================================================
    // ТОЧКА ВХОДУ (Метод Main)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №23 (Варіант 4) ===");
            Console.WriteLine("Комбінація структурних патернів: Adapter, Facade, Proxy\n");

            // 1. Демонстрація Adapter
            Console.WriteLine("--- 1. ДЕМОНСТРАЦІЯ ПАТЕРНУ ADAPTER ---");
            LdapAuthenticator oldLdapSystem = new LdapAuthenticator();
            IUserAuthenticator authSystem = new LdapAuthAdapter(oldLdapSystem);

            bool adminAuth = authSystem.Authenticate("admin", "secret123");
            Console.WriteLine($"Результат входу (admin): {adminAuth}");

            bool guestAuth = authSystem.Authenticate("guest", "wrong_pass");
            Console.WriteLine($"Результат входу (guest): {guestAuth}\n");

            // 2. Демонстрація Facade
            Console.WriteLine("--- 2. ДЕМОНСТРАЦІЯ ПАТЕРНУ FACADE ---");
            SecurityFacade securityFacade = new SecurityFacade();
            securityFacade.SetupNewUser("Ivan_99", "Manager", "Read_Reports");

            // 3. Демонстрація Proxy
            Console.WriteLine("--- 3. ДЕМОНСТРАЦІЯ ПАТЕРНУ PROXY ---");
            RealUserAccess databaseService = new RealUserAccess();

            // Сценарій А: Роль без доступу (Негативний)
            IUserAccess proxyForManager = new SecurityUserAccessProxy(databaseService, "Manager");
            string managerResult = proxyForManager.GetUser("Ivan_99");
            Console.WriteLine($"Відповідь системи: {managerResult}\n");

            // Сценарій Б: Роль із доступом (Позитивний)
            IUserAccess proxyForAdmin = new SecurityUserAccessProxy(databaseService, "Admin");
            string adminResult = proxyForAdmin.GetUser("Ivan_99");
            Console.WriteLine($"Відповідь системи: {adminResult}\n");

            Console.ReadLine();
        }
    }
}