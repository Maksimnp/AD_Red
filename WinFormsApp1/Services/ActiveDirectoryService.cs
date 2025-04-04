using ADManager.Models;
using System.DirectoryServices;
using System.Text;

namespace ADManager.Services
{
    public class ActiveDirectoryService
    {
        private readonly string _ldapPath;
        private readonly string _username;
        private readonly string _password;
        private readonly string _logFilePath = "ad_manager.log";

        // Публичные свойства для доступа к _username и _password
        public string Username => _username;
        public string Password => _password;

        public ActiveDirectoryService(string domainController, string baseDn, string username, string password)
        {
            _ldapPath = $"LDAP://{domainController}/{baseDn}";
            _username = username;
            _password = password;
            Log($"Инициализация сервиса AD: {_ldapPath}");
        }

        private void Log(string message)
        {
            File.AppendAllText(_logFilePath, $"{DateTime.Now} - {message}\n", Encoding.UTF8);
        }

        public bool TestConnection()
        {
            try
            {
                using var entry = new DirectoryEntry(_ldapPath, _username, _password);
                entry.RefreshCache();
                Log("Подключение к AD успешно");
                return true;
            }
            catch (Exception ex)
            {
                Log($"ERROR - Ошибка подключения к AD: {ex.Message}");
                return false;
            }
        }

        public User? FindUser(string sAMAccountName)
        {
            try
            {
                using var entry = new DirectoryEntry(_ldapPath, _username, _password);
                using var searcher = new DirectorySearcher(entry)
                {
                    Filter = $"(&(objectClass=user)(sAMAccountName={sAMAccountName}))",
                    SearchScope = SearchScope.Subtree
                };

                searcher.PropertiesToLoad.AddRange(new[]
                {
                    "sAMAccountName", "givenName", "sn", "displayName", "description",
                    "physicalDeliveryOfficeName", "telephoneNumber", "mail", "wWWHomePage",
                    "distinguishedName", "initials", "title"
                });

                var result = searcher.FindOne();
                if (result == null)
                {
                    Log($"WARNING - Пользователь {sAMAccountName} не найден");
                    return null;
                }

                return new User
                {
                    SAMAccountName = result.Properties["sAMAccountName"]?.Count > 0 ? result.Properties["sAMAccountName"][0]?.ToString() : null,
                    GivenName = result.Properties["givenName"]?.Count > 0 ? result.Properties["givenName"][0]?.ToString() : null,
                    Surname = result.Properties["sn"]?.Count > 0 ? result.Properties["sn"][0]?.ToString() : null,
                    DisplayName = result.Properties["displayName"]?.Count > 0 ? result.Properties["displayName"][0]?.ToString() : null,
                    Description = result.Properties["description"]?.Count > 0 ? result.Properties["description"][0]?.ToString() : null,
                    Office = result.Properties["physicalDeliveryOfficeName"]?.Count > 0 ? result.Properties["physicalDeliveryOfficeName"][0]?.ToString() : null,
                    TelephoneNumber = result.Properties["telephoneNumber"]?.Count > 0 ? result.Properties["telephoneNumber"][0]?.ToString() : null,
                    Email = result.Properties["mail"]?.Count > 0 ? result.Properties["mail"][0]?.ToString() : null,
                    WebPage = result.Properties["wWWHomePage"]?.Count > 0 ? result.Properties["wWWHomePage"][0]?.ToString() : null,
                    DistinguishedName = result.Properties["distinguishedName"]?.Count > 0 ? result.Properties["distinguishedName"][0]?.ToString() : null,
                    Initials = result.Properties["initials"]?.Count > 0 ? result.Properties["initials"][0]?.ToString() : null,
                    Title = result.Properties["title"]?.Count > 0 ? result.Properties["title"][0]?.ToString() : null
                };
            }
            catch (Exception ex)
            {
                Log($"ERROR - Ошибка поиска пользователя {sAMAccountName}: {ex.Message}");
                throw;
            }
        }

