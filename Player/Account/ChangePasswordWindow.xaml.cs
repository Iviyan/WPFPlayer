using System;
using System.Windows;

namespace Player
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        Settings settings;
        public string Login { get; init; }
        public string Password { get; private set; }

        public ChangePasswordWindow(Settings settings, string login)
        {
            InitializeComponent();
            this.settings = settings;
            this.Login = login;
        }

        private void UpdatePassBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(PassTB.Password))
            {
                MessageBox.Show("Пароль не может быть пустым");
                return;
            }

            bool r = settings.UpdateAccount(Login, PassTB.Password);
            if (r)
            {
                Password = PassTB.Password;
                MessageBox.Show("Пароль успешно изменён");
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Ошибка смены пароля");
                this.DialogResult = false;
            }
        }
    }
}
