using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfKorisnickiProgram.Klase;

namespace WpfKorisnickiProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private karticeDal kDal = new karticeDal();
        private List<kartice_tab> lista = new List<kartice_tab>();
        private int indeks = -1;
        private int operacija = -1;
                
        public MainWindow()
        {
            InitializeComponent();
        }
        private void DozvoliPromenu(bool dozvoli)
        {
            groupBox.IsEnabled = dozvoli;

        }
        private void Resetuj()
        {
            textBoxId.Clear();
            textBoxBrojKartice.Clear();
            textBoxIme.Clear();
            textBoxFakultet.Clear();
            textBoxTelefon.Clear();
            textBoxEmail.Clear();
            textBoxStanje.Clear();
            textBoxUplata.Clear();
            indeks = -1;
            textBoxBrojKartice.Focus();
            dataGrid1.SelectedIndex = -1;
            textBoxPretragaPoImenu.Clear();
            DozvoliPromenu(false);
        }        
        private bool Validacija()
        {
            int bk;
            if (!int.TryParse(textBoxBrojKartice.Text, out bk))
            {
                MessageBox.Show("Neispravan broj kartice");
                textBoxBrojKartice.Clear();
                textBoxBrojKartice.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBoxIme.Text))
            {
                MessageBox.Show("Unesite ime");
                textBoxIme.Focus();
                return false;
            }
            string Email = (string.IsNullOrWhiteSpace(textBoxEmail.Text)) ? null : textBoxEmail.Text;
            if (Email == null)
            {
                MessageBox.Show("Morate uneti e-mail");
                textBoxEmail.Focus();
                return false;
            }
            if (Email != null && !Regex.IsMatch(textBoxEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                MessageBox.Show("Morate uneti validan e-mail");
                textBoxEmail.Focus();
                return false;
            }
            return true;

        }
        private void PrikaziKorisnike()
        {
            lista = kDal.VratiKartice();
            if (lista!=null)
            {
                dataGrid1.ItemsSource = lista;
            }

        }
        private kartice_tab PrikaziKorisnika(int id)
        {
            foreach (kartice_tab k in dataGrid1.Items)
            {
                if (k.KarticaId == id)
                {
                    return k;
                }
            }
            return null;
        }
        private void Ubaci()
        {
            if (!Validacija())
            {
                return;
            }
            kartice_tab k = new kartice_tab();
            //k.KorisnikId = int.Parse(textBoxId.Text);
            k.brkartice = int.Parse(textBoxBrojKartice.Text);
            k.korisnik = textBoxIme.Text;
            k.faks = textBoxFakultet.Text;
            k.brtelefona = textBoxTelefon.Text;
            k.email = textBoxEmail.Text;
            k.stanje = 0;
            k.Uplata = 0;
            if (lista.Exists(p=>p.brkartice==k.brkartice))
            {
                MessageBox.Show($"Kartica sa brojem >{textBoxBrojKartice.Text}< vec postoji!", "Obavestenje!");
                textBoxBrojKartice.Clear();
                textBoxBrojKartice.Focus();
                return;
            }


            bool rez = kDal.UnesiKarticu(k);
            if (rez == true)
            {
                MessageBox.Show("Ubacen novi korisnik!");
                PrikaziKorisnike();
                DozvoliPromenu(false);
                dataGrid1.Focus();
                indeks = dataGrid1.Items.Count - 1;
                dataGrid1.SelectedIndex = indeks;
                dataGrid1.ScrollIntoView(dataGrid1.Items[indeks]);                
            }
        }                   //metoda ubaci
        private void Promeni()
        {
            indeks = dataGrid1.SelectedIndex;
            if (indeks < 0)
            {
                return;
            }
            if (!Validacija())
            {
                return;
            }
            MessageBoxResult rezultat = MessageBox.Show("Da li zelite da izvrsite promenu?", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rezultat == MessageBoxResult.Yes)
            {
                kartice_tab k = (kartice_tab)dataGrid1.SelectedItem;
                k.KarticaId = int.Parse(textBoxId.Text);
                k.brkartice = int.Parse(textBoxBrojKartice.Text);
                k.korisnik = textBoxIme.Text;
                k.faks = textBoxFakultet.Text;
                k.brtelefona = textBoxTelefon.Text;
                k.email = textBoxEmail.Text;
                k.stanje = double.Parse(textBoxStanje.Text);
                bool rez = kDal.IzmeniKarticu(k);
                if (rez == true)
                {
                    MessageBox.Show("Podaci promenjeni!", "Obavestenje");
                    PrikaziKorisnike();
                    DozvoliPromenu(false);
                    dataGrid1.Focus();
                    dataGrid1.SelectedIndex = indeks;
                    dataGrid1.ScrollIntoView(dataGrid1.Items[indeks]);                    
                }
                else
                {
                    MessageBox.Show("Greska pri promeni podataka!", "Greska!");
                }
            }
        }                //metoda promeni
        private void Obrisi()
        {
            indeks = dataGrid1.SelectedIndex;
            if (indeks < 0)
            {
                return;
            }
            MessageBoxResult r = MessageBox.Show("Da li zelite da obrisete korisnika iz baze?", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                MessageBoxResult r1 = MessageBox.Show("Da li ste sigurni?", "Potvrda!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (r1 == MessageBoxResult.Yes)
                {
                    kartice_tab k = (kartice_tab)dataGrid1.SelectedItem;
                    bool rez = kDal.ObrisiKarticu(k);
                    if (rez == true)
                    {
                        PrikaziKorisnike();
                        Resetuj();
                        MessageBox.Show("Korisnik uspesno obrisan!");
                    }
                }
            }
        }              
        private void IzvrsiUplatu()
        {
            indeks = dataGrid1.SelectedIndex;
          
            if (indeks < 0)
            {
                return;
            }
            if (!Validacija())
            {
                return;
            }
            
            kartice_tab k = (kartice_tab)dataGrid1.SelectedItem;
            k.KarticaId = int.Parse(textBoxId.Text);
            k.brkartice = int.Parse(textBoxBrojKartice.Text);
            k.korisnik = textBoxIme.Text;
            k.faks = textBoxFakultet.Text;
            k.brtelefona = textBoxTelefon.Text;
            k.email = textBoxEmail.Text;
            k.Uplata = double.Parse(textBoxUplata.Text);
            double s1 = double.Parse(textBoxStanje.Text) + double.Parse(textBoxUplata.Text);
            
            k.stanje = s1;
            
            if (k.stanje >= 3000 && k.stanje < 6000)
            {
                k.Uplata = double.Parse(textBoxUplata.Text) - 3000;
                k.stanje = s1;
                textBoxStanje.Text = k.stanje.ToString();
                MessageBox.Show($"Ispunjen uslov za vaucer sa {k.stanje} din.\nPreostali kredit je {k.stanje - 3000} din.\nNakon izdavanja vaucera zatvorite dijalog!", "Poruka", MessageBoxButton.OK, MessageBoxImage.Information);
                k.stanje = k.stanje - 3000;
            }
            if (k.stanje >= 6000 && k.stanje < 9000)
            {
                k.Uplata = double.Parse(textBoxUplata.Text) - 6000;
                k.stanje = s1;
                textBoxStanje.Text = k.stanje.ToString();
                MessageBox.Show($"Ispunjen uslov za 2 vaucera sa {k.stanje} din.\nPreostali kredit je {k.stanje - 6000} din.\nNakon izdavanja vaucera zatvorite dijalog!", "Poruka", MessageBoxButton.OK, MessageBoxImage.Information);
                k.stanje = k.stanje - 6000;
            }
            if (k.stanje >= 9000 && k.stanje < 12000)
            {
                k.Uplata = double.Parse(textBoxUplata.Text) - 9000;
                k.stanje = s1;
                textBoxStanje.Text = k.stanje.ToString();
                MessageBox.Show($"Ispunjen uslov za 3 vaucera sa {k.stanje} din.\nPreostali kredit je {k.stanje - 9000} din.\nNakon izdavanja vaucera zatvorite dijalog!", "Poruka", MessageBoxButton.OK, MessageBoxImage.Information);
                k.stanje = k.stanje - 9000;
            }
            if (k.stanje >= 12000 && k.stanje < 15000)
            {
                k.Uplata = double.Parse(textBoxUplata.Text) - 12000;
                k.stanje = s1;
                textBoxStanje.Text = k.stanje.ToString();
                MessageBox.Show($"Ispunjen uslov za 4 vaucera sa {k.stanje} din.\nPreostali kredit je {k.stanje - 12000} din.\nNakon izdavanja vaucera zatvorite dijalog!", "Poruka", MessageBoxButton.OK, MessageBoxImage.Information);
                k.stanje = k.stanje - 12000;
            }
            if (k.stanje >= 15000)
            {
                k.Uplata = double.Parse(textBoxUplata.Text) - 15000;
                k.stanje = s1;
                textBoxStanje.Text = k.stanje.ToString();
                MessageBox.Show($"Ispunjen uslov za 5 vaucera sa {k.stanje} din.\nPreostali kredit je {k.stanje - 15000} din.\nNakon izdavanja vaucera zatvorite dijalog!", "Poruka", MessageBoxButton.OK, MessageBoxImage.Information);
                k.stanje = k.stanje - 15000;
            }            

            bool rez = kDal.IzvrsiUplatu(k);
            if (rez==true)
            {
                textBoxPretragaPoImenu.Clear();
                PrikaziKorisnike();
                dataGrid1.Focus();
                dataGrid1.SelectedIndex = indeks;
                dataGrid1.ScrollIntoView(dataGrid1.Items[indeks]);
                textBoxStanje.Text = k.stanje.ToString();
                dataGrid1.Items.Refresh();
                MessageBox.Show("Uplata izvrsena", "Poruka");                
            }

        }
        private void buttonResetuj_Click(object sender, RoutedEventArgs e)
        {
            Resetuj();
        }                
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {                        
            PrikaziKorisnike();
            textBoxBrojKartice.Focus();
            DozvoliPromenu(false);            
        }       
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if (dataGrid1.SelectedIndex >-1)
            {
                kartice_tab k = (kartice_tab)dataGrid1.SelectedItem;
                textBoxId.Text = k.KarticaId.ToString();
                textBoxBrojKartice.Text = k.brkartice.ToString();
                textBoxIme.Text = k.korisnik;
                textBoxFakultet.Text = k.faks;
                textBoxTelefon.Text = k.brtelefona;
                textBoxEmail.Text = k.email;
                textBoxStanje.Text = (k.stanje.HasValue ? (double?)Math.Round(k.stanje.Value, 2) : null).ToString();
                indeks = dataGrid1.SelectedIndex;
                textBoxPretragaPoImenu.Clear();
                dataGrid1.Focus();
                dataGrid1.ScrollIntoView(dataGrid1.SelectedItem);
                textBoxPretragaPoImenu.Text = "";
                DozvoliPromenu(false);
            }
        }
        private void noviKorisnik_Click(object sender, RoutedEventArgs e)
        {
            Resetuj();
            DozvoliPromenu(true);
            operacija = 1;
            buttonSacuvaj.Content = "Ubaci";
        }
        private void promeniKorisnika_Click(object sender, RoutedEventArgs e)
        {
            if (indeks > -1)
            {
                DozvoliPromenu(true);                
                operacija = 0;
                buttonSacuvaj.Content = "Sacuvaj";
            }
            else
            {
                MessageBox.Show("Odaberi korisnika");
            }
        }
        private void obrisiKorisnika_Click(object sender, RoutedEventArgs e)
        {
            int indeks = dataGrid1.SelectedIndex;
            if (indeks < 0)
            {
                MessageBox.Show("Niste odabrali korisnika","Obavestenje",MessageBoxButton.OK,MessageBoxImage.Asterisk);
                return;
            }
            MessageBoxResult r = MessageBox.Show("Da li zelite da obrisete korisnika iz baze?", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                MessageBoxResult r1 = MessageBox.Show("Da li ste sigurni?", "Potvrda!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (r1 == MessageBoxResult.Yes)
                {
                    kartice_tab k = (kartice_tab)dataGrid1.SelectedItem;
                    bool rez = kDal.ObrisiKarticu(k);
                    if (rez == true)
                    {
                        PrikaziKorisnike();
                        Resetuj();
                        MessageBox.Show("Korisnik uspesno obrisan!");
                    }
                }
            }
        }
        private void zatvoriAplikaciju_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult rez = MessageBox.Show("Da li zelite da izadjete?\nPromene koje ste izvrsili nece biti sacuvane.", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rez==MessageBoxResult.Yes)
            {
                Close();
            }
            
        }
        private void buttonOdustani_Click(object sender, RoutedEventArgs e)
        {
            Resetuj();
            DozvoliPromenu(false);
            buttonSacuvaj.Content = "Sacuvaj";
        }
        private void buttonSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (operacija==1)
            {                
                Ubaci();                
            }
            if (operacija==0)
            {
                Promeni();
            }
        }
        private void textBoxBrojKartice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                try
                {
                    kartice_tab k = lista.First(p => p.brkartice == int.Parse(textBoxBrojKartice.Text));

                    if (k != null)
                    {
                        textBoxId.Text = k.KarticaId.ToString();
                        textBoxBrojKartice.Text = k.brkartice.ToString();
                        textBoxIme.Text = k.korisnik;
                        textBoxFakultet.Text = k.faks;
                        textBoxTelefon.Text = k.brtelefona;
                        textBoxEmail.Text = k.email;
                        textBoxStanje.Text = k.stanje.ToString();

                        textBoxPretragaPoImenu.Clear();
                        dataGrid1.SelectedItem = PrikaziKorisnika(k.KarticaId);
                        dataGrid1.ScrollIntoView(k);
                        dataGrid1.SelectedItem = k;
                    }
                    
                }
                catch (Exception)
                {
                    MessageBox.Show($"Ne postoji korisnik sa karticom br. >{textBoxBrojKartice.Text}<!", "Obavestenje!");
                    textBoxBrojKartice.Clear();
                    return;
                }

                textBoxUplata.Focus();
            }
        }        
        private void buttonUplati_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                IzvrsiUplatu();
                textBoxUplata.Clear();                
            }
            catch (Exception)
            {
                MessageBox.Show("Nije ispavna uplata!", "Greska",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
        }
        private void textBoxUplata_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    IzvrsiUplatu();
                    textBoxUplata.Clear();
                }
                catch (Exception)
                {
                    MessageBox.Show("Nije ispavna uplata!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        private void maslinastoZelena_Click(object sender, RoutedEventArgs e)
        {
            glavniGrid.Background = Brushes.Olive;
            groupBox.Background = Brushes.DarkOliveGreen;
            groupBox.Foreground = Brushes.LightGray;
            groupBox.BorderBrush = Brushes.DarkGreen;
            border1.Background = Brushes.DarkOliveGreen;
            border1.BorderBrush = Brushes.DarkGreen;            
            textBlock1BK.Foreground = Brushes.LightGreen;
            textBlock1EM.Foreground = Brushes.LightGreen;
            textBlock1FAK.Foreground = Brushes.LightGreen;
            textBlock1ID.Foreground = Brushes.LightGreen;
            textBlock1Ime.Foreground = Brushes.LightGreen;
            textBlock1ST.Foreground = Brushes.LightGreen;
            textBlock1TEL.Foreground = Brushes.LightGreen;
            textBlock1UP.Foreground = Brushes.LightGreen;
            
            buttonUplati.Background = Brushes.Olive;
            buttonResetuj.Background = Brushes.Olive;
            buttonUplati.Foreground = Brushes.LightGreen;
            buttonResetuj.Foreground = Brushes.LightGreen;

            buttonResetuj.BorderBrush = Brushes.LightGreen;
            buttonUplati.BorderBrush = Brushes.LightGreen;
        }
        private void tamnoSiva_Click(object sender, RoutedEventArgs e)
        {
            glavniGrid.Background = Brushes.Gray;
            groupBox.Background = Brushes.DarkGray;
            groupBox.Foreground = Brushes.LightGray;
            groupBox.BorderBrush = Brushes.DimGray;
            border1.Background = Brushes.DarkGray;
            border1.BorderBrush = Brushes.Gray;
            textBlock1BK.Foreground = Brushes.White;
            textBlock1EM.Foreground = Brushes.White;
            textBlock1FAK.Foreground = Brushes.White;
            textBlock1ID.Foreground = Brushes.White;
            textBlock1Ime.Foreground = Brushes.White;
            textBlock1ST.Foreground = Brushes.White;
            textBlock1TEL.Foreground = Brushes.White;
            textBlock1UP.Foreground = Brushes.White;
            
            buttonUplati.Background = Brushes.DarkGray;
            buttonResetuj.Background = Brushes.DarkGray;
            buttonUplati.Foreground = Brushes.Black;
            buttonResetuj.Foreground = Brushes.Black;

            buttonResetuj.BorderBrush = Brushes.WhiteSmoke;
            buttonUplati.BorderBrush = Brushes.WhiteSmoke;
        }
        private void oker_Click(object sender, RoutedEventArgs e)
        {
            glavniGrid.Background = Brushes.Beige;
        }
        private void narandzasta_Click(object sender, RoutedEventArgs e)
        {
            glavniGrid.Background = Brushes.Orange;
        }
        private void crna_Click(object sender, RoutedEventArgs e)
        {
            glavniGrid.Background = Brushes.Black;

            groupBox.Background = Brushes.Black;
            groupBox.Foreground = Brushes.Gray;
            groupBox.BorderBrush = Brushes.Black;
            border1.Background = Brushes.Black;
            border1.BorderBrush = Brushes.Black;
            textBlock1BK.Foreground = Brushes.WhiteSmoke;
            textBlock1EM.Foreground = Brushes.WhiteSmoke;
            textBlock1FAK.Foreground = Brushes.WhiteSmoke;
            textBlock1ID.Foreground = Brushes.WhiteSmoke;
            textBlock1Ime.Foreground = Brushes.WhiteSmoke;
            textBlock1ST.Foreground = Brushes.WhiteSmoke;
            textBlock1TEL.Foreground = Brushes.WhiteSmoke;
            textBlock1UP.Foreground = Brushes.WhiteSmoke;
            
            buttonUplati.Background = Brushes.Black;
            buttonResetuj.Background = Brushes.Black;
            buttonUplati.Foreground = Brushes.WhiteSmoke;
            buttonResetuj.Foreground = Brushes.White;

            buttonResetuj.BorderBrush = Brushes.WhiteSmoke;
            buttonUplati.BorderBrush = Brushes.WhiteSmoke;
        }
        private void bela_Click(object sender, RoutedEventArgs e)
        {
            glavniGrid.Background = Brushes.White;

            groupBox.Background = Brushes.White;
            groupBox.Foreground = Brushes.White;
            groupBox.BorderBrush = Brushes.White;
            border1.Background = Brushes.White;
            border1.BorderBrush = Brushes.White;
            textBlock1BK.Foreground = Brushes.DimGray;
            textBlock1EM.Foreground = Brushes.DimGray;
            textBlock1FAK.Foreground = Brushes.DimGray;
            textBlock1ID.Foreground = Brushes.DimGray;
            textBlock1Ime.Foreground = Brushes.DimGray;
            textBlock1ST.Foreground = Brushes.DimGray;
            textBlock1TEL.Foreground = Brushes.DimGray;
            textBlock1UP.Foreground = Brushes.DimGray;
       
            buttonUplati.Background = Brushes.White;            
            buttonResetuj.Background = Brushes.White;
            buttonUplati.Foreground = Brushes.Gray;
            buttonResetuj.Foreground = Brushes.Gray;

            buttonResetuj.BorderBrush = Brushes.WhiteSmoke;
            buttonUplati.BorderBrush = Brushes.WhiteSmoke;
        }               
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            byte a = 235;
            byte b = 235;
            byte c = 235;
            SolidColorBrush defSiva = new SolidColorBrush(Color.FromRgb(a, b, c));


            glavniGrid.Background = defSiva;

            groupBox.Background = Brushes.Gainsboro;
            groupBox.Foreground = Brushes.DarkSlateGray;
            groupBox.BorderBrush = Brushes.Gray;
            border1.Background = Brushes.Gainsboro;
            border1.BorderBrush = Brushes.Gray;
            textBlock1BK.Foreground = Brushes.Purple;
            textBlock1EM.Foreground = Brushes.Purple;
            textBlock1FAK.Foreground = Brushes.Purple;
            textBlock1ID.Foreground = Brushes.Purple;
            textBlock1Ime.Foreground = Brushes.Purple;
            textBlock1ST.Foreground = Brushes.Purple;
            textBlock1TEL.Foreground = Brushes.Purple;
            textBlock1UP.Foreground = Brushes.Purple;
            
            buttonUplati.Background = Brushes.LightGray;
            buttonResetuj.Background = Brushes.LightGray;
            buttonUplati.Foreground = Brushes.Black;
            buttonResetuj.Foreground = Brushes.Black;

            buttonResetuj.BorderBrush = Brushes.Black;
            buttonUplati.BorderBrush = Brushes.Black;
        }        
        private void najvise_Click(object sender, RoutedEventArgs e)
        {

        }
        private void textBoxPretragaPoImenu_TextChanged(object sender, TextChangedEventArgs e)
        {
            string pretraga = textBoxPretragaPoImenu.Text.ToLower();

            if (pretraga == "")
            {
                dataGrid1.ItemsSource = lista;
                return;
            }
            dataGrid1.ItemsSource = lista.Where(p => p.korisnik.ToLower().Contains(pretraga));
            dataGrid1.Items.Refresh();
        }
        private void buttonNoviKorisnik_Click(object sender, RoutedEventArgs e)
        {
            Resetuj();
            DozvoliPromenu(true);
            operacija = 1;
            buttonSacuvaj.Content = "Ubaci";
        }
        private void buttonObrisiKorisnika_Click(object sender, RoutedEventArgs e)
        {
            int indeks = dataGrid1.SelectedIndex;
            if (indeks < 0)
            {
                MessageBox.Show("Niste odabrali korisnika", "Obavestenje", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
            MessageBoxResult r = MessageBox.Show("Da li zelite da obrisete korisnika iz baze?", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                MessageBoxResult r1 = MessageBox.Show("Da li ste sigurni?", "Potvrda!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (r1 == MessageBoxResult.Yes)
                {
                    kartice_tab k = (kartice_tab)dataGrid1.SelectedItem;
                    bool rez = kDal.ObrisiKarticu(k);
                    if (rez == true)
                    {
                        PrikaziKorisnike();
                        Resetuj();
                        MessageBox.Show("Korisnik uspesno obrisan!");
                    }
                }
            }
        }
        private void buttonPromeniKorisnika_Click(object sender, RoutedEventArgs e)
        {
            if (indeks > -1)
            {
                DozvoliPromenu(true);
                operacija = 0;
                buttonSacuvaj.Content = "Sacuvaj";
            }
            else
            {
                MessageBox.Show("Odaberi korisnika");
            }
        }        
    }
}
