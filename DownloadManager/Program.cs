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

            DownloaderLogic dl = new DownloaderLogic();
            Console.WriteLine("Beginning download...");
            Task downloaderTask = Task.Run( () => {
                dl.Download();
            });
            Task pausingTask = Task.Run(() => {
                dl.Pause();
            });
            Task.WaitAll(downloaderTask, pausingTask);
            Console.WriteLine("Download completed.");
        }
    }
}
