using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Xceed.Document.NET;
using Xceed.Words.NET;


namespace TussentijdsProject
{
    /// <summary>
    /// Interaction logic for BestelForm.xaml
    /// </summary>
    public partial class BestelForm : Window
    {
        int bestellingID;
        bool isLeverancier;
        bool editable;
        public BestelForm(int bestelingID, bool isLeverancier = false, bool editable = false)
        {
            this.bestellingID = bestelingID;
            this.editable = editable;
            this.isLeverancier = isLeverancier;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                Bestelling bestelling = ctx.Bestellings.Where(s => s.BestellingID == bestellingID).FirstOrDefault();

                txtBonId.Text = bestelling.BestellingID.ToString();
                txtDatum.Text = ((DateTime)bestelling.DatumOpgemaakt).ToString("dd-MMM-yyyy");

                var besteller = (isLeverancier) ? ctx.Leveranciers.Where(s => s.LeverancierID == bestelling.LeverancierID).Select(s => new
                {
                    nummer = s.LeverancierID,
                    naam = s.Contactpersoon,
                    adress = s.Straatnaam + " " + s.Huisnummer + " " + s.Bus,
                    gemeente = s.Postcode + ", " + s.Gemeente,
                    telefoon = s.Telefoonnummer,
                    email = s.Emailadres
                }).FirstOrDefault()
                    :
                    ctx.Klants.Where(s => s.KlantID == bestelling.KlantID).Select(s => new
                    {
                        nummer = s.KlantID,
                        naam = s.Voornaam + " " + s.Achternaam,
                        adress = s.Straatnaam + " " + s.Huisnummer + " " + s.Bus,
                        gemeente = s.Postcode + ", " + s.Gemeente,
                        telefoon = s.Telefoonnummer,
                        email = s.Emailadres
                    }).FirstOrDefault();

                if (isLeverancier) txtKlant.Text = "LeveranciersNr.";
                txtKlantnr.Text = besteller.nummer.ToString();
                txtNaam.Text = besteller.naam;
                txtAdress.Text = besteller.adress;
                txtGemeente.Text = besteller.gemeente;
                txtTelefoon.Text = besteller.telefoon.ToString();
                txtMail.Text = besteller.email;
                hlEmail.NavigateUri = new Uri($"mailto:{ besteller.email.ToString()}");

                spButtons.Visibility = (editable) ? Visibility.Visible : Visibility.Collapsed;

                dgProducts.ItemsSource = bestelling.BestellingProducts.Select(s => s.Product).Select(s => new
                {
                    s,
                    Beschrijving = s.Naam + " ( " + s.Leverancier.Contactpersoon + " )",
                    Prijs = "€ " + Math.Round((double)s.Eenheid, 2),
                    Btw = s.BTW + "%",
                    Netto = "€ " + Math.Round((double)(s.Eenheid + ((s.Eenheid / 100) * s.BTW)), 2)
                }).ToList();

                txtEenTotaal.Text = Math.Round((double)bestelling.BestellingProducts.Select(s => s.Product.Eenheid).Sum(), 2).ToString();
                txtBtwTotaal.Text = Math.Round((double)bestelling.BestellingProducts.Select(s => (s.Product.Eenheid / 100) * s.Product.BTW).Sum(), 2).ToString();
                txtTotaal.Text = Math.Round((double)bestelling.BestellingProducts.Select(s => s.Product.Eenheid + ((s.Product.Eenheid / 100) * s.Product.BTW)).Sum(), 2).ToString();


                txtverkoper.Text = bestelling.PersoneelslidID + ": " + bestelling.Personeelslid.Voornaam;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddProductToBestelling addProduct = new AddProductToBestelling(bestellingID);
            addProduct.ShowDialog();
            Window_Loaded(sender, e);
        }
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {

            string fileName = $"bestelbon-{ ((isLeverancier) ? "Leveranciers" : "Klanten") }\\{txtDatum.Text} {txtBonId.Text}-{txtNaam.Text}.docx";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            try
            {
                var doc = DocX.Create(fileName);

                Picture img = doc.AddImage("img\\2492003.jpg").CreatePicture();
                doc.InsertParagraph().AppendPicture(img);
                doc.InsertParagraph("Company adress");
                doc.InsertParagraph("Company phone");
                doc.AddHyperlink("www.CompanyWebsite.be", new Uri("http://www.CompanyWebsite.be"));
                doc.InsertParagraph();

                Xceed.Document.NET.Table ct = doc.AddTable(1, 2);                
                ct.Rows[0].Cells[0].Paragraphs.First().Append("Iban:");
                ct.Rows[0].Cells[0].Paragraphs.First().Append("BIC:");
                ct.Rows[0].Cells[0].Paragraphs.First().Append("OnderNemingsnr:");
                ct.Rows[0].Cells[0].Paragraphs.First().Append("BTWnr.:");

                ct.Rows[0].Cells[1].Paragraphs.First().Append($"{txtKlant.Text} {txtKlantnr.Text}");
                ct.Rows[0].Cells[1].Paragraphs.First().Append(txtNaam.Text);
                ct.Rows[0].Cells[1].Paragraphs.First().Append(txtAdress.Text);
                ct.Rows[0].Cells[1].Paragraphs.First().Append(txtGemeente.Text);
                ct.Rows[0].Cells[1].Paragraphs.First().Append(txtTelefoon.Text);
                ct.Rows[0].Cells[1].Paragraphs.First().Append(txtMail.Text);
                doc.InsertTable(ct);
                doc.InsertParagraph();
                using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
                {
                    var ProductList = ctx.BestellingProducts.Where(s => s.BestellingID == bestellingID).Select(s => s.Product).Select(s => new
                    {
                        s,
                        Beschrijving = s.Naam + " ( " + s.Leverancier.Contactpersoon + " )",
                        Prijs = "€ " + Math.Round((double)s.Eenheid, 2),
                        Btw = s.BTW + "%",
                        Netto = "€ " + Math.Round((double)(s.Eenheid + ((s.Eenheid / 100) * s.BTW)), 2)
                    }).ToList();

                    Xceed.Document.NET.Table t = doc.AddTable(ProductList.Count() + 1, 5);
                    t.Rows[0].Cells[0].Paragraphs.First().Append("Id");
                    t.Rows[0].Cells[1].Paragraphs.First().Append("beschrijving");
                    t.Rows[0].Cells[2].Paragraphs.First().Append("Eenheidsprijs");
                    t.Rows[0].Cells[3].Paragraphs.First().Append("Btw");
                    t.Rows[0].Cells[4].Paragraphs.First().Append("Netto");
                    for (int i = 0; i < ProductList.Count(); i++)
                    {
                        t.Rows[i + 1].Cells[0].Paragraphs.First().Append(ProductList[i].s.ProductID.ToString());
                        t.Rows[i + 1].Cells[1].Paragraphs.First().Append(ProductList[i].Beschrijving);
                        t.Rows[i + 1].Cells[2].Paragraphs.First().Append(ProductList[i].Prijs);
                        t.Rows[i + 1].Cells[3].Paragraphs.First().Append(ProductList[i].Btw);
                        t.Rows[i + 1].Cells[4].Paragraphs.First().Append(ProductList[i].Netto);
                    }
                    doc.InsertTable(t);
                }
                doc.InsertParagraph($"totaal zonder btw:  €{txtEenTotaal.Text}");
                doc.InsertParagraph($"btw: +€{txtBtwTotaal.Text}");
                doc.InsertParagraph($"Totaal: €{txtTotaal.Text}");
                doc.InsertParagraph();
                doc.InsertParagraph();
                doc.InsertParagraph("Verkoper:");
                doc.InsertParagraph(txtverkoper.Text);

                doc.Save();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }

        }

    }
}
