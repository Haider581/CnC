using CnC.Core.Common;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CnC.Data;
using System.IO;
using System.Xml.Serialization;
using log4net;
using System.Reflection;
using System.Web;
using CnC.Core.Accounts;
using System.Net;

namespace CnC.Service
{
    public class CommonService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CardService));

        public City GetCity(int cityId)
        {
            try
            {
                return GetCities().FirstOrDefault(c => c.Id == cityId);
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<City> GetCities(int? countryId = null)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var cities = (IQueryable<City>)context.Cities;

                    if (countryId != null)
                    {
                        cities = cities.Where(c => c.CountryId == countryId);
                    }
                    return cities.ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public Country GetCountry(int countryId)
        {
            try
            {
                return GetCountries().FirstOrDefault(c => c.Id == countryId);
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }

        }

        public List<Country> GetCountries()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.Countries.ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public Language GetLanguage(int languageId)
        {
            return GetLanguages().FirstOrDefault(l => l.Id == languageId);
        }

        public List<Language> GetLanguages()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.Languages.ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public Language GetLanguageByCountryId(int countryId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    // return context.Languages.FirstOrDefault(ln=>ln.)
                }
                return null;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        #region Password helper
        public string GeneratePassword(int passwordLength) //length of salt    
        {
            // Special characters are not displaying at proper position in Persian email template
            //string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public string EncodeText(string text, string key) //encrypt password    
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            byte[] src = Encoding.Unicode.GetBytes(key);
            byte[] dst = new byte[src.Length + bytes.Length];
            System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            System.Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            //return Convert.ToBase64String(inArray);    
            return EncodeTextdMd5(Convert.ToBase64String(inArray));
        }

        public string EncodeTextdMd5(string text) //Encrypt using MD5    
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)    
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(text);
            encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string    
            return BitConverter.ToString(encodedBytes);
        }

        public string base64Encode(string text) // Encode    
        {
            try
            {
                byte[] encData_byte = new byte[text.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(text);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                //throw new UserException("Error in base64Encode" + ex.Message);
                return null;
            }
        }
        public string base64Decode(string text) //Decode    
        {
            try
            {
                var encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecodeByte = Convert.FromBase64String(text);
                int charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                char[] decodedChar = new char[charCount];
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                string result = new String(decodedChar);
                return result;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                // throw new UserException("Error in base64Decode" + ex.Message);
                return null;
            }
        }

        public string UrlSafeEncrypt(string clearText)
        {
            var encrypt = Encrypt(clearText);
            if (encrypt == null) throw new ArgumentNullException(nameof(encrypt));
            encrypt = HttpUtility.UrlEncode(encrypt);
            return encrypt;
        }
        private string Encrypt(string clearText)
        {
            try
            {
                string encryptionKey = ConfigurationManager.AppSettings["SecretKey"];
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    if (encryptor != null)
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(clearBytes, 0, clearBytes.Length);
                                cs.Close();
                            }
                            clearText = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                new MessageService().SendExceptionMessage(e);
                return null;
            }
            return clearText;
        }

        public string GetVerificationCode()
        {
            var rnd = new Random();
            return "" + rnd.Next(100000, 999999);
        }
        #endregion Password helper

        #region Validation

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Input string if its lengh is OK; otherwise, truncated input string</returns>
        public string EnsureMaximumLength(string str, int maxLength)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
                return str.Substring(0, maxLength);
            else
                return str;
        }

        /// <summary>
        /// Ensures that a string only contains numeric values
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
        public string EnsureNumericOnly(string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            var result = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                    result.Append(c);
            }
            return result.ToString();

            // Loop is faster than RegEx
            //return Regex.Replace(str, "\\D", "");
        }

        /// <summary>
        /// Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public string EnsureNotNull(string str)
        {
            if (str == null)
                return string.Empty;

            return str;
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public bool IsValidEmail(string email)
        {
            bool result = false;
            if (String.IsNullOrEmpty(email))
                return result;
            email = email.Trim();
            result = Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return result;
        }

        #endregion

        #region XML

        public T FromXml<T>(String xml)
        {
            T returnedXmlClass = default(T);

            try
            {
                using (TextReader reader = new StringReader(xml))
                {
                    try
                    {
                        returnedXmlClass =
                            (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                    }
                    catch (Exception exception)
                    {
                        new MessageService().SendExceptionMessage(exception);
                        log.Error(exception);
                        // String passed is not XML, simply return defaultXmlClass
                    }
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
            }

            return returnedXmlClass;
        }

        #endregion


        /// <summary>
        /// method to get Client ip address
        /// </summary>
        /// <param name="GetLan"> set to true if want to get local(LAN) Connected ip address</param>
        /// <returns></returns>
        public string GetIPAddress(bool GetLan = false)
        {
            string visitorIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (String.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
            {
                GetLan = true;
                visitorIPAddress = string.Empty;
            }

            if (GetLan)
            {
                if (string.IsNullOrEmpty(visitorIPAddress))
                {
                    //This is for Local(LAN) Connected ID Address
                    string stringHostName = Dns.GetHostName();
                    //Get Ip Host Entry
                    IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                    //Get Ip Address From The Ip Host Entry Address List
                    IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                    try
                    {
                        visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                    }
                    catch
                    {
                        try
                        {
                            visitorIPAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            try
                            {
                                arrIpAddress = Dns.GetHostAddresses(stringHostName);
                                visitorIPAddress = arrIpAddress[0].ToString();
                            }
                            catch
                            {
                                visitorIPAddress = "127.0.0.1";
                            }
                        }
                    }
                }
            }
            return visitorIPAddress;
        }

        /// <summary>
        /// method to get Client Web Browser Name
        /// </summary>
        /// <returns></returns>

        public string GetWebBrowserName()
        {
            string webBrowserName = string.Empty;
            try
            {
                webBrowserName = HttpContext.Current.Request.Browser.Browser;
            }
            catch (Exception ex)
            {
                new MessageService().SendExceptionMessage(ex);
                throw new Exception(ex.Message);
            }
            return webBrowserName;
        }

        /// <summary>
        /// method to get Client Machine Name
        /// </summary>
        /// <returns></returns>

        public string GetMachineName()
        {
            string machineName = string.Empty;
            try
            {
                machineName = System.Environment.MachineName;
            }
            catch (Exception ex)
            {
                new MessageService().SendExceptionMessage(ex);
                throw new Exception(ex.Message);
            }
            return machineName;
        }

    }
}
