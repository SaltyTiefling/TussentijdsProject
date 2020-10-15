﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for Databeheer.xaml
    /// </summary>
    public partial class Databeheer : Window
    {
        Personeelslid currentUser = new Personeelslid();
        public Databeheer(Personeelslid user)
        {
            currentUser = user;
            InitializeComponent();

            using (tussentijds_projectEntities ctx = new tussentijds_projectEntities())
            {


                Productenlijst.ItemsSource = ctx.Products.Select(s => new
                {
                    s,
                    Inkoopprijs = "€" + Math.Round((double)s.Inkoopprijs, 2),
                    Marge = "€" + Math.Round((double)s.Marge, 2),
                    Eenheid = "€" + Math.Round((double)s.Eenheid, 2),
                    BTW = s.BTW + "%",
                    Leverancier = s.Leverancier.Contactpersoon,
                    Categorie = s.Categorie.CategorieNaam
                }).ToList();

                Bestellingenlijst.ItemsSource = ctx.Bestellings.Select(s => new
                {
                    s,
                    Verkoper = s.Personeelslid.Voornaam,
                    Leverancier = s.Leverancier.Contactpersoon,
                    Klant = s.Klant.Voornaam + " " + s.Klant.Achternaam
                }).ToList();

                Klantenlijst.ItemsSource = ctx.Klants.Select(s => new
                {
                    s,
                    Naam = s.Voornaam + " " + s.Achternaam,
                    Adress = s.Straatnaam + " " + s.Huisnummer + s.Bus + ", " + s.Postcode + " " + s.Gemeente
                }).ToList();

                Categorienlijst.ItemsSource = ctx.Categories.Select(s => s).ToList();
                Leverancierslijst.ItemsSource = ctx.Leveranciers.Select(s => new
                {
                    s,
                    Adress = s.Straatnaam + " " + s.Huisnummer + s.Bus + ", " + s.Postcode + " " + s.Gemeente,
                }).ToList();

                Personeellijst.ItemsSource = ctx.PersoneelslidRols.GroupBy(s => s.PersoneelslidID).Select(pr => new
                {
                    persoon = ctx.Personeelslids.Where(s => s.PersoneelslidID == pr.Key).FirstOrDefault(),
                    Rollen = pr.ToList()
                }).ToList();
            }
        }

        private string ListToString(List<string> list)
        {
            string answer = "";
            foreach (string item in list)
            {
                answer += item + ", ";
            }

            return answer.Substring(answer.Length - 2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities ctx = new tussentijds_projectEntities())
            {
                var userRolls = ctx.PersoneelslidRols.Where(s => s.PersoneelslidID == currentUser.PersoneelslidID).Select(s => s.Rol.RolNaam);
                Debug.WriteLine("rollen");
                if (userRolls.Contains("Administrator"))
                {
                    Console.WriteLine("je bent een admin");
                }

                if (userRolls.Contains("Magazijnier"))
                {
                    Console.WriteLine("je bent een Magazijnier");
                }

                if (userRolls.Contains("Verkoper"))
                {
                    Console.WriteLine("je bent een Verkoper");
                }

            }
        }
    }
}