using System;
using System.Collections.Generic;
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
using System.Drawing;


namespace TussentijdsProject
{
    /// <summary>
    /// Interaction logic for PersoneelsForm.xaml
    /// </summary>
    public partial class PersoneelsForm : Window
    {
        int personeelsID;
        bool editable;
        public PersoneelsForm(int personeelsID, bool editable = false)
        {
            this.personeelsID = personeelsID;
            this.editable = editable;
            InitializeComponent();

            InputCorrect();
        }

        private List<string> InputCorrect()
        {
            List<string> answer = new List<string>();

            if (txtVoornaam.Text.Length <= 0)
            {
                epVoorNaam.Visibility = Visibility.Visible;
                answer.Add("geef een naam in");
            }
            else
            {
                epVoorNaam.Visibility = Visibility.Hidden;
            }

            return answer;
        }

        private void textfield_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputCorrect();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
            {
                Personeelslid personeelslid = ctx.Personeelslids.Where(s => s.PersoneelslidID == personeelsID).FirstOrDefault();
                lblId.Text = personeelslid.PersoneelslidID.ToString();
                txtVoornaam.Text = personeelslid.Voornaam;

                try
                {
                    Stream StreamObj = new MemoryStream(personeelslid.ProfielPhoto);
                    BitmapImage BitObj = new BitmapImage();
                    BitObj.BeginInit();
                    BitObj.StreamSource = StreamObj;
                    BitObj.EndInit();
                    imgProfielPhoto.Source = BitObj;
                }catch (Exception) {}

                lbrollen.ItemsSource = personeelslid.PersoneelslidRols.Select(s => s.Rol).ToList();
            }

            txtVoornaam.IsEnabled =
                lbrollen.IsEnabled =
                btnImge.IsEnabled = editable;

            btnImge.Visibility =
                btnRollen.Visibility = (editable)? Visibility.Visible : Visibility.Hidden;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (InputCorrect().Count <= 0)
            {
                using (tussentijds_projectEntities1 ctx = new tussentijds_projectEntities1())
                {
                    Personeelslid personeelslid = ctx.Personeelslids.Where(s => s.PersoneelslidID == personeelsID).FirstOrDefault();
                    personeelslid.Voornaam = txtVoornaam.Text;

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

        private void btnRollen_Click(object sender, RoutedEventArgs e)
        {
            ChangeRollsForm crf = new ChangeRollsForm(personeelsID);
            crf.ShowDialog();
        }
    }
}
