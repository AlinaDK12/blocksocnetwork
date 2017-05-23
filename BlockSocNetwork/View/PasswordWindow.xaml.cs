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
        private bool isFirst = false;
        private bool isEnter = false;

        public PasswordWindow()
        {
            InitializeComponent();

            isEnter = false; 

            if (Properties.Settings.Default.password == "")
            {
                FirstEnter.Visibility = Visibility.Visible;
                isFirst = true;
            }
            else
            {
                Enter.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isFirst)
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
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            isEnter = true;
            Close();
        }

        public void PasswordWindow_Closing(object sender, CancelEventArgs e)
        {
            if (isEnter)
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
                Show();
                // меняем надпись на пункте меню
                WindowState = CurrentWindowState;
                // отдать фокус окну
                Activate();                                             
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
