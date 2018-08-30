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
    /// Логика взаимодействия для AuthorsWindow.xaml
    /// </summary>
    public partial class AuthorsWindow : Window
    {
        public AuthorsWindow()
        {
            InitializeComponent();
        }

        private void Authors_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
