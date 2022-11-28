using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ViewModel.MainWindow vm;
        public MainWindow()
        {
            InitializeComponent();
            GlobalValues.MainWindow = this;
            vm = new ViewModel.MainWindow();
            DataContext = vm;
            GlobalValues.frame = dialog_frame;
            Background.ImageSource = (ImageSource)new ImageSourceConverter().ConvertFrom(Launcher.Resources.Resources.bg);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            GlobalValues.frame.Visibility = Visibility.Visible;
            GlobalValues.frame.Navigate(new Pages.SettingPage());
        }

        private void WindowMinize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GoToBroswer(object sender, MouseButtonEventArgs e)
        {
            dynamic control = sender;
            var url = control.Tag.ToString();
            Process.Start("explorer.exe", url);
        }

        private void RefreshServerInfo(object sender, MouseButtonEventArgs e)
        {
            vm.UpdateSI();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            vm.UpdateSI();
        }

        private void Official_Sel(object sender, RoutedEventArgs e)
        {
            vm.Official_Set();

        }

        private void Private_Sel(object sender, RoutedEventArgs e)
        {
            vm.Private_Set();
        }


    }
}
