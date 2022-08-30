using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PU_Test.Pages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        ViewModel.SettingPage vm = new ViewModel.SettingPage();
        public SettingPage()
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void CloseDialog(object sender, RoutedEventArgs e)
        {
            GlobalValues.frame.Visibility = Visibility.Collapsed;



        }

        private void GoToBroswer(object sender, MouseButtonEventArgs e)
        {
            dynamic control = sender;
            var url = control.Tag.ToString();
            Process.Start("explorer.exe", url);
        }
    }

}
