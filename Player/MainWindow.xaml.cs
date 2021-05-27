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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowModel vm = new();
        Settings settings = new();
        LocalSettings localSettings;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = vm;

            MediaControlBar.Playlist = new List<string>() {
                /*...*/
            };

            AccountMenu = this.Resources["AccountCMenu"] as ContextMenu;

            localSettings = settings.GetLocalSettings();
            if (localSettings.Login != null)
            {
                Account acc = settings.Auth(localSettings.Login, localSettings.Password);
                if (acc != null)
                {
                    vm.Login = acc.Login;
                    vm.IsPro = acc.Pro;
                }
            }
        }

        ContextMenu AccountMenu;
        private void AccountBtn_Click(object sender, RoutedEventArgs e)
        {
            AccountMenu.PlacementTarget = sender as UIElement;
            AccountMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            AccountMenu.IsOpen = true;
        }

        private void AuthMI_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new(settings);
            authWindow.ShowDialog();
            Account acc = authWindow.Account;
            if (acc != null)
            {
                localSettings.Login = vm.Login = acc.Login;
                vm.IsPro = acc.Pro;
                localSettings.Password = acc.Password;
            }
        }

        private void RegMI_Click(object sender, RoutedEventArgs e)
        {
            RegWindow regWindow = new(settings);
            bool? res = regWindow.ShowDialog();
            if (res == true)
            {
                Account acc = regWindow.Account;
                localSettings.Login = vm.Login = acc.Login;
                vm.IsPro = acc.Pro;
                localSettings.Password = acc.Password;
            }
        }

        private void LogoutMI_Click(object sender, RoutedEventArgs e)
        {
            vm.Login = null;
            localSettings.Login = localSettings.Password = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.UpdateLocalSettings(localSettings);
        }

        private void BuyProMI_Click(object sender, RoutedEventArgs e)
        {
            bool r = settings.UpdateAccount(vm.Login, pro: true);
            if (r) vm.IsPro = true;
        }
    }
}
