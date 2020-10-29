using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
    /// Interaction logic for nieuweBestellingMessage.xaml
    /// </summary>
    public partial class nieuweBestellingMessage : Window
    {
        int currentUser;
        public nieuweBestellingMessage(Personeelslid currentUser)
        {
            this.currentUser = currentUser.PersoneelslidID;
            InitializeComponent();
        }

        private void cbWie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            switch (cbWie.SelectedValue)
            {
                case "Leveranciers":
                    cbSchuldenaar.Visibility = txtLeverancier.Visibility = Visibility.Visible;
                    cbKlanten.Visibility = txtKlant.Visibility = Visibility.Collapsed;
                    break;
                case "Klanten":
                    cbSchuldenaar.Visibility = txtLeverancier.Visibility = Visibility.Collapsed;
                    cbKlanten.Visibility = txtKlant.Visibility = Visibility.Visible;
                    break;
                default:
                    this.Close();
                    break;
            }

        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                System.Linq.IQueryable<string> userRolls = ctx.PersoneelslidRols.Where(s => s.PersoneelslidID == currentUser).Select(s => s.Rol.RolNaam);

                cbKlanten.ItemsSource = ctx.Klants.ToList();
                cbKlanten.SelectedIndex = 0;
                cbSchuldenaar.ItemsSource = ctx.Leveranciers.ToList();
                cbSchuldenaar.SelectedIndex = 0;

                if (userRolls.Contains("Magazijnier"))
                {
                    Console.WriteLine("je bent een Magazijnier");
                    cbWie.Items.Add("Leveranciers");
                }

                if (userRolls.Contains("Verkoper"))
                {
                    cbWie.Items.Add("Klanten");

                }

                if (userRolls.Contains("Administrator"))
                {
                    while (!cbWie.Items.IsEmpty)
                    {
                        cbWie.Items.Remove(cbWie.Items[0]);
                    }
                    cbWie.ItemsSource = new List<string>() { "Leveranciers", "Klanten" };
                }
                cbWie.SelectedIndex = 0;
            }
        }

        private void btnOke_Click(object sender, RoutedEventArgs e)
        {
            Bestelling bestelling = new Bestelling();
            bool isleverancier = false;
            switch (cbWie.SelectedValue)
            {
                case "Leveranciers":
                    bestelling.Leverancier = (cbSchuldenaar.SelectedItem as Leverancier);
                    isleverancier = true;
                    break;
                case "Klanten":
                    bestelling.Klant = (cbKlanten.SelectedItem as Klant);
                    isleverancier = false;
                    break;
                default:
                    break;
            }
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                bestelling.DatumOpgemaakt = DateTime.Now;
                bestelling.PersoneelslidID = currentUser;
                ctx.Bestellings.Add(bestelling);

                ctx.SaveChanges();
                BestelForm bestel = new BestelForm(bestelling.BestellingID, isleverancier, true);
                bestel.ShowDialog();
            }
            this.Close();
        }
    }
}
