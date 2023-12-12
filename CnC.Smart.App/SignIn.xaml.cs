using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace CnC.Smart.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SignIn : MetroWindow
    {   
        public SignIn()
        {
            // Get Installed Browsers List
            RegistryKey browserKeys;

            //on 64 bit the browsers are in a different location
            browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");

            // on 32 bit
            if (browserKeys == null)
                browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");

            string[] browserNames = browserKeys.GetSubKeyNames();

            if (!browserNames.Contains("Google Chrome"))
            {
                SmartMessageBox messageBox = new SmartMessageBox();
                messageBox.lblMessage.Content = "This application requires Chrome which is not found on this machine, please download and install it";
                messageBox.lblDownloadUrl.Content = "https://www.google.com/chrome/";
                messageBox.ShowDialog();
                this.Close();
            }

            Process[] chromeInstances = Process.GetProcessesByName("chrome");
            if (chromeInstances.Length > 0)
            {
                MessageBox.Show("Please close all instances of Chrome and restart the application", "Use Card Services"
                    , MessageBoxButton.OK);
                this.Close();
            }            

            InitializeComponent();
            lblError.Foreground = new SolidColorBrush(Colors.Red);
        }


        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            textBox_UserName.BorderBrush = System.Windows.Media.Brushes.Black;
            textBox_Password.BorderBrush = System.Windows.Media.Brushes.Black;
            
            if (string.IsNullOrEmpty(textBox_UserName.Text))
            {
                textBox_UserName.BorderBrush = System.Windows.Media.Brushes.Red;
                lblError.Content = "please enter valid username and password";
                return;
            }

            if (string.IsNullOrEmpty(textBox_Password.Password))
            {
                textBox_Password.BorderBrush = System.Windows.Media.Brushes.Red;
                lblError.Content = "please enter valid username and password";
                return;
            }

            if (Authenticate(textBox_UserName.Text.Trim(), textBox_Password.Password.Trim()))
            {
                this.Close();
            }
        }
        public bool Authenticate(string username, string password)
        {
            string WEBSERVICE_URL = ConfigurationManager.AppSettings["ApiUrl"] + "api/ProxyAccount/Authenticate";

            try
            {
                string proxyServer = null;

                var request = (HttpWebRequest)WebRequest.Create(WEBSERVICE_URL);
                request.Method = "POST";
                request.Timeout = 20000;
                request.ContentType = "application/json";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", Base64Encode(username + ":" + password));

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        return false;

                    using (Stream stream = response.GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                        {
                            proxyServer = sr.ReadToEnd().Trim();
                        }
                    }
                }

                if (string.IsNullOrEmpty(proxyServer))
                    return false;

                var proxyServerInfo = proxyServer.Replace("\"", "").Split('|');

                if (proxyServerInfo.Length < 3)
                    return false;

                var installedAppVersion = new Version(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                var latestAppVersion = new Version(proxyServerInfo[0]);
                var result = installedAppVersion.CompareTo(latestAppVersion);

                if (result == 0)
                {
                    Process.Start("chrome", "--proxy-pac-url=\"" + proxyServerInfo[2] + "\"");
                    return true;
                }
                else
                {
                    SmartMessageBox messageBox = new SmartMessageBox();
                    messageBox.lblMessage.Content = "New version found, please download and install it";
                    messageBox.lblDownloadUrl.Content = proxyServerInfo[1];
                    messageBox.ShowDialog();
                    return true;
                }
            }
            catch (WebException) { }
            catch (Exception excep)
            {
                File.WriteAllText("temp.log", excep.Message);
                lblError.Content = "Unable to connect to server";
            }

            return false;            
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
    public class UserCredentials : IDataErrorInfo
    {
        public string Name { get; set; }

        public string Position { get; set; }


        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == "Name")
                {
                    if (string.IsNullOrEmpty(Name) || Name.Length < 3)
                        result = "Please enter a valid Username or Email";
                }
                if (columnName == "Position")
                {
                    if (string.IsNullOrEmpty(Position) || Position.Length < 3)
                        result = "Please enter a password";
                }
                return result;
            }
        }
    }
}