        public void UpdateUser(User user)
        {
            if (string.IsNullOrEmpty(user.DistinguishedName))
                throw new ArgumentException("DistinguishedName пользователя не указан");

            try
            {
                using var entry = new DirectoryEntry($"LDAP://{user.DistinguishedName}", _username, _password);
                entry.RefreshCache();

                if (!string.IsNullOrEmpty(user.GivenName)) entry.Properties["givenName"].Value = user.GivenName;
                else entry.Properties["givenName"].Clear();

                if (!string.IsNullOrEmpty(user.Surname)) entry.Properties["sn"].Value = user.Surname;
                else entry.Properties["sn"].Clear();

                if (!string.IsNullOrEmpty(user.DisplayName)) entry.Properties["displayName"].Value = user.DisplayName;
                else entry.Properties["displayName"].Clear();

                if (!string.IsNullOrEmpty(user.Title)) entry.Properties["title"].Value = user.Title;
                else entry.Properties["title"].Clear();

                if (!string.IsNullOrEmpty(user.Description)) entry.Properties["description"].Value = user.Description;
                else entry.Properties["description"].Clear();

                if (!string.IsNullOrEmpty(user.Office)) entry.Properties["physicalDeliveryOfficeName"].Value = user.Office;
                else entry.Properties["physicalDeliveryOfficeName"].Clear();

                if (!string.IsNullOrEmpty(user.TelephoneNumber)) entry.Properties["telephoneNumber"].Value = user.TelephoneNumber;
                else entry.Properties["telephoneNumber"].Clear();

                if (!string.IsNullOrEmpty(user.Email)) entry.Properties["mail"].Value = user.Email;
                else entry.Properties["mail"].Clear();

                if (!string.IsNullOrEmpty(user.WebPage)) entry.Properties["wWWHomePage"].Value = user.WebPage;
                else entry.Properties["wWWHomePage"].Clear();

                entry.CommitChanges();
                Log($"Пользователь {user.SAMAccountName} успешно обновлен");
            }
            catch (Exception ex)
            {
                Log($"ERROR - Ошибка обновления пользователя {user.SAMAccountName}: {ex.Message}");
                throw;
            }
        }

        public void CreateUser(User user, string password, string ouDn)
        {
            try
            {
                using var ou = new DirectoryEntry($"LDAP://{ouDn}", _username, _password);
                ou.RefreshCache();

                // Проверяем, существует ли пользователь с таким CN
                using var searcher = new DirectorySearcher(ou)
                {
                    Filter = $"(&(objectClass=user)(CN={user.DisplayName}))",
                    SearchScope = SearchScope.OneLevel
                };

                if (searcher.FindOne() != null)
                    throw new InvalidOperationException($"Пользователь с именем {user.DisplayName} уже существует в {ouDn}");

                // Проверяем, существует ли пользователь с таким sAMAccountName
                using var domainSearcher = new DirectorySearcher(new DirectoryEntry(_ldapPath, _username, _password))
                {
                    Filter = $"(&(objectClass=user)(sAMAccountName={user.SAMAccountName}))",
                    SearchScope = SearchScope.Subtree
                };

                if (domainSearcher.FindOne() != null)
                    throw new InvalidOperationException($"Пользователь с логином {user.SAMAccountName} уже существует в домене");

                // Создаем пользователя
                using var newUser = ou.Children.Add($"CN={user.DisplayName}", "user");
                newUser.Properties["sAMAccountName"].Value = user.SAMAccountName;
                newUser.Properties["userPrincipalName"].Value = $"{user.SAMAccountName}@{_ldapPath.Split('/')[2].Replace("DC=", "").Replace(",", ".")}";
                newUser.Properties["givenName"].Value = user.GivenName;
                newUser.Properties["sn"].Value = user.Surname;
                newUser.Properties["displayName"].Value = user.DisplayName;
                newUser.Properties["name"].Value = user.DisplayName;
                newUser.Properties["userAccountControl"].Value = 512; // Активный пользователь

                if (!string.IsNullOrEmpty(user.Description)) newUser.Properties["description"].Value = user.Description;
                if (!string.IsNullOrEmpty(user.Office)) newUser.Properties["physicalDeliveryOfficeName"].Value = user.Office;
                if (!string.IsNullOrEmpty(user.TelephoneNumber)) newUser.Properties["telephoneNumber"].Value = user.TelephoneNumber;
                if (!string.IsNullOrEmpty(user.Email)) newUser.Properties["mail"].Value = user.Email;
                if (!string.IsNullOrEmpty(user.WebPage)) newUser.Properties["wWWHomePage"].Value = user.WebPage;

                newUser.CommitChanges();
                newUser.Invoke("SetPassword", new object[] { password });
                newUser.CommitChanges();

                Log($"Пользователь {user.SAMAccountName} успешно создан в {ouDn}");
            }
            catch (Exception ex)
            {
                Log($"ERROR - Ошибка создания пользователя {user.SAMAccountName}: {ex.Message}");
                throw;
            }
        }

        public List<string> GetOrganizationalUnits()
        {
            try
            {
                using var entry = new DirectoryEntry(_ldapPath, _username, _password);
                using var searcher = new DirectorySearcher(entry)
                {
                    Filter = "(objectClass=organizationalUnit)",
                    SearchScope = SearchScope.Subtree
                };

                searcher.PropertiesToLoad.Add("distinguishedName");

                var results = searcher.FindAll();
                var ouList = new List<string>();

                foreach (SearchResult result in results)
                {
                    if (result.Properties["distinguishedName"]?.Count > 0)
                    {
                        var dn = result.Properties["distinguishedName"][0]?.ToString();
                        if (dn != null) // Проверка на null
                        {
                            ouList.Add(dn);
                        }
                    }
                }

                Log($"Найдено {ouList.Count} подразделений");
                return ouList;
            }
            catch (Exception ex)
            {
                Log($"ERROR - Ошибка получения списка OU: {ex.Message}");
                throw;
            }
        }
    }
}