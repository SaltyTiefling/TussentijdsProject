using System;
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
    /// Interaction logic for ChangeRollsForm.xaml
    /// </summary>
    public partial class ChangeRollsForm : Window
    {
        int personeelsID;
        public ChangeRollsForm(int personeelsID)
        {
            this.personeelsID = personeelsID;
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LaadLijsten();   
        }

        private void LaadLijsten()
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                Personeelslid personeelslid = ctx.Personeelslids.Where(s => s.PersoneelslidID == personeelsID).FirstOrDefault();
                lbCurrent.ItemsSource = personeelslid.PersoneelslidRols.Select(s => s.Rol).ToList();
                lbAvailable.ItemsSource = ctx.Rols.Select(s => s).ToList();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                Personeelslid personeelslid = ctx.Personeelslids.Where(s => s.PersoneelslidID == personeelsID).FirstOrDefault();
                //ctx.PersoneelslidRols.RemoveRange(ctx.PersoneelslidRols.Where(s => (s.PersoneelslidID == personeelsID)).Where(s => lbCurrent.SelectedItems.Contains(s.Rol)));
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
