using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    /// Interaction logic for productForm.xaml
    /// </summary>
    public partial class productForm : Window
    {
        int productID;
        bool editable;
        public productForm(Personeelslid user, int productID, bool editable = false)
        {
            this.productID = productID;

            this.editable = editable;
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                var query = ctx.Products.Where(s => s.ProductID == productID).Select(s => s).FirstOrDefault();
                query.Naam = txtNaam.Text;
                query.Inkoopprijs = decimal.Parse(txtinpkoopprijs.Text);
                query.Marge = decimal.Parse(txtMarge.Text);
                query.Eenheid = decimal.Parse(txtEenheid.Text);
                query.BTW = int.Parse(txtBtw.Text);
                query.Categorie = (cbCategorie.SelectedItem as Categorie);
                query.Leverancier = (cbLeverancier.SelectedItem as Leverancier);
                ctx.SaveChanges();
            }
            this.Close();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                var product = ctx.Products.Where(s => s.ProductID == productID).Select(s => s).FirstOrDefault();
                lblId.Text = product.ProductID.ToString();
                txtNaam.Text = product.Naam;
                txtinpkoopprijs.Text = product.Inkoopprijs.ToString();
                txtMarge.Text = product.Marge.ToString();
                txtEenheid.Text = product.Eenheid.ToString();
                txtBtw.Text = product.BTW.ToString();

                if (product.Leverancier != null)
                {
                    cbCategorie.Items.Add(ctx.Categories.Where(s => s.CategorieID == product.Categorie.CategorieID).Select(s => s).FirstOrDefault());
                    cbLeverancier.Items.Add(ctx.Leveranciers.Where(s => s.LeverancierID == product.Leverancier.LeverancierID).Select(s => s).FirstOrDefault());
                    cbLeverancier.SelectedIndex = 0;
                    cbCategorie.SelectedIndex = 0;
                }
                else
                {
                    cbLeverancier.ItemsSource = ctx.Leveranciers.Select(s => s).ToList();
                    cbCategorie.ItemsSource = ctx.Categories.Select(s => s).ToList();
                }

                txtNaam.IsEnabled = editable;
                txtinpkoopprijs.IsEnabled = editable;
                txtMarge.IsEnabled = editable;
                txtEenheid.IsEnabled = editable;
                txtBtw.IsEnabled = editable;
                cbCategorie.IsEnabled = editable;
                cbLeverancier.IsEnabled = editable;

            }
        }
    }
}
