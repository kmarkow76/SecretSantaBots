# Документация проекта SecretSantaBot

## Описание проекта
**SecretSantaBot** — это Telegram-бот для автоматизации игры "Тайный Санта". Он позволяет администратору организовать игру, регистрировать участников, автоматически распределять пары и отправлять уведомления.

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
│   ├── GameCrudOperations.cs  // Операции CRUD для игры
│   └── ParticipantCrudOperations.cs // Операции CRUD для участников
├── Database                   // Миграции и контекст
│   ├── CRUD
│   │   ├── ParticipantRepository.cs // Управление участниками
│   │   └── GameRepository.cs      // Управление состоянием игры
│   ├── ApplicationDbContext.cs // Контекст Entity Framework Core
│   ├── Models
│   │   ├── Game.cs             // Модель игры
│   │   └── Participant.cs      // Модель участника
│   └── Migrations             // Миграции для SQLite
├── Config                     // Конфигурация приложения
│   ├── AppSettings.json       // Настройки приложения
│   └── BotConfig.cs           // Чтение конфигурации (ключи, настройки)
└── Program.cs                 // Точка входа в приложение
