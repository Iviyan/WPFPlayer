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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        Settings settings;
        public Account Account { get; set; }

        public AuthWindow(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
        }

        private void AuthBtn_Click(object sender, RoutedEventArgs e)
        {
            Account = settings.Auth(LoginTB.Text, PassTB.Password);
            if (Account == null)
            {
                MessageBox.Show("Ошибка авторизции");
            } else
                this.DialogResult = true;
        }
    }
}
