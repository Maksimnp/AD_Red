using ADManager.Models;
using ADManager.Services;
using ADManager.ViewModels;
using System.Windows.Forms;
using System.Drawing;
using System.DirectoryServices;

namespace ADManager
{
    public partial class MainForm : Form
    {
        private ActiveDirectoryService? _adService;
        private UserManagementViewModel? _userManagementVM;
        private OUViewModel? _ouVM;

        public MainForm()
        {
            StartPosition = FormStartPosition.CenterScreen; // Окно открывается по центру экрана
            MinimumSize = new Size(600, 800); // Минимальный размер окна
            AutoSize = true; // Автоматическая подстройка размера
            BackColor = Color.FromArgb(245, 245, 245); // Светлый фон
            ShowLoginForm();
        }

        private void ShowLoginForm()
        {
            using var loginForm = new LoginForm();
            if (loginForm.ShowDialog() != DialogResult.OK)
            {
                Close();
                return;
            }

            _adService = new ActiveDirectoryService(loginForm.DomainController, loginForm.BaseDn, loginForm.Username, loginForm.Password);
            if (!_adService.TestConnection())
            {
                MessageBox.Show("Не удалось подключиться к Active Directory", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            _userManagementVM = new UserManagementViewModel(_adService);
            _ouVM = new OUViewModel(_adService);
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                Padding = new Point(10, 10)
            };

            // Вкладка "Управление пользователями"
            var userManagementTab = new TabPage("Управление пользователями")
            {
                BackColor = Color.White
            };
            userManagementTab.Controls.Add(CreateUserManagementPanel());
            tabControl.TabPages.Add(userManagementTab);

            // Вкладка "Подразделения (OU)"
            var ouTab = new TabPage("Подразделения (OU)")
            {
                BackColor = Color.White
            };
            ouTab.Controls.Add(CreateOUPanel());
            tabControl.TabPages.Add(ouTab);

            Controls.Add(tabControl);
        }

        private Panel CreateUserManagementPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoScroll = true, // Прокрутка
                BackColor = Color.White,
                Padding = new Padding(20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Создаем FlowLayoutPanel для центрирования содержимого
            var flowPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Anchor = AnchorStyles.None,
                Location = new Point((panel.ClientSize.Width - 500) / 2, 0) // Центрируем по горизонтали
            };

            // Поиск пользователя
            var searchLabel = new Label
            {
                Text = "Поиск пользователя",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243), // Синий акцентный цвет
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15)
            };
            flowPanel.Controls.Add(searchLabel);

