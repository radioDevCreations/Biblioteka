using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BibliotekaWPF
{
    /// <summary>
    /// Logika interakcji dla klasy LogInPage.xaml
    /// Strona LogInPage to 1 z 4 stron umieszczanych w obiekcie MainFrame podczas działania programu.
    /// </summary>
    public partial class LogInPage : Page
    {
        /// <summary>
        /// Właściwość Mw jest ustawiana przez konstruktor klasy LogInPage na okno główne aplikacji.
        /// Kontekst służący do odwoływania się do właściwości okna głównego.
        /// </summary>

        MainWindow Mw { get; set; }
        public LogInPage()
        {
            InitializeComponent();
            Mw = (MainWindow)Application.Current.MainWindow;
        }

        /// <summary>
        /// Metoda LogInButtonClick jest wywoływana po naciśnięciu przez użytkownika przycisku "LogInButton".
        /// Metoda ta wyszukuje użytkownika w bazie danych po jego identyfikatorze (IDCzytelnika).
        /// Jeżeli logowanie się powiodło to metoda ustawia zawartość MainFrame na panel aplikacji (ApplicationPage).
        /// Jeżeli logowanie się nie powiodło w programie wyrzucany jest wyjątek, a metoda wyświetla użytkownikowi stosowny komunikat.
        /// </summary>

        private void LogInButtonClick(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new BibliotekaDBContext())
            {
                    int target;
                    bool success = Int32.TryParse(this.ReaderID.Text, out target);
                    var login = dbContext.Czytelnicy
                        .Where(reader => reader.IDCzytelnika == target);

                if (success)
                {
                    try
                    {
                        if (login.Count() == 1)
                        {
                            ApplicationPage app = new ApplicationPage(target);
                            Mw.MainFrame.Content = app;
                            app.GenerateUserPage();
                        }
                        else
                        {
                            throw new LogInException();
                        }
                    }
                    catch
                    {
                        LogInExceptionComunicate.Text = "Brak użytkownika w bazie.";
                    }
                }
                else
                {
                    LogInExceptionComunicate.Text = "Błędny login.";
                }
            }

        }

        /// <summary>
        /// Metoda ToSignUpPage jest wywoływana po naciśnięciu hiperłącza "Dołącz!".
        /// Metoda ta ustawia zawartość MainFrame na stronę rejestracji użytkownika (SignUpPage).
        /// </summary>

        private void ToSignUpPageClick(object sender, RoutedEventArgs e)
        {
            Mw.MainFrame.Content = new SignUpPage();
        }

        /// <summary>
        /// Klasa wyjątku LogInException.
        /// Klasa dziedziczy po klasie System.Exception.
        /// </summary>

        public class LogInException : Exception
        {
            public LogInException()
            {
            }

            public LogInException(string message)
                : base(message)
            {
            }

            public LogInException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}
