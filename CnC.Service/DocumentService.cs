using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Service
{
    public class DocumentService
    {
        private string documentMainDirectoryPath;

        public DocumentService()
        {
            documentMainDirectoryPath = ConfigurationManager.AppSettings["DocumentMainDirectoryPath"];
            if (!documentMainDirectoryPath.EndsWith("\\"))
                documentMainDirectoryPath += "\\";
        }

        public string GetDocumentDirectoryPath(string nic)
        {
            if (string.IsNullOrEmpty(nic) || nic.Length != new SettingService().NINLength)
                throw new ArgumentException("NIN is invalid");

            string documentDirectoryPath = documentMainDirectoryPath + nic.Substring(0, 4) + "\\" + nic;

            if (!Directory.Exists(documentDirectoryPath))
                Directory.CreateDirectory(documentDirectoryPath);

            return documentDirectoryPath += "\\";
        }
    }
}
