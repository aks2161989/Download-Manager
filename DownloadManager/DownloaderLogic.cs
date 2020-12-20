using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager
{
    class DownloaderLogic
    {
        public string DownloadLink { get; set; }
        public string DownloadDestination { get; set; }

        internal void GetUserInput()
        {
            Console.Write("Enter the download URL: ");
            DownloadLink = Console.ReadLine();
            Console.Write("Enter the download destination: ");
            DownloadDestination = Console.ReadLine();
        }

        internal void Download()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(DownloadLink, DownloadDestination);
            Console.WriteLine($"The file has been downloaded to {DownloadDestination}");
        }
    }
}
