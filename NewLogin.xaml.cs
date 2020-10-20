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
    /// Interaction logic for NewLogin.xaml
    /// </summary>
    public partial class NewLogin : Window
    {
        public NewLogin()
        {
            InitializeComponent();
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                cbLogin.ItemsSource = ctx.Personeelslids.Select(s => s).ToList();
                cbLogin.SelectedIndex = 0;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                Personeelslid geselecteerdPersoon = (cbLogin.SelectedItem as Personeelslid);
                string encrypted = Encrytion.Encrypt(geselecteerdPersoon.Voornaam, txtWachtwoord.Text);

                var wachtwoordenInTable = ctx.Logins.Where(s => s.PersoneelslidID == geselecteerdPersoon.PersoneelslidID).Select(s => s.Wachtwoord);
                if (!wachtwoordenInTable.Contains(encrypted))
                {
                    if (MessageBox.Show("bent u zeker?", "zeker?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ctx.Logins.Add(new Login() { PersoneelslidID = geselecteerdPersoon.PersoneelslidID, Wachtwoord = encrypted });
                        ctx.SaveChanges();
                        MessageBox.Show("wachtwoord is opgeslagen");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("wachtwoord is niet opgeslagen");
                    }
                }
                else
                {
                    MessageBox.Show($"dit wachtwoord bestaat al voor {geselecteerdPersoon.Voornaam}");
                }

            }
            this.Close();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
