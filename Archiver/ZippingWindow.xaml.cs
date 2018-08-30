using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Archiver
{
    public partial class ZippingWindow : Window
    {
        ArchiveProvider compressor = new ArchiveProvider();

        public ZippingWindow()
        {
            InitializeComponent();
            compressor.ProcessMessages += ProcessMessages;
        }

        void ProcessMessages(string message)
        {
            LogListBox.Items.Add(message);
        }

        private void Zipping_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void AddFilesButton_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FilesListBox.Items.Add(ofd.FileName);
                }
            }
        }

        private void AddFoldersButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    FilesListBox.Items.Add(fbd.SelectedPath);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FilesListBox.Items.RemoveAt(FilesListBox.SelectedIndex);
            }
            catch { }
        }

        private void ZipButton_Click(object sender, RoutedEventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CompressorOption option = new CompressorOption()
                    {
                        Password = PasswordBox.Password,
                        Output = sfd.FileName
                    };
                    foreach (string line in FilesListBox.Items)
                        option.IncludePath.Add(line);
                    compressor.Compress(option);
                }
            }
        }
    }
}
