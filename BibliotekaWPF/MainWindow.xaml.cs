using System.Windows;

namespace BibliotekaWPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// Komponent MainWindow to 1 z 2 okien dostępnych dla użytkownika w aplikacji Biblioteka.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Zainicjalizowany komponent MainWindow tworzy nową stronę logowania i umieszcza ją w obiekcie MainFrame.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.MainFrame.Content = new LogInPage();
        }

        /// <summary>
        /// Metoda MainWindowClosing wywoływana jest przy zamknięciu głównego okna (obiektu MainWindow) aplikacji.
        /// Metoda MainWindowClosing odpowiada za zamknięcie całego programu.
        /// </summary>
        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
