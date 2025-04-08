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
        private TextBox? _surnameTextBox;
        private TextBox? _givenNameTextBox;
        private TextBox? _displayNameTextBox;
        private ComboBox? _displayNameOrderComboBox;

        public MainForm()
        {
            StartPosition = FormStartPosition.CenterScreen; // Окно открывается по центру экрана
            MinimumSize = new Size(600, 700); // Минимальный размер окна
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
            _surnameTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            surnamePanel.Controls.Add(surnameLabel);
            surnamePanel.Controls.Add(_surnameTextBox);
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
            _givenNameTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            givenNamePanel.Controls.Add(givenNameLabel);
            givenNamePanel.Controls.Add(_givenNameTextBox);
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
            _displayNameTextBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            displayNamePanel.Controls.Add(displayNameLabel);
            displayNamePanel.Controls.Add(_displayNameTextBox);
            flowPanel.Controls.Add(displayNamePanel);

            // Панель для выбора порядка отображения
            var displayNameOrderPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };
            var displayNameOrderLabel = new Label
            {
                Text = "Порядок отображения:",
                Width = 150,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Margin = new Padding(0, 5, 10, 0)
            };
            _displayNameOrderComboBox = new ComboBox
            {
                Width = 150,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _displayNameOrderComboBox.Items.Add("Фамилия Имя");
            _displayNameOrderComboBox.Items.Add("Имя Фамилия");
            _displayNameOrderComboBox.SelectedIndex = 0; // По умолчанию "Фамилия Имя"
            displayNameOrderPanel.Controls.Add(displayNameOrderLabel);
            displayNameOrderPanel.Controls.Add(_displayNameOrderComboBox);
            flowPanel.Controls.Add(displayNameOrderPanel);

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

            // Метод для обновления поля "Отображаемое имя"
            void UpdateDisplayName()
            {
                if (_surnameTextBox == null || _givenNameTextBox == null || _displayNameTextBox == null || _displayNameOrderComboBox == null)
                    return;

                string surname = _surnameTextBox.Text.Trim();
                string givenName = _givenNameTextBox.Text.Trim();
                bool surnameFirst = _displayNameOrderComboBox.SelectedIndex == 0; // true: Фамилия Имя, false: Имя Фамилия

                if (string.IsNullOrEmpty(surname) && string.IsNullOrEmpty(givenName))
                {
                    _displayNameTextBox.Text = string.Empty;
                    return;
                }

                _displayNameTextBox.Text = surnameFirst
                    ? $"{surname} {givenName}".Trim()
                    : $"{givenName} {surname}".Trim();
            }

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
                _surnameTextBox!.Text = user.Surname;
                _givenNameTextBox!.Text = user.GivenName;
                initialsTextBox.Text = user.Initials;
                titleTextBox.Text = user.Title;
                descriptionTextBox.Text = user.Description;
                officeTextBox.Text = user.Office;
                telephoneTextBox.Text = user.TelephoneNumber;
                emailTextBox.Text = user.Email;
                webPageTextBox.Text = user.WebPage;

                // Устанавливаем DisplayName
                if (string.IsNullOrEmpty(user.DisplayName))
                {
                    UpdateDisplayName(); // Если DisplayName пустое, формируем его на основе порядка
                }
                else
                {
                    _displayNameTextBox!.Text = user.DisplayName;
                }
            };

            // Обработчик изменения порядка отображения
            _displayNameOrderComboBox!.SelectedIndexChanged += (s, e) =>
            {
                UpdateDisplayName();
            };

            // Обработчики изменения текста в полях "Фамилия" и "Имя"
            _surnameTextBox!.TextChanged += (s, e) =>
            {
                UpdateDisplayName();
            };

            _givenNameTextBox!.TextChanged += (s, e) =>
            {
                UpdateDisplayName();
            };

            saveButton.Click += (s, e) =>
            {
                var updatedUser = new User
                {
                    Surname = _surnameTextBox!.Text,
                    GivenName = _givenNameTextBox!.Text,
                    Initials = initialsTextBox.Text,
                    DisplayName = _displayNameTextBox!.Text,
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
                Height = 150,
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

            panel.Controls.Add(flowPanel);

            // Обработчик события выбора подразделения
            ouListBox.SelectedIndexChanged += (s, e) =>
            {
                usersListBox.Items.Clear();

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
                Text = "BASE_DN:",
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