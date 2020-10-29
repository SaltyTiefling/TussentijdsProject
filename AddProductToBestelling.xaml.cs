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
    /// Interaction logic for AddProductToBestelling.xaml
    /// </summary>
    public partial class AddProductToBestelling : Window
    {
        int bestellingID;
        public AddProductToBestelling(int bestellingID)
        {
            this.bestellingID = bestellingID;
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                cbProducten.ItemsSource = ctx.Products.ToList();
                cbProducten.SelectedIndex = 0;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                ctx.BestellingProducts.Add(new BestellingProduct() { BestellingID = bestellingID, ProductID = (cbProducten.SelectedItem as Product).ProductID});
                ctx.SaveChanges();
                this.Close();
            }
        }
    }
}
