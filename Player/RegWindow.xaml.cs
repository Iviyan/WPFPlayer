using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Player
{
    /// <summary>
    /// Логика взаимодействия для RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        Settings settings;
        public Account Account { get; set; }

        public RegWindow(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(LoginTB.Text) || String.IsNullOrWhiteSpace(PassTB.Password))
            {
                MessageBox.Show("Одно из полей не заполнено");
                return;
            }

            Account acc = new(LoginTB.Text, PassTB.Password);
            if (settings.AddAccount(acc))
            {
                Account = acc;
                MessageBox.Show("Регистрация успешно завершена", "Успех");
                this.DialogResult = true;
            } else
            {
                MessageBox.Show("Такой аккаунт уже сущетсвует", "Ошибка регистрации");
            }
        }
    }
}