            // Панель для поля "Имя"
            var usernamePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var usernameLabel = new Label
            {
                Text = "Имя",
                Width = 100,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var usernameTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            usernamePanel.Controls.Add(usernameLabel);
            usernamePanel.Controls.Add(usernameTextBox);
            flowPanel.Controls.Add(usernamePanel);

            // Панель для кнопки "Найти пользователя"
            var searchButtonPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 20)
            };
            var searchButton = new Button
            {
                Text = "Найти пользователя",
                Width = 150,
                Height = 35,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(33, 150, 243), // Синий фон кнопки
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(110, 0, 0, 0) // Центрируем кнопку
            };
            searchButton.FlatAppearance.BorderSize = 0;
            searchButtonPanel.Controls.Add(searchButton);
            flowPanel.Controls.Add(searchButtonPanel);

            var userDetailsLabel = new Label
            {
                Text = "Редактирование пользователя",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };
            flowPanel.Controls.Add(userDetailsLabel);

            // Поля пользователя
            var loginPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var loginLabel = new Label
            {
                Text = "Логин:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var loginTextBox = new TextBox
            {
                Width = 300,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            loginPanel.Controls.Add(loginLabel);
            loginPanel.Controls.Add(loginTextBox);
            flowPanel.Controls.Add(loginPanel);

            var surnamePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var surnameLabel = new Label
            {
                Text = "Фамилия:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var surnameTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            surnamePanel.Controls.Add(surnameLabel);
            surnamePanel.Controls.Add(surnameTextBox);
            flowPanel.Controls.Add(surnamePanel);

            var givenNamePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var givenNameLabel = new Label
            {
                Text = "Имя:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var givenNameTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            givenNamePanel.Controls.Add(givenNameLabel);
            givenNamePanel.Controls.Add(givenNameTextBox);
            flowPanel.Controls.Add(givenNamePanel);

            var initialsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var initialsLabel = new Label
            {
                Text = "Инициалы:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var initialsTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            initialsPanel.Controls.Add(initialsLabel);
            initialsPanel.Controls.Add(initialsTextBox);
            flowPanel.Controls.Add(initialsPanel);

            var titlePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var titleLabel = new Label
            {
                Text = "Должность:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var titleTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            titlePanel.Controls.Add(titleLabel);
            titlePanel.Controls.Add(titleTextBox);
            flowPanel.Controls.Add(titlePanel);

            var displayNamePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var displayNameLabel = new Label
            {
                Text = "Отображаемое имя:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var displayNameTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            displayNamePanel.Controls.Add(displayNameLabel);
            displayNamePanel.Controls.Add(displayNameTextBox);
            flowPanel.Controls.Add(displayNamePanel);

            var descriptionPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var descriptionLabel = new Label
            {
                Text = "Описание:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var descriptionTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            descriptionPanel.Controls.Add(descriptionLabel);
            descriptionPanel.Controls.Add(descriptionTextBox);
            flowPanel.Controls.Add(descriptionPanel);

            var officePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var officeLabel = new Label
            {
                Text = "Комната:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var officeTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            officePanel.Controls.Add(officeLabel);
            officePanel.Controls.Add(officeTextBox);
            flowPanel.Controls.Add(officePanel);

            var telephonePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var telephoneLabel = new Label
            {
                Text = "Номер телефона:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var telephoneTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            telephonePanel.Controls.Add(telephoneLabel);
            telephonePanel.Controls.Add(telephoneTextBox);
            flowPanel.Controls.Add(telephonePanel);

            var emailPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var emailLabel = new Label
            {
                Text = "Email:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var emailTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            emailPanel.Controls.Add(emailLabel);
            emailPanel.Controls.Add(emailTextBox);
            flowPanel.Controls.Add(emailPanel);

            var webPagePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 20)
            };
            var webPageLabel = new Label
            {
                Text = "Веб-страница:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            var webPageTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            webPagePanel.Controls.Add(webPageLabel);
            webPagePanel.Controls.Add(webPageTextBox);
            flowPanel.Controls.Add(webPagePanel);

            // Панель для кнопки "Сохранить изменения"
            var buttonsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 20)
            };
            var saveButton = new Button
            {
                Text = "Сохранить изменения",
                Width = 150,
                Height = 35,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(110, 0, 0, 0)
            };
            saveButton.FlatAppearance.BorderSize = 0;
            buttonsPanel.Controls.Add(saveButton);
            flowPanel.Controls.Add(buttonsPanel);

            panel.Controls.Add(flowPanel);

            // Обработчики событий
            searchButton.Click += (s, e) =>
            {
                var (success, error) = _userManagementVM!.FindUser(usernameTextBox.Text);
                if (!success)
                {
                    MessageBox.Show(error, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var user = _userManagementVM.CurrentUser!;
                loginTextBox.Text = user.SAMAccountName;
                surnameTextBox.Text = user.Surname;
                givenNameTextBox.Text = user.GivenName;
                initialsTextBox.Text = user.Initials;
                titleTextBox.Text = user.Title;
                descriptionTextBox.Text = user.Description;
                officeTextBox.Text = user.Office;
                telephoneTextBox.Text = user.TelephoneNumber;
                emailTextBox.Text = user.Email;
                webPageTextBox.Text = user.WebPage;

                // Автоматическое обновление DisplayName на основе SAMAccountName, если оно пустое
                if (string.IsNullOrEmpty(user.DisplayName))
                {
                    user.DisplayName = user.SAMAccountName; // Или можно использовать формат, например: $"{user.GivenName} {user.Surname}"
                    displayNameTextBox.Text = user.DisplayName;

                    // Сохраняем обновленное DisplayName в Active Directory
                    var updatedUser = new User
                    {
                        DistinguishedName = user.DistinguishedName,
                        DisplayName = user.DisplayName
                    };
                    var (updateSuccess, updateError) = _userManagementVM.SaveChanges(updatedUser);
                    if (!updateSuccess)
                    {
                        MessageBox.Show(updateError, "Ошибка при обновлении DisplayName", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    displayNameTextBox.Text = user.DisplayName;
                }
            };

            saveButton.Click += (s, e) =>
            {
                var updatedUser = new User
                {
                    Surname = surnameTextBox.Text,
                    GivenName = givenNameTextBox.Text,
                    Initials = initialsTextBox.Text,
                    DisplayName = displayNameTextBox.Text,
                    Title = titleTextBox.Text,
                    Description = descriptionTextBox.Text,
                    Office = officeTextBox.Text,
                    TelephoneNumber = telephoneTextBox.Text,
                    Email = emailTextBox.Text,
                    WebPage = webPageTextBox.Text
                };

                var (success, error) = _userManagementVM!.SaveChanges(updatedUser);
                if (!success)
                {
                    MessageBox.Show(error, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // Обновляем положение FlowLayoutPanel при изменении размера панели
            panel.Resize += (s, e) =>
            {
                flowPanel.Location = new Point((panel.ClientSize.Width - flowPanel.Width) / 2, 0);
            };

            return panel;
        }

        private Panel CreateOUPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoScroll = true, // Прокрутка
                BackColor = Color.White,
                Padding = new Padding(20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Создаем FlowLayoutPanel для центрирования содержимого
            var flowPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Anchor = AnchorStyles.None,
                Location = new Point((panel.ClientSize.Width - 420) / 2, 0) // Центрируем по горизонтали
            };

            // Заголовок "Список подразделений"
            var ouLabel = new Label
            {
                Text = "Список подразделений:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };
            flowPanel.Controls.Add(ouLabel);

            // Список подразделений
            var ouListBox = new ListBox
            {
                Width = 400,
                Height = 150, // Уменьшаем высоту, чтобы уместить список пользователей
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            ouListBox.Items.AddRange(_ouVM!.OrganizationalUnits.ToArray());
            flowPanel.Controls.Add(ouListBox);

            // Заголовок "Список пользователей"
            var usersLabel = new Label
            {
                Text = "Список пользователей:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                AutoSize = true,
                Margin = new Padding(0, 20, 0, 10)
            };
            flowPanel.Controls.Add(usersLabel);

            // Список пользователей
            var usersListBox = new ListBox
            {
                Width = 400,
                Height = 150,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            flowPanel.Controls.Add(usersListBox);

            // Панель для отображения информации о пользователе
            var userInfoPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Margin = new Padding(0, 20, 0, 0)
            };

            // Добавляем метки для отображения информации о пользователе
            var loginLabel = new Label
            {
                Text = "Логин: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(loginLabel);

            var surnameLabel = new Label
            {
                Text = "Фамилия: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(surnameLabel);

            var givenNameLabel = new Label
            {
                Text = "Имя: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(givenNameLabel);

            var initialsLabel = new Label
            {
                Text = "Инициалы: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(initialsLabel);

            var titleLabel = new Label
            {
                Text = "Должность: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(titleLabel);

            var displayNameLabel = new Label
            {
                Text = "Отображаемое имя: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(displayNameLabel);

            var descriptionLabel = new Label
            {
                Text = "Описание: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(descriptionLabel);

            var officeLabel = new Label
            {
                Text = "Комната: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(officeLabel);

            var telephoneLabel = new Label
            {
                Text = "Номер телефона: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(telephoneLabel);

            var emailLabel = new Label
            {
                Text = "Email: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(emailLabel);

            var webPageLabel = new Label
            {
                Text = "Веб-страница: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                AutoSize = true
            };
            userInfoPanel.Controls.Add(webPageLabel);

            flowPanel.Controls.Add(userInfoPanel);
            panel.Controls.Add(flowPanel);

            // Обработчик события выбора подразделения
            ouListBox.SelectedIndexChanged += (s, e) =>
            {
                usersListBox.Items.Clear(); // Очищаем список пользователей
                if (ouListBox.SelectedItem == null)
                    return;

                string selectedOU = ouListBox.SelectedItem.ToString();
                try
                {
                    using var entry = new DirectoryEntry($"LDAP://{selectedOU}", _adService.Username, _adService.Password);
                    using var searcher = new DirectorySearcher(entry)
                    {
                        Filter = "(objectClass=user)",
                        SearchScope = SearchScope.Subtree
                    };

                    searcher.PropertiesToLoad.Add("sAMAccountName");
                    searcher.PropertiesToLoad.Add("displayName");

                    var results = searcher.FindAll();
                    foreach (SearchResult result in results)
                    {
                        string samAccountName = result.Properties["sAMAccountName"]?.Count > 0 ? result.Properties["sAMAccountName"][0]?.ToString() : "Не указано";
                        string displayName = result.Properties["displayName"]?.Count > 0 ? result.Properties["displayName"][0]?.ToString() : "Не указано";
                        usersListBox.Items.Add($"{displayName} ({samAccountName})");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении списка пользователей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Обработчик события выбора пользователя
            usersListBox.SelectedIndexChanged += async (s, e) =>
            {
                if (usersListBox.SelectedItem == null)
                    return;

                string selectedUser = usersListBox.SelectedItem.ToString();
                string sAMAccountName = selectedUser.Split('(')[1].TrimEnd(')').Trim();

                try
                {
                    var user = _adService.FindUser(sAMAccountName);
                    if (user == null)
                    {
                        MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Обновляем метки с информацией о пользователе
                    loginLabel.Text = $"Логин: {user.SAMAccountName}";
                    surnameLabel.Text = $"Фамилия: {user.Surname}";
                    givenNameLabel.Text = $"Имя: {user.GivenName}";
                    initialsLabel.Text = $"Инициалы: {user.Initials}";
                    titleLabel.Text = $"Должность: {user.Title}";
                    displayNameLabel.Text = $"Отображаемое имя: {user.DisplayName}";
                    descriptionLabel.Text = $"Описание: {user.Description}";
                    officeLabel.Text = $"Комната: {user.Office}";
                    telephoneLabel.Text = $"Номер телефона: {user.TelephoneNumber}";
                    emailLabel.Text = $"Email: {user.Email}";
                    webPageLabel.Text = $"Веб-страница: {user.WebPage}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении данных пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Обновляем положение FlowLayoutPanel при изменении размера панели
            panel.Resize += (s, e) =>
            {
                flowPanel.Location = new Point((panel.ClientSize.Width - flowPanel.Width) / 2, 0);
            };

            return panel;
        }
    }

    public class LoginForm : Form
    {
        public string DomainController => domainControllerTextBox.Text;
        public string BaseDn => baseDnTextBox.Text;
        public string Username => usernameTextBox.Text;
        public string Password => passwordTextBox.Text;

        private readonly TextBox domainControllerTextBox;
        private readonly TextBox baseDnTextBox;
        private readonly TextBox usernameTextBox;
        private readonly TextBox passwordTextBox;

        public LoginForm()
        {
            Text = "Вход в Active Directory";
            Size = new Size(400, 300);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen; // Окно открывается по центру экрана
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            // Создаем FlowLayoutPanel для центрирования содержимого
            var flowPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Anchor = AnchorStyles.None,
                Location = new Point((ClientSize.Width - 350) / 2, 0) // Центрируем по горизонтали
            };

            var domainControllerPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 10, 0, 10)
            };
            var domainControllerLabel = new Label
            {
                Text = "IP сервера:",
                Width = 100,
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            domainControllerTextBox = new TextBox
            {
                Width = 200,
                Text = "",
                BorderStyle = BorderStyle.FixedSingle
            };
            domainControllerPanel.Controls.Add(domainControllerLabel);
            domainControllerPanel.Controls.Add(domainControllerTextBox);
            flowPanel.Controls.Add(domainControllerPanel);

            var baseDnPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var baseDnLabel = new Label
            {
                Text = "BASE DN:",
                Width = 100,
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            baseDnTextBox = new TextBox
            {
                Width = 200,
                Text = "DC=,DC=",
                BorderStyle = BorderStyle.FixedSingle
            };
            baseDnPanel.Controls.Add(baseDnLabel);
            baseDnPanel.Controls.Add(baseDnTextBox);
            flowPanel.Controls.Add(baseDnPanel);

            var usernamePanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var usernameLabel = new Label
            {
                Text = "Имя пользователя:",
                Width = 100,
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            usernameTextBox = new TextBox
            {
                Width = 200,
                BorderStyle = BorderStyle.FixedSingle
            };
            usernamePanel.Controls.Add(usernameLabel);
            usernamePanel.Controls.Add(usernameTextBox);
            flowPanel.Controls.Add(usernamePanel);

            var passwordPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 20)
            };
            var passwordLabel = new Label
            {
                Text = "Пароль:",
                Width = 100,
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            passwordTextBox = new TextBox
            {
                Width = 200,
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            passwordPanel.Controls.Add(passwordLabel);
            passwordPanel.Controls.Add(passwordTextBox);
            flowPanel.Controls.Add(passwordPanel);

            var loginButton = new Button
            {
                Text = "Войти",
                Width = 100,
                Height = 40,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(110, 0, 0, 0) // Центрируем кнопку
            };
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Click += (s, e) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
            flowPanel.Controls.Add(loginButton);

            Controls.Add(flowPanel);

            // Обновляем положение FlowLayoutPanel при изменении размера формы
            Resize += (s, e) =>
            {
                flowPanel.Location = new Point((ClientSize.Width - flowPanel.Width) / 2, 20);
            };
        }
    }
}