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
    /// Interaction logic for CategorienForm.xaml
    /// </summary>
    public partial class CategorienForm : Window
    {
        int categorieID;
        bool editable = false;
        public CategorienForm(int categorieID, bool editable = false)
        {
            this.categorieID = categorieID;
            this.editable = editable;
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                Categorie cat = ctx.Categories.Where(s => s.CategorieID == categorieID).FirstOrDefault();

                lblId.Text = cat.CategorieID.ToString();
                txtCategorieNaam.Text = cat.CategorieNaam;
            }
            txtCategorieNaam.IsEnabled = editable;
        }
        private List<string> InputCorrect()
        {
            List<string> answer = new List<string>();

            if (txtCategorieNaam.Text.Length <= 0)
            {
                epCategorieNaam.Visibility = Visibility.Visible;
                answer.Add("geef een categorienaam in");
            }
            else
            {
                epCategorieNaam.Visibility = Visibility.Hidden;
            }
            return answer;
        }

        private void textfield_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputCorrect();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (InputCorrect().Count <= 0)
            {
                using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
                {
                    Categorie cat = ctx.Categories.Where(s => s.CategorieID == categorieID).FirstOrDefault();
                    cat.CategorieNaam = txtCategorieNaam.Text;
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
    }
}
