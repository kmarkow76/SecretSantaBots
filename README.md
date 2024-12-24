# Документация проекта SecretSantaBot

## Описание проекта
**SecretSantaBot** — это Telegram-бот для автоматизации игры "Тайный Санта". Бот позволяет администраторам организовывать игру, регистрировать участников, автоматически распределять пары и отправлять уведомления.

---

## Структура проекта
```plaintext
SecretSantaBot
├── Config                     // Конфигурация приложения
│   ├── appsettings.json       // Настройки приложения
├── Controllers                // Контроллеры для обработки команд
│   ├── AdminController.cs     // Обработка команд для администраторов
│   ├── UserController.cs      // Обработка команд для участников
│   └── CommandHandler.cs      // Общая обработка входящих сообщений
├── Services                   // Логика приложения
│   ├── GameService.cs         // Логика игры (регистрация, распределение)
│   ├── AdminValidationService.cs // Проверка прав администратора
│   ├── NotificationService.cs // Уведомления пользователям
│   ├── PairingService.cs      // Алгоритм распределения участников
│   └── ValidationService.cs   // Общая валидация моделей
├── Validators                 // Логика валидации моделей
│   ├── GameValidation.cs      // Валидация
│   └── ParticipantValidation.cs // Валидация
├── Database                   // Миграции и контекст
│   ├── CRUD
│   │   ├── ParticipantCrudOperations.cs // Управление участниками
│   │   └── GameCrudOperations.cs      // Управление состоянием игры
│   ├── ApplicationDbContext.cs // Контекст Entity Framework Core
│   ├── Models
│   │   ├── Game.cs             // Модель игры
│   │   └── Participant.cs      // Модель участника
│   └── Migrations             // Миграции
├── Config                     // Конфигурация приложения
│   ├── AppSettings.json       // Настройки приложения
│   └── BotConfig.cs           // Чтение конфигурации (ключи, настройки)
└── Program.cs                 // Точка входа в приложение
```

---

## Доступные команды

### Для администраторов
- **`/start <валюта> <сумма>`**  
  🔹 Начать регистрацию.  
  _Описание:_ Запускает прием заявок на участие. Администратор указывает валюту и сумму подарка.  
  ✉️ _Ответ:_ Бот уведомляет участников о начале регистрации с указанными условиями.

- **`/stop`**  
  🔹 Завершить регистрацию.  
  _Описание:_ Останавливает прием заявок, распределяет участников по парам и отправляет каждому участнику личное сообщение с информацией о том, кому он дарит подарок.  
  ✉️ _Ответ:_ Уведомление в общем чате о завершении регистрации и отправка личных сообщений.

- **`/reset`**  
  🔹 Сбросить игру.  
  _Описание:_ Полностью очищает данные текущей игры, позволяя начать новую.  
  ✉️ _Ответ:_ Уведомление об успешном сбросе и предложение начать новую регистрацию.

### Для участников
- **`/join`**  
  🔹 Подать заявку на участие.  
  _Описание:_ Позволяет участнику зарегистрироваться в текущей игре.  
  ✉️ _Ответ:_ Подтверждение регистрации и информация о сумме подарка.
---

## Используемые зависимости
Проект использует следующие библиотеки:  
- **FluentValidation** (версия 11.11.0) — для валидации моделей.  
- **Microsoft.EntityFrameworkCore** (версия 8.0.0) — для работы с базой данных.  
- **Microsoft.EntityFrameworkCore.Design** (версия 8.0.0) — для генерации миграций.  
- **Npgsql** (версия 8.0.0) — драйвер для PostgreSQL.  
- **Npgsql.EntityFrameworkCore.PostgreSQL** (версия 8.0.0) — интеграция EF Core с PostgreSQL.  
- **Telegram.Bot** (версия 22.2.0) — для взаимодействия с Telegram API.  

---

## Установка зависимостей
Для установки зависимостей используйте команду `dotnet add package`. Примеры:  

- Установка **FluentValidation**:  
   ```bash
   dotnet add package FluentValidation --version 11.11.0
   ```  

- Установка **Microsoft.EntityFrameworkCore**:  
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
   ```  

- Установка **Microsoft.EntityFrameworkCore.Design**:  
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
   ```  

- Установка **Npgsql**:  
   ```bash
   dotnet add package Npgsql --version 8.0.0
   ```  

- Установка **Npgsql.EntityFrameworkCore.PostgreSQL**:  
   ```bash
   dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
   ```  

- Установка **Telegram.Bot**:  
   ```bash
   dotnet add package Telegram.Bot --version 22.2.0
   ```  

---

## Запуск приложения
1. Настройте файл `appsettings.json` с необходимыми параметрами (например, токен бота, параметры подключения к базе данных).  

2. Выполните миграции для базы данных:  
   ```bash
   dotnet ef database update
   ```  

3. Запустите приложение:  
   ```bash
   dotnet run
   ```  

---

## Определение администраторов
Администраторы определяются в классе **AuthorizationService**. Пример инициализации:

```csharp
/// <summary>
/// Конструктор, инициализирующий список администраторов.
/// </summary>
/// <param name="admins">Коллекция идентификаторов пользователей, которые являются администраторами.</param>
public AuthorizationService()
{
    _admins = new List<long>
    {
        904281253
    };
}
```

---

## Параметры для игры
Параметры игры можно указать при обработке команды `/start`. Пример:

```csharp
if (message.Text.StartsWith("/start"))
{
    await adminController.StartGame(chatId, userId, "РУБ", 200);
}
```

---

## Контактная информация
Для вопросов и предложений обратитесь к разработчику.

