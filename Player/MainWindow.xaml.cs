using System;
using System.Collections.Generic;
using System.IO;
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
        MainWindowViewModel vm = new();
        Settings settings = new();
        LocalSettings localSettings;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = vm;

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

            vm.Playlists = new(settings.GetPlaylists());

            MediaControlBar.MediaOpened += MediaControlBar_MediaOpened;
        }

        private void MediaControlBar_MediaOpened()
        {
            if (media.HasVideo)
            {
                VideoView.Visibility = Visibility.Visible;
                ExpandVideo.Visibility = Visibility.Visible;
            } else
            {
                VideoView.Visibility = Visibility.Collapsed;
                ExpandVideo.Visibility = Visibility.Hidden;
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
            settings.UpdatePlaylists(vm.Playlists);
        }

        private void BuyProMI_Click(object sender, RoutedEventArgs e)
        {
            bool r = settings.UpdateAccount(vm.Login, pro: true);
            if (r) vm.IsPro = true;
        }

        static string[] mediaFileExtensions = { "wav", "aac", "wma", "wmv", "avi", "mpg", "mpeg", "m1v", "mp2", "mp3", "mpa", "mpe", "m3u", "mp4", "mov", "3g2", "3gp2", "3gp", "3gpp", "m4a", "cda", "aif", "aifc", "aiff", "mid", "midi", "rmi", "mkv" };
        private void PlaylistLB_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    if (File.Exists(file) && mediaFileExtensions.Contains(System.IO.Path.GetExtension(file).Substring(1).ToLower()))
                        vm.CurrentPlaylist.Items.Add(file);
                }
            }
        }

        private void PlaylistLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistLB.SelectedIndex >= 0)
                MediaControlBar.SetPlaylist(vm.CurrentPlaylist.Items, PlaylistLB.SelectedIndex);
        }

        private void CloseVideoViewBtn_Click(object sender, RoutedEventArgs e)
        {
            VideoView.Visibility = Visibility.Collapsed;
        }

        private void ExpandVideo_Click(object sender, RoutedEventArgs e)
        {
            VideoView.Visibility = Visibility.Visible;
        }

        private void ChangePassMI_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new(settings, localSettings.Login);
            bool? res = changePasswordWindow.ShowDialog();
            if (res == true)
            {
                localSettings.Password = changePasswordWindow.Password;
            }
        }
    }
}
