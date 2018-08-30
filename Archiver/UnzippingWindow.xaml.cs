using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Archiver
{
    public partial class UnzippingWindow : Window
    {
        ArchiveProvider decompressor = new ArchiveProvider();

        public UnzippingWindow()
        {
            InitializeComponent();
            decompressor.ProcessMessages += ProcessMessages; 
        }
        void ProcessMessages(string message)
        {
            LogListBox.Items.Add(message);
        }
        private void Unzipping_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ChooseTextBox.Text = ofd.FileName;
                }
            }
        }

        private void UnzipButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    decompressor.Decompress(ChooseTextBox.Text, fbd.SelectedPath, PasswordBox.Password);
                }
            }
        }
    }
}
