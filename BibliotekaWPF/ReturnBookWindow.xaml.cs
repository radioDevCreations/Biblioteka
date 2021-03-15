using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BibliotekaWPF
{
    /// <summary>
    /// Logika interakcji dla klasy ReturnBookWindow.xaml
    /// Komponent ReturnBookWindow to 2 z 2 okien dostępnych dla użytkownika w aplikacji Biblioteka.
    /// </summary>
    public partial class ReturnBookWindow : Window
    {

        /// <summary>
        /// Właściwość Mw jest ustawiana przez konstruktor klasy ApplicationPage na okno główne aplikacji.
        /// Kontekst służący do odwoływania się do właściwości okna głównego.
        /// Właściwość Login typu int reprezentuje identyfikator aktualnie zalogowanego do aplikacji czytelnika.
        /// Właściwość ApplicationContext jest kontekst panelu użytkownika, dla którego wywołane zostało okno zwrotu książek.
        /// W programie może istnieć wyłącznie jedna instancja Typu ReturnBookWindow jednocześnie.
        /// </summary>

        MainWindow mw;
        ApplicationPage applicationContext;
        private int login;
        public int Login { get => login; set => login = value; }
        public MainWindow Mw { get => mw; set => mw = value; }
        public ApplicationPage ApplicationContext { get => applicationContext; set => applicationContext = value; }


        public ReturnBookWindow(int _login)
        {
            InitializeComponent();
            Mw = (MainWindow)Application.Current.MainWindow;
            Login = _login;
            ApplicationContext = (ApplicationPage)Mw.MainFrame.Content;
            User.Content = User.Content.ToString() + " " + Login;
        }

        /// <summary>
        /// Metoda GenerateReturnPage renderuje dla użytkownika panel z książkami wypożyczonymi przez niego,
        /// które z pomocą tego panelu może oddać do biblioteki.
        /// Lista książek jest tworzona na podstawie tabeli Wypożyczenia z podłączonej bazy danych.
        /// Brane pod uwagę są takie wypożyczenia, które były wykonane przez zalogowanego użytkowanika i mają StatusWypozyczenia "AKTYWNE".
        /// Każda następna wypożyczona książka pojawia się w tym panelu po ponownym jego wyrenderowaniu.
        /// Każda zwrócona przez użytkownika książka pojawi się w panelu użytkownika kontekstowej aplikacji.
        /// </summary>

        public void GenerateReturnPage()
        {
            BooksList.Items.Clear();

            using (var dbContext = new BibliotekaDBContext())
            {
                var rents = dbContext.Wypozyczenia.Where(rent => (rent.IDCzytelnika == Login && rent.StatusWypozyczenia == "AKTYWNE"));
                foreach (Wypozyczenia rent in rents)
                {
                    var Book = dbContext.Ksiazki.Where(book => book.IDKsiazki == rent.IDKsiazki)
                                                  .Select(book => book)
                                                  .First();
                    var Autor = dbContext.Autorzy.Where(autor => autor.IDAutora == Book.Autor)
                                                  .Select(autor => autor.ImieAutora)
                                                  .First();
                    Autor = Autor.TrimEnd(' ');
                    Autor += ' ';

                    Autor += dbContext.Autorzy.Where(autor => autor.IDAutora == Book.Autor)
                                                  .Select(autor => autor.NazwiskoAutora)
                                                  .First();
                    Autor = Autor.TrimEnd(' ');

                    BooksList.Items.Add(new ListBoxItem() { Tag = (int)rent.IDWypozyczenia, Content = $"\"{Book.Tytul.TrimEnd(' ')}\" - {Autor}", FontSize = 12 });

                }
            }
        }

        /// <summary>
        /// Metoda ReturnBookDecisionButton wywoływana jest w momencie naciśnięcia przez użytkownika przycisku ReturnBookDecisionButton.
        /// Jeżeli użytkownik wybrał z listy książkę, którą zamierza zwrócić to dla danego wypożyczenia metoda zmienia StatusWypozyczenia na "ODDANE"
        /// oraz dla oddanej książki W polu Wypozyczona status zostaje zmieniony na 0 (false);
        /// Zmiany zostają zapisane, a panel użytkownika i panel zwrotu książek są ponownie renderowane.
        /// </summary>

        private void ReturnBookDecisionButtonClick(object sender, RoutedEventArgs e)
        {
            if (BooksList.SelectedItem != null)
            {
                using (var dbContext = new BibliotekaDBContext())
                {
                    ListBoxItem item = (ListBoxItem)BooksList.SelectedItem;
                    int tag = (int)item.Tag;
                    Wypozyczenia Rent = dbContext.Wypozyczenia.Where(rent => rent.IDWypozyczenia == tag).First();
                    Rent.StatusWypozyczenia = "ODDANE";
                    Rent.DataOddania = DateTime.Now;
                    dbContext.Ksiazki.Where(book => book.IDKsiazki == Rent.IDKsiazki).First().Wypozyczona = false;
                    dbContext.SaveChanges();
                }
                this.GenerateReturnPage();
                ApplicationContext.GenerateUserPage();
            }
        }

        /// <summary>
        /// Metoda ReturnBookWindowClosing przy zamknięciu okna zwrotu książek w dowolny sposób
        /// ustawia dla kontekstu aplikacji właściwość przechowującą instancję tego okna na null.
        /// </summary>

        private void ReturnBookWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplicationContext.ReturnBook = null;
        }
    }
}
