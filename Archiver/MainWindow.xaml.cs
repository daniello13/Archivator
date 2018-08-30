using System.Windows;


namespace Archiver
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ZippingButton_Click(object sender, RoutedEventArgs e)
        {
            ZippingWindow zipping = new ZippingWindow();
            zipping.Show();
            Close();
        }

        private void UnzippingButton_Click(object sender, RoutedEventArgs e)
        {
            UnzippingWindow unzipping = new UnzippingWindow();
            unzipping.Show();
            Close();
        }
        //-----------begin new
        private void LZWButton_Click(object sender, RoutedEventArgs e)
        {
            LZW_Window zipping = new LZW_Window();
            zipping.Show();
            Close();
        }
        
        private void UnLZWButton_Click(object sender, RoutedEventArgs e)
        {
            UnLZW_Window unzipping = new UnLZW_Window();
            unzipping.Show();
            Close();
        }
        //------------end new
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void AuthorButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorsWindow authors = new AuthorsWindow();
            authors.Show();
            Close();
        }
    }
}
