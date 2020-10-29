using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Graph;

namespace TussentijdsProject
{
    /// <summary>
    /// Interaction logic for Databeheer.xaml
    /// </summary>
    public partial class Databeheer : Window
    {
        public Personeelslid currentUser { get; } = new Personeelslid();
        int selectedID = 0;
        public Databeheer(Personeelslid user)
        {
            currentUser = user;
            InitializeComponent();
            LaadLijsten();

        }
        private void LaadLijsten()
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
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
                    Adress = s.Straatnaam + " " + s.Huisnummer + " " + s.Bus + ", " + s.Postcode + " " + s.Gemeente,
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
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                System.Linq.IQueryable<string> userRolls = ctx.PersoneelslidRols.Where(s => s.PersoneelslidID == currentUser.PersoneelslidID).Select(s => s.Rol.RolNaam);
                Debug.WriteLine("rollen");

                btnBekijk.IsEnabled = true;

                if (userRolls.Contains("Magazijnier"))
                {
                    Console.WriteLine("je bent een Magazijnier");

                    switch ((tabs.SelectedValue as TabItem).Header.ToString())
                    {
                        case "Bestellingen":
                        case "Klanten":
                            btnDelete.IsEnabled =
                            btnNieuw.IsEnabled =
                            btnEdit.IsEnabled =
                            btnFileEdit.IsEnabled = false;
                            break;
                        case "Producten":
                            btnFileEdit.Visibility = Visibility.Visible;
                            break;
                        default:
                            btnDelete.IsEnabled =
                            btnNieuw.IsEnabled =
                            btnEdit.IsEnabled =
                            btnFileEdit.IsEnabled = true;
                            btnFileEdit.Visibility = Visibility.Collapsed;
                            break;
                    }

                }

                if (userRolls.Contains("Verkoper"))
                {
                    Console.WriteLine("je bent een Verkoper");

                    switch ((tabs.SelectedValue as TabItem).Header.ToString())
                    {
                        case "Klanten":
                            btnDelete.IsEnabled =
                            btnNieuw.IsEnabled =
                            btnEdit.IsEnabled = true;
                            break;
                        default:
                            btnDelete.IsEnabled =
                            btnNieuw.IsEnabled =
                            btnEdit.IsEnabled = false;
                            break;
                    }
                }

