using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BibliotekaWPF
{
    /// <summary>
    /// Logika interakcji dla klasy ApplicationPage.xaml
    /// Strona ApplicationPage to 4 z 4 stron umieszczanych w obiekcie MainFrame podczas działania programu.
    /// </summary>
    public partial class ApplicationPage : Page
    {
        /// <summary>
        /// Właściwość Mw jest ustawiana przez konstruktor klasy ApplicationPage na okno główne aplikacji.
        /// Kontekst służący do odwoływania się do właściwości okna głównego.
        /// Właściwość Login typu int reprezentuje identyfikator aktualnie zalogowanego do aplikacji czytelnika.
        /// Właściwość ReturnBook w trakcie działania programu przyjmuje instancję klasy ReturnBookWindow, czyli okno zwrotu wypożyczonych książek.
        /// Dopóki użytkownik nie otworzy okna zwrotu książek naciśnięciem odpowiedniego przycisku właściwość jest ustawiana domyślnie przez konstruktor na null.
        /// </summary>

        MainWindow mw;
        ReturnBookWindow returnBook;
        int login;
        public int Login { get => login; set => login = value; }
        public MainWindow Mw { get => mw; set => mw = value; }
        public ReturnBookWindow ReturnBook { get => returnBook; set => returnBook = value; }

        /// <summary>
        /// Konstruktor ustawia właściwość Login na podaną podczas logowania.
        /// </summary>
        public ApplicationPage(int _login)
        {
            InitializeComponent();
            Mw = (MainWindow)Application.Current.MainWindow;
            Login = _login;
            ReturnBook = null;
            User.Content = User.Content.ToString() + " " + Login;
        }

        /// <summary>
        /// Metoda GenerateUserPage renderuje dla użytkownika panel z książkami dostępnymi aktualnie do wypożyczenia.
        /// Dostępne książki to wszystkie książki z podłączonej bazy danych takie, że w polu Wypozyczona przechowują wartość 0 (false).
        /// Pojedyńcza książka wyświetlana jest jako "Książka - Autor".
        /// </summary>

        public void GenerateUserPage()
        {
            BooksList.Items.Clear();
                
            using (var dbContext = new BibliotekaDBContext())
            {
                var books = dbContext.Ksiazki.Where(book => book.Wypozyczona == false);
                foreach (Ksiazki book in books)
                {
                    var Autor = dbContext.Autorzy.Where(autor => autor.IDAutora == book.Autor)
                                                  .Select(autor => autor.ImieAutora)
                                                  .First();
                    Autor = Autor.TrimEnd(' ');
                    Autor += ' ';

                    Autor += dbContext.Autorzy.Where(autor => autor.IDAutora == book.Autor)
                                                  .Select(autor => autor.NazwiskoAutora)
                                                  .First();
                    Autor = Autor.TrimEnd(' ');

                    BooksList.Items.Add(new ListBoxItem() {Tag = (int)book.IDKsiazki, Content = $"\"{book.Tytul.TrimEnd(' ')}\" - {Autor}", FontSize=12 });
                    
                }
            }
        }

        /// <summary>
        /// Metoda LogOutButtonClick powoduje wylogowanie użytkownika (powrót do strony logowania) oraz zamknięcie okna zwrotu książek (ReturnBookWindow) jeżeli było ono otwarte.
        /// Ustawia też wartość ReturnBook na domyślne null.
        /// </summary>

        private void LogOutButtonClick(object sender, RoutedEventArgs e)
        {
            Mw.MainFrame.Content = new LogInPage();
            if (ReturnBook != null)
            {
                ReturnBook.Close();
                ReturnBook = null;
            }
        }

        /// <summary>
        /// Metoda RentBookButtonClick powoduje utworzenie obiektu (zdarzenia) typu Wypozyczenie i przypisanie wartości do jego wybranych pól.
        /// Obiekt zostanie utworzony wyłącznie jeśli użytkownik wybrał z listy książkę, którą chciałby wypożyczyć.
        /// Pole Wypozyczona w podpiętej bazie danych jest ustawiane na true.
        /// Obiekt typu Wypozyczenie zostaje dodany do tabeli w bazie danych i zostają zapisane zmiany.
        /// Następuje ponowne wygenerowanie listy dostępnych książek bez wypożyczonej książki 
        /// oraz ponowne wygenerowanie listy wypożyczonych przez użytkownika książek (jeżeli istnieje okno ReturnBookWindow).
        /// </summary>

        private void RentBookButtonClick(object sender, RoutedEventArgs e)
        {
            if (BooksList.SelectedItem != null)
            {
                using (var dbContext = new BibliotekaDBContext())
                {
                    ListBoxItem item = (ListBoxItem)BooksList.SelectedItem;
                    int tag = (int)item.Tag;
                    dbContext.Ksiazki.Where(book => book.IDKsiazki == tag).First().Wypozyczona = true;

                    var wypozyczenie = new Wypozyczenia()
                    {
                        IDCzytelnika = Login,
                        IDKsiazki = tag,
                        DataWypozyczenia = DateTime.Now,
                        DataOddania = null,
                        StatusWypozyczenia = "AKTYWNE"
                    };

                    dbContext.Wypozyczenia.Add(wypozyczenie);

                    dbContext.SaveChanges();
                }
                this.GenerateUserPage();
                if (ReturnBook != null) ReturnBook.GenerateReturnPage();
            }
        }

        /// <summary>
        /// Metoda ReturnBookButtonClick otwiera nowe okno ReturnBookWindow i przekazuje jako parametr identyfikator użytkownika.
        /// Następnie renderuje listę książek wypożyczonych przez użytkownika.
        /// </summary>

        private void ReturnBookButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReturnBook == null)
            {
                ReturnBook = new ReturnBookWindow(Login);
                ReturnBook.GenerateReturnPage();
                ReturnBook.Show();
            }
        }
    }
}
