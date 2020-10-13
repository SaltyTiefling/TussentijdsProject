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
            using (tussentijds_projectEntities ctx = new tussentijds_projectEntities())
            {
                cbLogin.ItemsSource = ctx.Personeelslids.Select(s => s).ToList();
                cbLogin.SelectedIndex = 0;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities ctx = new tussentijds_projectEntities())
            {
                string encrypted = Encrytion.Encrypt((cbLogin.SelectedItem as Personeelslid).Voornaam, txtWachtwoord.Text);
                MessageBox.Show(encrypted);
                ctx.Logins.Add(new Login(){PersoneelslidID = (cbLogin.SelectedItem as Personeelslid).PersoneelslidID, Wachtwoord = encrypted});
                ctx.SaveChanges();
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
