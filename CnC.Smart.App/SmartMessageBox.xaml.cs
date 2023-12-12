using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CnC.Smart.App
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class SmartMessageBox : MetroWindow
    {        
        public SmartMessageBox()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            // Open download Chrome url in default browser
            if (lblDownloadUrl.Content.ToString().ToLower().Contains("chrome"))
                Process.Start(lblDownloadUrl.Content.ToString());
            else
                Process.Start("chrome", lblDownloadUrl.Content.ToString());

            this.Close();
        }
    }
}
