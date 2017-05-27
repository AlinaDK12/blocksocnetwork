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
        private bool isClose = false;
        MainCode mainCode = new MainCode();
        Statistics statistics = new Statistics();

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
        
        private void MenuItemTimeSetting_Click(object sender, RoutedEventArgs e)
        {
            TabControl.SelectedItem = TimeSetting;
        }

        private void MenuItemWebsitesSetting_Click(object sender, RoutedEventArgs e)
        {
            TabControl.SelectedItem = WebsitesSetting;
        }

        private void MenuItemStatistics_Click(object sender, RoutedEventArgs e)
        {
            TabControl.SelectedItem = Statistics;
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        private void MenuItemAboutProgram_Click(object sender, RoutedEventArgs e)
        {
            AboutProgramWindow aboutProgramWindow = new AboutProgramWindow();
            aboutProgramWindow.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!isClose)
            {
                PasswordWindow passwordWindow = new PasswordWindow();
                passwordWindow.Show();
            }           
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            isClose = true;
            statistics.Write(mainCode.statisticsWebsites);
            Close();
        }
    }

   
}
