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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace KML2SQL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        MapUploader myUploader;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void myUploader_progressUpdate(string text)
        {
            resultTextBox.Text = text;
        }

        private void serverNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (serverNameBox.Text == "foo.myserver.com")
                serverNameBox.Clear();
        }

        private void KMLFileLocationBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (KMLFileLocationBox.Text == "C:\\...")
                KMLFileLocationBox.Clear();
        }

        private void userNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (userNameBox.Text == "username")
                userNameBox.Clear();
        }

        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordBox.Clear();
        }

        private void tableBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tableBox.Text == "myTable")
                tableBox.Clear();
        }

        private void columnNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (columnNameBox.Text == "polygon")
                columnNameBox.Clear();
        }

        private void sridCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            sridBox.IsEnabled = true;
        }

        private void sridCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            sridBox.Text = "4326";
            sridBox.IsEnabled = false;
        }

        private void CreateDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            bool geography;
            if (geographyMode.IsChecked != null)
                geography = (bool)geographyMode.IsChecked;
            else
                geography = false;
            int srid = parseSRID(geography);
            if (srid != 0)
            {
                try
                {
                    myUploader = new MapUploader(serverNameBox.Text, databaseNameBox.Text, userNameBox.Text, passwordBox.Password, 
                        columnNameBox.Text, KMLFileLocationBox.Text, tableBox.Text, srid, geography);
                    Binding b = new Binding();
                    b.Source = myUploader;
                    b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    b.Path = new PropertyPath("Progress");
                    this.resultTextBox.SetBinding(TextBlock.TextProperty, b);
                    myUploader.Upload();
                }
                catch (Exception ex)
                {
                    resultTextBox.Text = "Error: " + ex.ToString();
                }
            }
        }

        private int parseSRID(bool geographyMode)
        {
            if (!geographyMode)
                return 4326;
            else
            {
                MessageBoxResult sridMessage;
                int srid;
                if (int.TryParse(sridBox.Text, out srid))
                    return srid;
                else
                    sridMessage = MessageBox.Show("SRID must be a valid four digit number");
                return srid;
            }
        }

        private void databaseNameBox_GotFocus(object sender, RoutedEventArgs e)
        {

            if (databaseNameBox.Text == "myDatabase")
                databaseNameBox.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
            myOpenFileDialog.Filter = "KML Files (*.kml)|*.kml|All Files (*.*)|*.*";
            myOpenFileDialog.FileName = "myFile.kml";
            Nullable<bool> result = myOpenFileDialog.ShowDialog();
            if (result == true)
            {
                try
                {
                    KMLFileLocationBox.Text = myOpenFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while opening the file" + myOpenFileDialog.FileName + "\n" + ex.Message, "Unable to open KML file.");
                }
            }
        }

        private void geometryMode_Checked(object sender, RoutedEventArgs e)
        {
            if (sridCheckBox != null)
                sridCheckBox.IsEnabled = false;
            if (sridBox != null)
                sridBox.Text = "NA";
        }

        private void geographyMode_Checked(object sender, RoutedEventArgs e)
        {
            if (sridCheckBox != null)
                sridCheckBox.IsEnabled = true;
            if (sridBox != null)
                sridBox.Text = "4326";
        }
    }
}
