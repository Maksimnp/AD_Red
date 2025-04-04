using ADManager.Models; // Добавляем
using ADManager.Services;
using System.Text.RegularExpressions;

namespace ADManager.ViewModels
{
    public class UserManagementViewModel
    {
        private readonly ActiveDirectoryService _adService;
        public User? CurrentUser { get; private set; }

        public UserManagementViewModel(ActiveDirectoryService adService)
        {
            _adService = adService;
        }

        public (bool Success, string? Error) FindUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return (false, "Введите имя пользователя");

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9\-_\.]+$"))
                return (false, "Имя пользователя содержит недопустимые символы");

            try
            {
                CurrentUser = _adService.FindUser(username);
                if (CurrentUser == null)
                    return (false, $"Пользователь {username} не найден");

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка поиска пользователя: {ex.Message}");
            }
        }

        public (bool Success, string? Error) SaveChanges(User updatedUser)
        {
            if (CurrentUser == null || string.IsNullOrEmpty(CurrentUser.DistinguishedName))
                return (false, "Сначала найдите пользователя");

            try
            {
                updatedUser.SAMAccountName = CurrentUser.SAMAccountName;
                updatedUser.DistinguishedName = CurrentUser.DistinguishedName;

                if (!string.IsNullOrEmpty(updatedUser.TelephoneNumber) && !Regex.IsMatch(updatedUser.TelephoneNumber, @"^[\d\s\-\+\(\)]+$"))
                    return (false, "Недопустимые символы в номере телефона");

                if (!string.IsNullOrEmpty(updatedUser.Email) && !Regex.IsMatch(updatedUser.Email, @"^[^@]+@[^@]+\.[^@]+$"))
                    return (false, "Некорректный email адрес");

                _adService.UpdateUser(updatedUser);
                CurrentUser = updatedUser;
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка обновления данных: {ex.Message}");
            }
        }
    }
}