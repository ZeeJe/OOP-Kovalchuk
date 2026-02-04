# Домашня робота №2. LSP: приклади порушень
Принцип підстановки Лісков (LSP) стверджує, що об'єкти базового класу повинні бути замінені об'єктами похідних класів без порушення коректності роботи програми. Нижче наведено приклади, де цей принцип порушується.

# 1. Приклад: Птахи (Ostrich & Bird)
## Опис реалізації (Порушення LSP)
Базовий клас Bird має метод Fly(). Клас Ostrich (Страус) успадковує Bird, але оскільки страуси не літають, розробник часто викидає виключення.

public class Bird {
    public virtual void Fly() => Console.WriteLine("Птах летить");
}

public class Ostrich : Bird {
    public override void Fly() => throw new NotSupportedException("Страуси не літають!");
}
Чому це порушує LSP: Клієнтський код очікує, що будь-який птах може летіти. Виклик Fly() для страуса призведе до аварійного завершення програми.

## Рішення (Дотримання LSP)
Потрібно розділити птахів на тих, що літають, і тих, що ні, за допомогою інтерфейсів або зміни ієрархії.

public class Bird { /* Спільні властивості */ }

public interface IFlyable { void Fly(); }

public class Sparrow : Bird, IFlyable {
    public void Fly() => Console.WriteLine("Горобець летить");
}

public class Ostrich : Bird { /* Страус просто не реалізує IFlyable */ }
# 2. Приклад: Електронні пристрої (Smartphone & WiredTelephone)
## Опис реалізації (Порушення LSP)
Базовий клас Phone має метод SendSMS(). Стаціонарний телефон (WiredTelephone) успадковує Phone, але не підтримує повідомлення.

public class Phone {
    public virtual void Call() => Console.WriteLine("Виклик...");
    public virtual void SendSMS() => Console.WriteLine("SMS відправлено");
}

public class WiredTelephone : Phone {
    public override void SendSMS() => /* Нічого не робить або кидає помилку */ ;
}
Чому це порушує LSP: Клієнт, який працює з типом Phone, вважає, що функція SMS доступна. Це викликає помилкову поведінку, якщо стаціонарний телефон "мовчки" ігнорує запит на відправку.

## Рішення (Дотримання LSP)
Виносимо функцію повідомлень в окремий інтерфейс.

public interface ICallable { void Call(); }
public interface ISmsCapable { void SendSMS(); }

public class SmartPhone : ICallable, ISmsCapable {
    public void Call() { /*...*/ }
    public void SendSMS() { /*...*/ }
}

public class WiredTelephone : ICallable {
    public void Call() { /*...*/ }
}
# 3. Приклад: Робота з БД (ReadOnlyRepository)
## Опис реалізації (Порушення LSP)
Базовий репозиторій має методи Get() та Save(). Ми створюємо ReadOnlyRepository, де метод Save() блокується.

public class Repository {
    public virtual void Get() => Console.WriteLine("Дані отримано");
    public virtual void Save() => Console.WriteLine("Дані збережено");
}

public class ReadOnlyRepository : Repository {
    public override void Save() => throw new UnauthorizedAccessException("Тільки для читання!");
}
Чому це порушує LSP: Код, який займається обробкою даних, очікує, що він може зберегти результат. Передача "обрізаного" репозиторію ламає бізнес-логіку.

## Рішення (Дотримання LSP)
Використання композиції або розділення інтерфейсів для читання та запису.

public interface IReader { void Get(); }
public interface IWriter { void Save(); }

public class FullRepository : IReader, IWriter {
    public void Get() { /*...*/ }
    public void Save() { /*...*/ }
}

public class ReadOnlyRepository : IReader {
    public void Get() { /*...*/ }
}
# Висновок
Порушення LSP зазвичай виникає, коли ми використовуємо наслідування лише заради повторного використання коду, забуваючи про поведінкову сумісність. Правильний підхід — це використання інтерфейсів та композиції, що дозволяє клієнтському коду бути впевненим у результаті виклику методів.