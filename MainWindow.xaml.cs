using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TussentijdsProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
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
            string encrypted = Encrytion.Encrypt((cbLogin.SelectedItem as Personeelslid).Voornaam, txtWachtwoord.Text);

            using (tussentijds_projectEntities ctx = new tussentijds_projectEntities())
            {
                string wachtwoordInTable = ctx.Logins.Where(s => s.PersoneelslidID == 6).Select(s => s.Wachtwoord).FirstOrDefault();
                MessageBox.Show($"{wachtwoordInTable} == {encrypted} : {(wachtwoordInTable.Equals(encrypted))}");
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void NewLogin(object sender, RoutedEventArgs e)
        {
            NewLogin nl = new NewLogin();
            nl.ShowDialog();
        }
    }
}
