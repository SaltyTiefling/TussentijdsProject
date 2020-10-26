using System;
using System.Collections.Generic;
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
    /// Interaction logic for LeverancierForm.xaml
    /// </summary>
    public partial class LeverancierForm : Window
    {
        int leverancierID;
        bool editable;
        public LeverancierForm(int leverancierID, bool editable = false)
        {
            this.leverancierID = leverancierID;
            this.editable = editable;
            InitializeComponent();
            InputCorrect();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                Leverancier leverancier = ctx.Leveranciers.Where(s => s.LeverancierID == leverancierID).FirstOrDefault();

                lblId.Text = leverancier.LeverancierID.ToString();
                txtVoornaam.Text = leverancier.Contactpersoon;
                txtStraat.Text = leverancier.Straatnaam;
                txtnr.Text = leverancier.Huisnummer.ToString();
                txtBus.Text = leverancier.Bus;
                txtPostcode.Text = leverancier.Postcode.ToString();
                txtGemeente.Text = leverancier.Gemeente;
                txtTelephoon.Text = leverancier.Telefoonnummer;
                txtMail.Text = leverancier.Emailadres;
            }

            txtVoornaam.IsEnabled =
            txtStraat.IsEnabled =
            txtnr.IsEnabled =
            txtBus.IsEnabled =
            txtPostcode.IsEnabled =
            txtGemeente.IsEnabled =
            txtTelephoon.IsEnabled =
            txtMail.IsEnabled = editable;
            InputCorrect();
        }

        private void textfield_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputCorrect();
        }

        private List<string> InputCorrect()
        {
            List<string> answer = new List<string>();

            if (txtVoornaam.Text.Length <= 0)
            {
                epVoorNaam.Visibility = Visibility.Visible;
                answer.Add("geef een contactpersoon in");
            }
            else
            {
                epVoorNaam.Visibility = Visibility.Hidden;
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

            try
            {
                int.Parse(txtnr.Text);
                epStraat.Visibility = Visibility.Hidden;
               
            }
            catch (Exception)
            {
                epStraat.Visibility = Visibility.Visible;
                answer.Add("Geef een geldig huisnummer in");
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (InputCorrect().Count <= 0)
            {
                using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
                {
                    var query = ctx.Leveranciers.Where(s => s.LeverancierID == leverancierID).Select(s => s).FirstOrDefault();
                    query.Contactpersoon = txtVoornaam.Text;
                    query.Straatnaam = txtStraat.Text;
                    query.Huisnummer = int.Parse(txtnr.Text);
                    try { query.Bus = txtBus.Text; } catch (Exception) { query.Bus = null; }
                    query.Postcode = int.Parse(txtPostcode.Text);
                    query.Gemeente = txtGemeente.Text;
                    query.Telefoonnummer = txtTelephoon.Text;
                    query.Emailadres = txtMail.Text;

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
    }
}
