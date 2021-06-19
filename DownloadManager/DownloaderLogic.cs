using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DownloadManager
{
	public class DownloaderLogic
	{
		public bool IsPaused { get; set; }
		public bool IsDownloading { get; set; }
		public DownloaderLogic()
		{
			IsPaused = false;
			IsDownloading = false;
		}
		public async Task Download()
		{
			IsDownloading = true;
			DateTime startTime = DateTime.UtcNow;
			WebRequest request = WebRequest.Create("https://dl.myabandonware.com/t/n00C5Nqj4U1hgr6nJe7ABc0dr2d9SiX7UkN2t3iIAYGc52jf3X/Man-of-War-II-Chains-of-Command_Win_EN_ISO-Version.zip");
			WebResponse response = request.GetResponse();
			using (Stream responseStream = response.GetResponseStream())
			{
				using (Stream fileStream = File.OpenWrite(@"f:\\my_downloads\\Man-of-War-II-Chains-of-Command_Win_EN_ISO-Version.zip"))
				{
					byte[] buffer = new byte[4096];
					int bytesRead = responseStream.Read(buffer, 0, 4096);
					while (bytesRead > 0 && IsPaused == false)
					{
						 await fileStream.WriteAsync(buffer, 0, bytesRead);
						DateTime nowTime = DateTime.UtcNow;
						if ((nowTime - startTime).TotalMinutes > 5)
						{
							throw new ApplicationException(
								"Download timed out");
						}
						bytesRead = responseStream.Read(buffer, 0, 4096);
					}
					IsDownloading = false;
				}
			}
		}
		public void Pause()
		{
			char? input = null;
			Console.Write("Enter p to pause: ");
			while (IsDownloading == true && input == null)
			{
				input = Console.ReadKey().KeyChar;
			}
			if (IsDownloading == false)
			{
				return;
			}
			if (input == 'p' || input == 'P')
			{
				IsPaused = true;
				Console.WriteLine("Download paused.");
			}
		}
	}
}
