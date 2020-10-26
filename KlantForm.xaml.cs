using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TussentijdsProject
{
    /// <summary>
    /// Interaction logic for KlantForm.xaml
    /// </summary>
    public partial class KlantForm : Window
    {
        int klantID;
        bool editable;
        public KlantForm(int klantID, bool editable = false)
        {
            this.klantID = klantID;
            this.editable = editable;
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (InputCorrect().Count <= 0)
            {
                using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
                {
                    var query = ctx.Klants.Where(s => s.KlantID == klantID).Select(s => s).FirstOrDefault();
                    query.Voornaam = txtVoornaam.Text;
                    query.Achternaam = txtAchternaam.Text;
                    query.Straatnaam = txtStraat.Text;
                    query.Huisnummer = txtnr.Text;
                    try { query.Bus = txtnr.Text; } catch (Exception) {query.Bus = null; }
                    query.Postcode = int.Parse(txtnr.Text);
                    query.Gemeente = txtGemeente.Text;
                    query.Telefoonnummer = txtTelephoon.Text;
                    query.Emailadres = txtMail.Text;
                    query.Opmerking = txtOpmerking.Text;

                    ctx.SaveChanges();
                }
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(String.Join(Environment.NewLine, InputCorrect()));
            }
        }

        private List<string> InputCorrect()
        {
            List<string> answer = new List<string>();

            if (txtVoornaam.Text.Length <= 0)
            {
                epVoorNaam.Visibility = Visibility.Visible;
                answer.Add("geef een voornaam in");
            }
            else
            {
                epVoorNaam.Visibility = Visibility.Hidden;
            }


            if (txtAchternaam.Text.Length <= 0)
            {
                epAchternaam.Visibility = Visibility.Visible;
                answer.Add("geef een Achternaam in");
            }
            else
            {
                epAchternaam.Visibility = Visibility.Hidden;
            }

            if (txtStraat.Text.Length <= 0)
            {
                epStraat.Visibility = Visibility.Visible;
                answer.Add("geef een Straatnaam in");
            }
            else
            {
                epStraat.Visibility = Visibility.Hidden;
            }

            if (txtnr.Text.Length <= 0)
            {
                epStraat.Visibility = Visibility.Visible;
                answer.Add("geef een straatnummer in");
            }
            else
            {
                epStraat.Visibility = Visibility.Hidden;
            }


            try
            {
                int.Parse(txtPostcode.Text);
                epPostcode.Visibility = Visibility.Hidden;
                if (txtPostcode.Text.Length != 4)
                {
                    epPostcode.Visibility = Visibility.Visible;
                    answer.Add("Geef een geldig 4 geldige voor postcode");
                }
            }
            catch (Exception)
            {
                epPostcode.Visibility = Visibility.Visible;
                answer.Add("Geef een geldig numerical postcode in");
            }

            if (txtGemeente.Text.Length <= 0)
            {
                epGemeente.Visibility = Visibility.Visible;
                answer.Add("geef een Gemeente in");
            }
            else
            {
                epGemeente.Visibility = Visibility.Hidden;
            }

            if (txtTelephoon.Text.Length != 10)
            {
                epTelephoon.Visibility = Visibility.Visible;
                answer.Add("geef een geldig telefoonnummer in");
            }
            else
            {
                epTelephoon.Visibility = Visibility.Hidden;
            }

            if (txtMail.Text.Length <= 0)
            {
                epMail.Visibility = Visibility.Visible;
                answer.Add("geef een Email in");
            }
            else
            {
                epMail.Visibility = Visibility.Hidden;
            }


            return answer;
        }


        private void textfield_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputCorrect();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                Klant klant = ctx.Klants.Where(s => s.KlantID == klantID).FirstOrDefault();
                lblId.Text = klant.KlantID.ToString();
                txtVoornaam.Text = klant.Voornaam;
                txtAchternaam.Text = klant.Achternaam;
                txtStraat.Text = klant.Straatnaam;
                txtnr.Text = klant.Huisnummer;
                txtBus.Text = klant.Bus;
                txtPostcode.Text = klant.Postcode.ToString();
                txtGemeente.Text = klant.Gemeente;
                txtTelephoon.Text = klant.Telefoonnummer;
                txtMail.Text = klant.Emailadres;
                txtOpmerking.Text = klant.Opmerking;
            }

            txtVoornaam.IsEnabled =
            txtAchternaam.IsEnabled =
            txtStraat.IsEnabled =
            txtnr.IsEnabled =
            txtBus.IsEnabled =
            txtPostcode.IsEnabled =
            txtGemeente.IsEnabled =
            txtTelephoon.IsEnabled =
            txtMail.IsEnabled =
            txtOpmerking.IsEnabled = editable;
            InputCorrect();
        }
    }
}