                if (userRolls.Contains("Administrator"))
                {
                    Console.WriteLine("je bent een admin");
                    if ((tabs.SelectedValue as TabItem).Header.ToString() == "Producten")
                    {
                        btnFileEdit.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnFileEdit.Visibility = Visibility.Collapsed;
                    }
                    btnDelete.IsEnabled =
                    btnNieuw.IsEnabled =
                    btnEdit.IsEnabled = true;

                }
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {

                switch ((tabs.SelectedValue as TabItem).Header.ToString())
                {
                    case "Producten":
                        Product product = new Product();
                        ctx.Products.Add(product);
                        ctx.SaveChanges();
                        productForm pf = new productForm(product.ProductID, true);
                        if (pf.ShowDialog() != true)
                        {
                            ctx.Products.Remove(product);
                            ctx.SaveChanges();
                        }
                        break;
                    case "Klanten":
                        Klant klant = new Klant();
                        klant.AangemaaktOp = DateTime.Now;
                        ctx.Klants.Add(klant);
                        ctx.SaveChanges();
                        KlantForm kf = new KlantForm(klant.KlantID, true);
                        if (kf.ShowDialog() != true)
                        {
                            ctx.Klants.Remove(klant);
                            ctx.SaveChanges();
                        }
                        break;
                    case "Categorien":
                        Categorie categorie = new Categorie();
                        ctx.Categories.Add(categorie);
                        ctx.SaveChanges();
                        CategorienForm cf = new CategorienForm(categorie.CategorieID, true);
                        if (cf.ShowDialog() != true)
                        {
                            ctx.Categories.Remove(categorie);
                            ctx.SaveChanges();
                        }
                        break;
                    case "Leveranciers":
                        Leverancier leverancier = new Leverancier();
                        ctx.Leveranciers.Add(leverancier);
                        ctx.SaveChanges();
                        LeverancierForm lf = new LeverancierForm(selectedID, true);
                        if (lf.ShowDialog() != true)
                        {
                            ctx.Leveranciers.Remove(leverancier);
                            ctx.SaveChanges();
                        }
                        break;
                    case "Personeel":
                        Personeelslid personeelslid = new Personeelslid();
                        ctx.Personeelslids.Add(personeelslid);
                        ctx.SaveChanges();
                        PersoneelsForm pef = new PersoneelsForm(selectedID, true);
                        if (pef.ShowDialog() != true)
                        {
                            ctx.Personeelslids.Remove(personeelslid);
                            ctx.SaveChanges();
                        }
                        break;
                    case "Bestellingen":
                        nieuweBestellingMessage nbm = new nieuweBestellingMessage(currentUser);
                        nbm.ShowDialog();
                        break;
                    default:
                        MessageBox.Show("er is iets mis gegaan selecteer een andere databank aub");
                        break;
                }

                LaadLijsten();
            }
        }
        private void Bekijk_Click(object sender, RoutedEventArgs e)
        {
            OpenForm(false);

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            OpenForm(true);
        }
        private void OpenForm(bool editable)
        {
            if (selectedID > 0)
            {
                switch (((tabs.SelectedValue as TabItem).Header.ToString()))
                {
                    case "Producten":
                        productForm pf = new productForm(selectedID, editable);
                        pf.ShowDialog();
                        break;
                    case "Klanten":
                        KlantForm kf = new KlantForm(selectedID, editable);
                        kf.ShowDialog();
                        break;
                    case "Categorien":
                        CategorienForm cf = new CategorienForm(selectedID, editable);
                        cf.ShowDialog();
                        break;
                    case "Leveranciers":
                        LeverancierForm lf = new LeverancierForm(selectedID, editable);
                        lf.ShowDialog();
                        break;
                    case "Personeel":
                        PersoneelsForm pef = new PersoneelsForm(selectedID, editable);
                        pef.ShowDialog();
                        break;
                    case "Bestellingen":
                        bool isleverancier = false;
                        using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
                        {
                            Bestelling querry = ctx.Bestellings.Where(s => s.BestellingID == selectedID).FirstOrDefault();

                            if (querry.Leverancier == null)
                            {
                                isleverancier = false;
                            }
                            else
                            {
                                isleverancier = true;
                            }
                        }

                        BestelForm bf = new BestelForm(selectedID, isleverancier, editable);
                        bf.ShowDialog();
                        break;
                    default:
                        MessageBox.Show("er is iets mis gegaan selecteer een andere databank aub");
                        break;
                }
            }
            else
            {
                MessageBox.Show($"selecteer eerst een rij uit {(tabs.SelectedValue as TabItem).Header}");
            }
            LaadLijsten();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedID > 0)
            {
                using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
                {
                    switch (((tabs.SelectedValue as TabItem).Header.ToString()))
                    {
                        case "Producten":
                            Product product = ctx.Products.Select(s => s).Where(s => s.ProductID == selectedID).FirstOrDefault();
                            ctx.Products.Remove(product);
                            break;
                        case "Klanten":
                            Klant klant = ctx.Klants.Select(s => s).Where(s => s.KlantID == selectedID).FirstOrDefault();

                            foreach (Bestelling item in klant.Bestellings)
                            {
                                item.KlantID = null;
                            }

                            ctx.Klants.Remove(klant);
                            break;
                        case "Categorien":
                            Categorie categorie = ctx.Categories.Where(s => s.CategorieID == selectedID).FirstOrDefault();
                            foreach (Product product1 in categorie.Products)
                            {
                                product1.CategorieID = null;
                            }
                            ctx.Categories.Remove(categorie);
                            break;
                        case "Leveranciers":
                            Leverancier leverancier = ctx.Leveranciers.Where(s => s.LeverancierID == selectedID).FirstOrDefault();

                            foreach (Product product1 in leverancier.Products)
                            {
                                product1.LeverancierID = null;
                            }

                            foreach (Bestelling item in leverancier.Bestellings)
                            {
                                item.LeverancierID = null;
                            }

                            ctx.Leveranciers.Remove(leverancier);
                            break;
                        case "Personeel":
                            Personeelslid personeelslid = ctx.Personeelslids.Where(s => s.PersoneelslidID == selectedID).FirstOrDefault();

                            if (personeelslid == currentUser)
                            {
                                MessageBox.Show("je kan jezelf niet verwijderen");
                            }
                            else
                            {
                                foreach (Bestelling item in personeelslid.Bestellings)
                                {
                                    item.PersoneelslidID = null;
                                }

                                ctx.Logins.RemoveRange(personeelslid.Logins);
                                ctx.PersoneelslidRols.RemoveRange(personeelslid.PersoneelslidRols);
                                ctx.Personeelslids.Remove(personeelslid);
                                this.Close();
                            }

                            break;
                        case "Bestellingen":
                            Bestelling bestelling = ctx.Bestellings.Where(s => s.BestellingID == selectedID).FirstOrDefault();
                            ctx.BestellingProducts.RemoveRange(bestelling.BestellingProducts);
                            ctx.Bestellings.Remove(bestelling);
                            break;
                        default:
                            MessageBox.Show("er is iets mis gegaan selecteer een andere databank aub");
                            break;

                    }
                    ctx.SaveChanges();
                }
            }
            else
            {
                MessageBox.Show($"selecteer eerst een rij uit {(tabs.SelectedValue as TabItem).Header}");
            }
            LaadLijsten();
        }

        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedID = 0;
            lblSelectedID.Text = selectedID.ToString();

            Window_Loaded(sender, e);

        }

        private void Productenlijst_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                try
                {
                    selectedID = (ctx.Products.ToList()[Productenlijst.SelectedIndex] as Product).ProductID;
                }
                catch (Exception)
                {
                    selectedID = 0;
                }
                lblSelectedID.Text = selectedID.ToString();
            }
        }

        private void Klantenlijst_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                try
                {
                    selectedID = (ctx.Klants.ToList()[Klantenlijst.SelectedIndex] as Klant).KlantID;
                }
                catch (Exception)
                {
                    selectedID = 0;
                }
                lblSelectedID.Text = selectedID.ToString();
            }
        }

        private void Categorienlijst_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                try
                {
                    selectedID = (ctx.Categories.ToList()[Categorienlijst.SelectedIndex] as Categorie).CategorieID;
                }
                catch (Exception)
                {
                    selectedID = 0;
                }
                lblSelectedID.Text = selectedID.ToString();
            }
        }

        private void Leverancierslijst_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                try
                {
                    selectedID = (ctx.Leveranciers.ToList()[Leverancierslijst.SelectedIndex] as Leverancier).LeverancierID;
                }
                catch (Exception)
                {
                    selectedID = 0;
                }
                lblSelectedID.Text = selectedID.ToString();
            }
        }

        private void Personeellijst_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                try
                {
                    selectedID = (ctx.Personeelslids.ToList()[Personeellijst.SelectedIndex] as Personeelslid).PersoneelslidID;
                }
                catch (Exception)
                {
                    selectedID = 0;
                }
                lblSelectedID.Text = selectedID.ToString();
            }
        }

        private void Bestellingenlijst_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                try
                {
                    selectedID = (ctx.Bestellings.ToList()[Bestellingenlijst.SelectedIndex] as Bestelling).BestellingID;
                }
                catch (Exception)
                {
                    selectedID = 0;
                }
                lblSelectedID.Text = selectedID.ToString();
            }
        }

        private void btnFileEdit_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Json files (*.json)|*.json";
            openFile.ShowDialog();

            try
            {
                using (StreamReader file = System.IO.File.OpenText(openFile.FileName))
                using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jOProduct = (JObject)JToken.ReadFrom(reader);
                    dynamic dyProduct = jOProduct;
                    selectedID = dyProduct.ProductID;

                    if (ctx.Products.Select(s => s.ProductID).ToList().Contains(selectedID))
                    {
                        Debug.WriteLine("Product bestaat al");

                        Product product = ctx.Products.Where(s => s.ProductID == selectedID).FirstOrDefault();
                        product.Naam = dyProduct.Naam;
                        product.Inkoopprijs = dyProduct.Inkoopprijs;
                        product.Marge = dyProduct.Marge;
                        product.Eenheid = dyProduct.Eenheid;
                        product.BTW = dyProduct.BTW;
                        product.LeverancierID = dyProduct.LeverancierID;
                        product.CategorieID = dyProduct.CategorieID;
                    }
                    else
                    {
                        Debug.WriteLine("nieuw product?");

                        Product newProduct = new Product();

                        newProduct.Naam = dyProduct.Naam;
                        newProduct.Inkoopprijs = dyProduct.Inkoopprijs;
                        newProduct.Marge = dyProduct.Marge;
                        newProduct.Eenheid = dyProduct.Eenheid;
                        newProduct.BTW = dyProduct.BTW;
                        newProduct.LeverancierID = dyProduct.LeverancierID;
                        newProduct.CategorieID = dyProduct.CategorieID;


                        if (MessageBox.Show($"het product met ID: {selectedID} bestaat nog niet wil {newProduct.Naam} toevoegen onder een nieuwe ID?", "invoegen product", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            ctx.Products.Add(newProduct);
                        }
                    }
                    ctx.SaveChanges();
                    LaadLijsten();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("er is iet's mis gegaan check je Json file en probeer opnieuw");
            }
        }

    }
}

