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

namespace Archiver
{
    /// <summary>
    /// Логика взаимодействия для UnLZW_Window.xaml
    /// </summary>
    public partial class UnLZW_Window : Window
    {
        public UnLZW_Window()
        {
            InitializeComponent();
        }

        private void but_Click(object sender, RoutedEventArgs e)
        {
            LZW arch = new LZW();
            arch.Decompress(t1.Text, t2.Text);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
