using System.Windows;
using System.Windows.Controls;

namespace BibliotekaWPF
{
    /// <summary>
    /// Logika interakcji dla klasy IDPage.xaml
    /// Strona IDPage to 3 z 4 stron umieszczanych w obiekcie MainFrame podczas działania programu.
    /// </summary>
    public partial class IDPage : Page
    {
        /// <summary>
        /// Właściwość Mw jest ustawiana przez konstruktor klasy IDPage na okno główne aplikacji.
        /// Kontekst służący do odwoływania się do właściwości okna głównego.
        /// </summary>

        MainWindow Mw { get; set; }

        /// <summary>
        /// Konstruktor ustawia zawartość pola IDField na identyfikator nowego użytkownika przekazany do konstruktora w parametrze.
        /// Zawartość pola IDField jest wyświetlana na środku strony w celu zaprezentowania użytkownikowi jego identyfikatora (niezbędnego przy logowaniu się do aplikacji).
        /// </summary>
        
        public IDPage(int userID)
        {
            InitializeComponent();
            Mw = (MainWindow)Application.Current.MainWindow;
            IDField.Content += " ";
            IDField.Content += userID.ToString();
        }

        /// <summary>
        /// Metoda NextButtonClick jest wywoływana po naciśnięciu przycisku "NextButton".
        /// Metoda ta ustawia zawartość MainFrame na stronę logowania użytkownika (LogInPage).
        /// </summary>

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            Mw.MainFrame.Content = new LogInPage();
        }
    }
}
