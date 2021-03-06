﻿using System;
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
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                cbLogin.ItemsSource = ctx.Personeelslids.Select(s => s).ToList();
                cbLogin.SelectedIndex = 0;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Personeelslid geselecteerdPersoon = (cbLogin.SelectedItem as Personeelslid);
            string encrypted = Encrytion.Encrypt(geselecteerdPersoon.Voornaam, txtWachtwoord.Text);
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                List<string> wachtwoordenInTable = null;
                wachtwoordenInTable = ctx.Logins.Where(s => s.PersoneelslidID == geselecteerdPersoon.PersoneelslidID).Select(s => s.Wachtwoord).ToList();
                if (wachtwoordenInTable.Contains(encrypted))
                {
                    this.Hide();
                    Databeheer databeheer = new Databeheer(geselecteerdPersoon);
                    databeheer.ShowDialog();
                    this.Show();
                }
                else
                {
                    MessageBox.Show($"dit is niet het juiste wachtwoord voor {geselecteerdPersoon.Voornaam}");
                }
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

        private void cbLogin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
