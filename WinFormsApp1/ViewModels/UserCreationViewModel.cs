using ADManager.Models; // Добавляем
using ADManager.Services;
using System.Text.RegularExpressions;

namespace ADManager.ViewModels
{
    public class UserCreationViewModel
    {
        private readonly ActiveDirectoryService _adService;
        public List<string> OrganizationalUnits { get; private set; } = new();

        public UserCreationViewModel(ActiveDirectoryService adService)
        {
            _adService = adService;
            LoadOrganizationalUnits();
        }

        private void LoadOrganizationalUnits()
        {
            try
            {
                OrganizationalUnits = _adService.GetOrganizationalUnits();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки подразделений: {ex.Message}", "Ошибка");
            }
        }

        public (bool Success, string? Error) CreateUser(User user, string password, string confirmPassword, string ouDn)
        {
            if (string.IsNullOrEmpty(user.SAMAccountName) || string.IsNullOrEmpty(user.GivenName) || string.IsNullOrEmpty(user.Surname))
                return (false, "Заполните обязательные поля (Логин, Имя, Фамилия)");

            if (string.IsNullOrEmpty(password) || password != confirmPassword)
                return (false, "Пароли не совпадают или не введены");

            if (password.Length < 8)
                return (false, "Пароль должен быть не менее 8 символов");

            if (!Regex.IsMatch(user.SAMAccountName, @"^[a-zA-Z0-9\-_]+$"))
                return (false, "Логин содержит недопустимые символы");

            if (string.IsNullOrEmpty(ouDn))
                return (false, "Выберите подразделение (OU)");

            try
            {
                user.DisplayName = string.IsNullOrEmpty(user.DisplayName) ? $"{user.GivenName} {user.Surname}" : user.DisplayName;
                _adService.CreateUser(user, password, ouDn);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка создания пользователя: {ex.Message}");
            }
        }
    }
}