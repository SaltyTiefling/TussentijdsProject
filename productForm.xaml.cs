using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public productForm(int productID, bool editable = false)
        {
            this.productID = productID;

            this.editable = editable;
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (InputCorrect().Count <= 0)
            {
                using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
                {
                    var query = ctx.Products.Where(s => s.ProductID == productID).Select(s => s).FirstOrDefault();
                    query.Naam = txtNaam.Text;
                    query.Inkoopprijs = decimal.Parse(txtinpkoopprijs.Text);
                    query.Marge = decimal.Parse(txtMarge.Text);
                    query.Eenheid = decimal.Parse(txtEenheid.Text);
                    query.BTW = int.Parse(txtBtw.Text);
                    query.CategorieID = (cbCategorie.SelectedItem as Categorie).CategorieID;
                    query.LeverancierID = (cbLeverancier.SelectedItem as Leverancier).LeverancierID;
                    ctx.SaveChanges();
                }
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(String.Join(Environment.NewLine, InputCorrect()));
            }
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
                

                cbLeverancier.ItemsSource = ctx.Leveranciers.Select(s => s).ToList();

                try
                {
                    cbLeverancier.SelectedIndex = cbLeverancier.Items.IndexOf(product.Leverancier);
                }
                catch (Exception)
                {
                    cbLeverancier.SelectedIndex = 0;
                    throw;
                }

                cbCategorie.ItemsSource = ctx.Categories.Select(s => s).ToList();
                try
                {
                    cbCategorie.SelectedIndex = cbCategorie.Items.IndexOf(product.Categorie);
                }
                catch (Exception)
                {
                    cbCategorie.SelectedIndex = 0;
                    throw;
                }


                txtNaam.IsEnabled =
                txtinpkoopprijs.IsEnabled =
                txtMarge.IsEnabled =
                txtEenheid.IsEnabled =
                txtBtw.IsEnabled =
                cbCategorie.IsEnabled =
                cbLeverancier.IsEnabled = editable;
            }
            InputCorrect();
        }


        private List<string> InputCorrect()
        {
            List<string> answer = new List<string>();

            if (txtNaam.Text.Length <= 0)
            {
                epNaam.Visibility = Visibility.Visible;
                answer.Add("Geef een naam in voor het product");
                
            }
            else
            {
                epNaam.Visibility = Visibility.Hidden;
                txtNaam.ToolTip = null;
            }

            try
            {
                decimal.Parse(txtinpkoopprijs.Text);
                epInkoopprijs.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                epInkoopprijs.Visibility = Visibility.Visible;
                answer.Add("Geef een geldige inkoopprijs");
            }
            try
            {
                decimal.Parse(txtMarge.Text);
                epMarge.Visibility = Visibility.Hidden;

            }
            catch (Exception)
            {
                epMarge.Visibility = Visibility.Visible;
                answer.Add("Geef een geldige Marge");
            }

            try
            {
                decimal.Parse(txtEenheid.Text);
                epEenheid.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                epEenheid.Visibility = Visibility.Visible;
                answer.Add("Geef een geldige Eenheid");
            }

            try
            {
                int.Parse(txtBtw.Text);
                epBtw.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                epBtw.Visibility = Visibility.Visible;
                answer.Add("Geef een geldige BTW");
            }


            return answer;
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputCorrect();
        }

    }
}
