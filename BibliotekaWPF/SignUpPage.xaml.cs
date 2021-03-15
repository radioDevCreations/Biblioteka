using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BibliotekaWPF
{
    /// <summary>
    /// Logika interakcji dla klasy SignUpPage.xaml
    /// Strona SignUpPage to 2 z 4 stron umieszczanych w obiekcie MainFrame podczas działania programu.
    /// </summary>

    public partial class SignUpPage : Page
    {
        /// <summary>
        /// Właściwość Mw jest ustawiana przez konstruktor klasy SignUpPage na okno główne aplikacji.
        /// Kontekst służący do odwoływania się do właściwości okna głównego.
        /// Właściwość GenderBoxes typu string reprezentuje aktualnie zaznaczony na stronie rejestracji checkbox w polu PŁEĆ.
        /// Właściwość GenderBoxes przyjmuje w programie wartości "K" - kobieta, "M" - mężczyzna lub null, jeżeli żadna z płci nie została zaznaczona.
        /// </summary>
        MainWindow Mw { get; set; }
        string GenderBoxes { get; set; }
        public SignUpPage()
        {
            InitializeComponent();
            Mw = (MainWindow)Application.Current.MainWindow;
        }

        /// <summary>
        /// Metoda SignUpButtonClick jest wywoływana po naciśnięciu przez użytkownika przycisku "SignUpButton".
        /// Metoda ta próbuje:
        /// - wywołać dla wartości pól formularza rejestracji metodę ValidateSignUpForm
        /// - utworzyć kontekst bazy danych i obiekt repezentujący nowego czytelnika z przypisanymi zwalidowanymi wartościami z formularza rejestracji.
        /// - dodać użytkownika do bazy danych i zapisać zmiany
        /// - zapisać w zmiennej newUserID identyfikator nowo dodanego użytkownika.
        /// - pokazać użytkownikowi jego nowe id poprzez zmianę zawartości obiektu MainFrame na stonę prezentacji ID (IDPage)
        /// Jeżeli którakolwiek z powyższych czynności się nie powiodła w programie wyrzucany jest wyjątek, a metoda wyświetla użytkownikowi stosowny komunikat.
        /// </summary>
        /// /// <seealso cref="ValidateSignUpForm(string, string, string, string, string, string, string, string, string)">
        /// Metoda walidująca dane wprowadzone w formularzu rejestracji.
        /// </seealso>

        private void SignUpButtonClick(object sender, RoutedEventArgs e)
        {
            int newUserID = 0;
            try
            {

                ValidateSignUpForm(
                    this.NameBox.Text,
                    this.SurnameBox.Text,
                    this.GenderBoxes,
                    this.CityBox.Text,
                    this.PostalCodeBox.Text,
                    this.StreetBox.Text,
                    this.HouseNumberBox.Text,
                    this.LocalNumberBox.Text,
                    this.EmailBox.Text
                    );

                using (var dbContext = new BibliotekaDBContext())
                {
                    var czytelnik = new Czytelnicy()
                    {
                        ImieCzytelnika = this.NameBox.Text,
                        NazwiskoCzytelnika = this.SurnameBox.Text,
                        Plec = this.GenderBoxes,
                        DataPrzystapienia = DateTime.Now,
                        Miasto = this.CityBox.Text,
                        KodPocztowy = this.PostalCodeBox.Text,
                        Ulica = this.StreetBox.Text,
                        NumerDomu = this.HouseNumberBox.Text,
                        NumerLokalu = this.LocalNumberBox.Text,
                        EMail = this.EmailBox.Text
                    };

                    dbContext.Czytelnicy.Add(czytelnik);
                    dbContext.SaveChanges();

                    newUserID = czytelnik.IDCzytelnika;
                }
                Mw.MainFrame.Content = new IDPage(newUserID);
            }
            catch(SignUpException exception)
            {
                string Comunicate = null;
                switch (exception.Message)
                {
                    case "email":
                        Comunicate = "Błędny email.";
                        break;
                    case "numer domu":
                        Comunicate = "Błędny numer domu.";
                        break;
                    case "numer lokalu":
                        Comunicate = "Błędny numer lokalu.";
                        break;
                    case "ulica":
                        Comunicate = "Błędna nazwa ulicy.";
                        break;
                    case "kod pocztowy":
                        Comunicate = "Format kodu pocztowego: 00-000";
                        break;
                    case "miasto":
                        Comunicate = "Błędna nazwa miasta.";
                        break;
                    case "płeć":
                        Comunicate = "Wybierz płeć.";
                        break;
                    case "nazwisko":
                        Comunicate = "Niepoprawne nazwisko.";
                        break;
                    case "imię":
                        Comunicate = "Niepoprawne imię.";
                        break;
                    default:
                        Comunicate = "Nieznany błąd.";
                        break;
                }
                ComunicateBanner.Text = Comunicate;
            }

        }

        /// <summary>
        /// Metoda ValidateSignUpForm sprawdza poprawność danych wprowadzonych przez użytkownika w formularzu rejestracji.
        /// Właściwość e zawiera ostatni wyjątek utworzony, jeżeli któryś z parametrów metody nie przejdzie testu poprawności
        /// lub null ustawiony domyślnie, oraz nie zmieniany w przypadku kiedy wszystkie parametry są poprawne.
        /// Wyjątki zapisywane są do właściwości e ze skróconym komunikatem błędu we właściwości Message.
        /// Metoda w przypadku wystąpienia wyjątku wyrzuca go i ustawia kolor panelu komunikatu błądu na stronie na czerwony.
        /// </summary>

        private void ValidateSignUpForm(string _name, string _surname, string _gender, string _city, string _postalcode, string _street, string _housenumber, string _localnumber, string _email)
        {
            SignUpException e = null;
            int spacesCounter = 0;

            /// /// <param name="_name">
            /// Parametr reprezentuje imię czytelnika.
            /// Sprawdzenie czy zmienna nie jest pusta, oraz czy podane imie nie składa się z samych spacji.
            /// </param>

            if (_name.Length == 0 || _name == null)
            {
                e = new SignUpException("imię");
            }
            else
            {
                foreach (char c in _name)
                {
                    if (c == ' ') spacesCounter++;
                }
                if (spacesCounter == _name.Length)
                {
                    e = new SignUpException("imię");
                }
            }

            spacesCounter = 0;

            /// <param name="_surname">
            /// Parametr reprezentuje nazwisko czytelnika.
            /// Sprawdzenie czy zmienna nie jest pusta, oraz czy podane nazwisko nie składa się z samych spacji.
            /// </param>

            if (_surname.Length == 0 || _surname == null)
            {
                e = new SignUpException("nazwisko");
            }
            else
            {
                foreach (char c in _surname)
                {
                    if (c == ' ') spacesCounter++;
                }
                if (spacesCounter == _surname.Length)
                {
                    e = new SignUpException("nazwisko");
                }
            }

            spacesCounter = 0;

            /// <param name="_gender">
            /// Sprawdzenie czy został zaznaczony któryś z checkboxów reprezentujących płeć.
            /// </param>

            if (!(_gender == "M" || _gender == "K"))
            {
                e = new SignUpException("płeć");
            }

            /// <param name="_city">
            /// Parametr reprezentuje miejscowość zamieszkiwaną przez czytelnika.
            /// Sprawdzenie czy zmienna nie jest pusta, oraz czy podana miejscowość nie składa się z samych spacji.
            /// </param>

            if (_city.Length == 0 || _city == null)
            {
                e = new SignUpException("miasto");
            }
            else
            {
                foreach (char c in _city)
                {
                    if (c == ' ') spacesCounter++;
                }
                if (spacesCounter == _city.Length)
                {
                    e = new SignUpException("miasto");
                }
            }

            /// <param name="_postalcode">
            /// Parametr reprezentuje kod pocztowy mjejscowości zamieszkiwanej przez czytelnika.
            /// Sprawdzenie jest w wymaganym formacie "00-000".
            /// </param>

            spacesCounter = 0;

            if (_postalcode.Length != 6 || _postalcode == null)
            {
                e = new SignUpException("kod pocztowy");
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i == 2)
                    {
                        if (_postalcode[i] != '-') e = new SignUpException("kod pocztowy");
                    }
                    else
                    {
                        if (!Char.IsDigit(_postalcode[i]))
                        {
                            e = new SignUpException("kod pocztowy");
                        }
                    }
                }
            }

            /// <param name="_street">
            /// Parametr reprezentuje ulicę zamieszkiwaną przez czytelnika.
            /// Sprawdzenie czy zmienna nie jest pusta, oraz czy podana ulica nie składa się z samych spacji.
            /// </param>

            if (_street.Length == 0 || _street == null)
            {
                e = new SignUpException("ulica");
            }
            else
            {
                foreach (char c in _street)
                {
                    if (c == ' ') spacesCounter++;
                }
                if (spacesCounter == _street.Length)
                {
                    e = new SignUpException("ulica");
                }
            }

            spacesCounter = 0;

            /// <param name="_housenumber">
            /// Parametr reprezentuje numer domu czytelnika.
            /// Sprawdzenie czy zmienna nie jest pusta, nie jest dłuższa niż 5 znaków oraz czy podany numer składa się wyłącznie ze znaków alfanumerycznych.
            /// </param>


            if (_housenumber.Length == 0 || _housenumber == null || _housenumber.Length > 5)
            {
                e = new SignUpException("numer domu");
            }
            else
            {
                foreach (char c in _housenumber)
                {
                    if (!(Char.IsLetterOrDigit(c)))
                    {
                        e = new SignUpException("numer domu");
                        break;
                    }
                }
            }

            /// <param name="_localnumber">
            /// Parametr reprezentuje numer lokalu czytelnika.
            /// Sprawdzenie czy zmienna nie jest dłuższa niż 3 znaki.
            /// </param>

            if (_localnumber.Length > 3)
            {
                e = new SignUpException("numer lokalu");
            }

            /// <param name="_email">
            /// Parametr reprezentuje adres email czytelnika.
            /// Sprawdzenie czy zmienna jest w formacie example@email.sth
            /// </param>

            if (!(_email.Contains('@')))
            {
                e = new SignUpException("email");
            }
            else if (!(_email.Contains('.')))
            {
                e = new SignUpException("email");
            }
            else if ((_email.LastIndexOf('@')) > (_email.LastIndexOf('.')))
            {
                e = new SignUpException("email");
            }
            else if ((_email.IndexOf('@')) != (_email.LastIndexOf('@')))
            {
                e = new SignUpException("email");
            }
            else if (_email.IndexOf('@') == 0)
            {
                e = new SignUpException("email");
            }
            else if (_email.LastIndexOf('.') == _email.Length-1)
            {
                e = new SignUpException("email");
            }
            else if ((_email.LastIndexOf('.')) - (_email.IndexOf('@')) == 1)
            {
                e = new SignUpException("email");
            }

            if (e != null)
            {
                ComunicateBanner.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0, 0));
                throw e;
            }
        }

        /// <summary>
        /// Metoda FemaleChecked wywoływana jest po zaznaczeniu checkboxa female.
        /// </summary>

        private void FemaleChecked(object sender, RoutedEventArgs e)
        {
            this.male.IsChecked = false;
            GenderBoxes = "K";
        }

        /// <summary>
        /// Metoda GenderUnchecked wywoływana jest po odznaczeniu dowolnego checkboxa reprezentującego płeć czytelnika.
        /// </summary>

        private void GenderUnchecked(object sender, RoutedEventArgs e)
        {
            GenderBoxes = null;
        }

        /// <summary>
        /// Metoda MaleChecked wywoływana jest po zaznaczeniu checkboxa male.
        /// </summary>

        private void MaleChecked(object sender, RoutedEventArgs e)
        {
            this.female.IsChecked = false;
            GenderBoxes = "M";
        }

        /// <summary>
        /// Metoda ToLogInPage jest wywoływana po naciśnięciu hiperłącza "Zaloguj się!".
        /// Metoda ta ustawia zawartość MainFrame na stronę logowania użytkownika (LogInPage).
        /// </summary>

        private void ToLogInPageClick(object sender, RoutedEventArgs e)
        {
            Mw.MainFrame.Content = new LogInPage();
        }

        /// <summary>
        /// Klasa wyjątku SignUpException.
        /// Klasa dziedziczy po klasie System.Exception.
        /// </summary>

        public class SignUpException : Exception
        {
            public SignUpException()
            {
            }

            public SignUpException(string message)
                : base(message)
            {
            }

            public SignUpException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}
