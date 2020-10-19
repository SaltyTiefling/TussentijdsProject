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
        public Product product = new Product();
        bool editable;
        public productForm(bool editable = false)
        {
            this.editable = editable;
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities ctx = new tussentijds_projectEntities())
            {
                product = new Product();
                ctx.Products.Add(product);
                ctx.SaveChanges();
                
                product.Naam = txtNaam.Text;
                product.Inkoopprijs = decimal.Parse(txtinpkoopprijs.Text);
                product.Marge = decimal.Parse(txtMarge.Text);
                product.Eenheid = decimal.Parse(txtEenheid.Text);
                product.BTW = int.Parse(txtBtw.Text);
                product.Categorie = (cbCategorie.SelectedItem as Categorie);
                product.Leverancier = (cbLeverancier.SelectedItem as Leverancier);
                
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities ctx = new tussentijds_projectEntities())
            {
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
