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
using System.ComponentModel;

namespace BlockSocNetwork
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CheckTime.IsChecked = Properties.Settings.Default.isCheckedTime;
            CheckDayTime.IsChecked = Properties.Settings.Default.isCheckedDayTime;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        public void checkboxTime_Checked(object sender, RoutedEventArgs e)
        {
            MainCode.isCheckedTime = true;
            Properties.Settings.Default.isCheckedTime = true;
            Properties.Settings.Default.Save();
        }

        public void checkboxTime_Unchecked(object sender, RoutedEventArgs e)
        {
            MainCode.isCheckedTime = false;
            Properties.Settings.Default.isCheckedTime = false;
            Properties.Settings.Default.Save();
        }

        public void checkboxDayTime_Checked(object sender, RoutedEventArgs e)
        {
            MainCode.isCheckedDayTime = true;
            Properties.Settings.Default.isCheckedDayTime = true;
            Properties.Settings.Default.Save();
        }

        public void checkboxDayTime_Unchecked(object sender, RoutedEventArgs e)
        {
            MainCode.isCheckedDayTime = false;
            Properties.Settings.Default.isCheckedDayTime = false;
            Properties.Settings.Default.Save();
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            //PasswordWindow passwordWindow = new PasswordWindow();
            //passwordWindow.Show();
        }
    }

   
}
