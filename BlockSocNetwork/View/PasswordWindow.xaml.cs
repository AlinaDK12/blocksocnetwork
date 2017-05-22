using System.Windows;

namespace BlockSocNetwork
{
    /// <summary>
    /// Логика взаимодействия для PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        private bool isFirst = false;

        public PasswordWindow()
        {
            InitializeComponent();

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

            Close();
        }
    }
}
