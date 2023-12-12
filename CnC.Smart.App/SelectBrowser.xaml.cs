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
    /// Interaction logic for SelectBrowser.xaml
    /// </summary>
    public partial class SelectBrowser : MetroWindow
    {
        public string ProxyURL = null;
        public SelectBrowser(string _proxyURL)
        {
            ProxyURL = (_proxyURL.Contains("http://www.")) ? _proxyURL.Replace(@"\", "") : "http://www." + _proxyURL.Replace(@"\", "");
            InitializeComponent();
        }

        private void DefaultBrowser_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ProxyURL);
        }

        private void ChromeBrowser_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("chrome", @ProxyURL);
        }

        private void IexploreBrowser_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("iexplore", @ProxyURL);
        }

        private void FirefoxBrowser_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("firefox", @ProxyURL);
        }
    }
}
