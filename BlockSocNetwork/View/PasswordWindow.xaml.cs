using System.Windows;
using System.Windows.Forms;
using System;
using System.ComponentModel;

namespace BlockSocNetwork
{
    /// <summary>
    /// Логика взаимодействия для PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        private bool isFirstEnter = false;
        private bool isStartProgarm = true;
        MainWindow mainWindow = new MainWindow();
        
        public PasswordWindow()
        {
            InitializeComponent();

            Properties.Settings.Default.isClose = false;
            Properties.Settings.Default.Save();

            if (Properties.Settings.Default.password == "")
            {
                FirstEnter.Visibility = Visibility.Visible;
                isFirstEnter = true;
            }
            else
            {
                Enter.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isFirstEnter)
            {
                Properties.Settings.Default.password = Password.Password;
                Properties.Settings.Default.Save();
            }
            else
            {
                if (!(Properties.Settings.Default.password == Password.Password))
                {
                    Error.Visibility = Visibility.Visible;
                    return;
                }
            }

            Password.Password = "";

            if(isStartProgarm)
            {
                isStartProgarm = false;
                mainWindow.Show();              
            }
            else
            {
                mainWindow.Visibility = Visibility.Visible;
            }

            PswdWindow.Visibility = Visibility.Hidden;
        }

        public void PasswordWindow_Closing(object sender, CancelEventArgs e)
        {
            if (Properties.Settings.Default.isClose)
                e.Cancel = false;
            else
                e.Cancel = true;
        }


        private NotifyIcon TrayIcon = null;
        // переопределяем обработку первичной инициализации приложения
        protected override void OnSourceInitialized(EventArgs e)
        {
            // базовый функционал приложения в момент запуска
            base.OnSourceInitialized(e);                    
            createTrayIcon(); 
        }

        private bool createTrayIcon()
        {
            bool result = false;
            // только если не создана иконка ранее, создается новая
            if (TrayIcon == null)
            { 
                TrayIcon = new NotifyIcon(); 
                TrayIcon.Icon = Properties.Resources.Block;                
                TrayIcon.Text = "Блокировщик";                              

                TrayIcon.Click += delegate (object sender, EventArgs e) {
                    if ((e as MouseEventArgs).Button == MouseButtons.Left)
                    {
                        // по левой кнопке показываем или прячем окно
                        ShowHideMainWindow(sender, null);
                    }                   
                };
                result = true;
            }
            else
            { // все переменные были созданы ранее
                result = true;
            }
            // делаем иконку видимой в трее
            TrayIcon.Visible = true; 
            return result;
        }

        private void ShowHideMainWindow(object sender, RoutedEventArgs e)
        {
            if (IsVisible)
            {
                Hide();
            }
            else
            {
                try
                {
                    Show();
                    // меняем надпись на пункте меню
                    WindowState = CurrentWindowState;
                    // отдать фокус окну
                    Activate();
                }
                 catch { }                                          
            }
        }

        private WindowState fCurrentWindowState = WindowState.Normal;
        public WindowState CurrentWindowState
        {
            get { return fCurrentWindowState; }
            set { fCurrentWindowState = value; }
        }

        // переопределяем встроенную реакцию на изменение состояния сознания окна
        protected override void OnStateChanged(EventArgs e)
        {
            // системная обработка
            base.OnStateChanged(e); 
            if (this.WindowState == WindowState.Minimized)
            {
                Hide();
            }
            else
            {
                CurrentWindowState = WindowState;
            }
        }
    }
}
