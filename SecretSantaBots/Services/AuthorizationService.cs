namespace SecretSantaBots.Services
{
    /// <summary>
    /// Сервис для авторизации пользователей в игре, проверяющий наличие прав администратора.
    /// </summary>
    public class AuthorizationService
    {
        /// <summary>
        /// Множество идентификаторов пользователей, которые являются администраторами.
        /// </summary>
        private readonly List<long> _admins;

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
        /// <summary>
        /// Проверяет, является ли пользователь администратором.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя для проверки.</param>
        /// <returns>Задача, возвращающая <c>true</c>, если пользователь является администратором, и <c>false</c> в противном случае.</returns>
        public virtual Task<bool> IsAdmin(long userId)
        {
            return Task.FromResult(_admins.Contains(userId));
        }
    }
}