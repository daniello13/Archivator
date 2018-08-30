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
    /// Логика взаимодействия для LZW_Window.xaml
    /// </summary>
    public partial class LZW_Window : Window
    {
        public LZW_Window()
        {
            InitializeComponent();
            
        }
        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LZW arch = new LZW();
            arch.Compress(t1.Text, t2.Text);
        }
    }
}
