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
using System.Windows.Forms.DataVisualization.Charting;

namespace BlockSocNetwork
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // построение графика
                List<WebsiteStatisticsModel> statisticsList = new List<WebsiteStatisticsModel>();
                statisticsList = statistics.Get();

                IEnumerable<string> domainList = new List<string>();
                domainList = statisticsList.Select(x => x.Name).Distinct();

                IEnumerable<string> datesList = new List<string>();

                chart.ChartAreas.Add(new ChartArea("Graphics"));
                chart.ChartAreas["Graphics"].AxisX.Title = "Дата";
                chart.ChartAreas["Graphics"].AxisY.Title = "Время (мин)";
                chart.ChartAreas["Graphics"].AxisX.MajorGrid.Enabled = false;
                chart.ChartAreas["Graphics"].AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;

                foreach (var site in domainList)
                {
                    List<double> timeList = new List<double>();
                    double time = 0;
                    foreach (var item in statisticsList)
                    {
                        if (site == item.Name)
                        {
                            time = (Convert.ToDouble(item.Time.ToString().Split(':')[0])) * 60 + (Convert.ToDouble(item.Time.ToString().Split(':')[1])) + (Convert.ToDouble(item.Time.ToString().Split(':')[2])) / 60;
                            timeList.Add(time);
                        }
                    }

                    datesList = statisticsList.Where(s => s.Name == site).Select(x => x.Date.Split('.')[0] + "." + x.Date.Split('.')[1]).Distinct();

                    string[] xData = datesList.ToArray();
                    double[] yData = timeList.ToArray();

                    string name = site;
                    chart.Series.Add(new Series(name));
                    chart.Series[name].ChartArea = "Graphics";
                    chart.Series[name].ChartType = SeriesChartType.Line;
                    chart.Series[name].Points.DataBindXY(xData, yData);
                    chart.Series[name].ToolTip = site;
                    chart.Series[name].BorderWidth = 5;
                    chart.Legends.Add(site);
                    chart.Legends[site].Docking = Docking.Bottom;
                    chart.Legends[site].LegendStyle = LegendStyle.Table;
                    chart.Legends[site].Alignment = System.Drawing.StringAlignment.Center;
                }             
            }
           catch
            {
                TextStatisics.Visibility = Visibility.Visible;
                ChartTitle.Visibility = Visibility.Hidden;
            }
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
            if (!Properties.Settings.Default.isClose)
            {
                e.Cancel = true;
                BigWindow.Visibility = Visibility.Hidden;
                var pswdWindow = Application.Current.Windows[1];
                pswdWindow.Visibility = Visibility.Visible;
            }           
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isClose = true;
            Properties.Settings.Default.Save();
            statistics.Write(mainCode.statisticsWebsites);

            //закрываем все окна
            while(Application.Current.Windows.Count != 0)
            {
                Application.Current.Windows[0].Close();
            }
        }
    }

   
}
