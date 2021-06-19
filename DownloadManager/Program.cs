using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            var fw = new DownloaderLogic("http://download.microsoft.com/download/E/E/2/EE2D29A1-2D5C-463C-B7F1-40E4170F5E2C/KinectSDK-v1.0-Setup.exe", @"F:\KinetSDK.exe", 5120);

            // Display progress...
            Task.Factory.StartNew(() =>
            {
                while (!fw.Done)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(string.Format("ContentLength: {0} | BytesWritten: {1}", fw.ContentLength, fw.BytesWritten));
                }
            });

            // Start the download...
            fw.Start();

            // Simulate pause...
            Thread.Sleep(500);
            fw.Pause();
            Thread.Sleep(2000);

            // Start the download from where we left, and when done print to console.
            fw.Start().ContinueWith(t => Console.WriteLine("Done"));

            Console.ReadKey();
        }
    }
}
