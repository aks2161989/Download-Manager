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
		public void Download()
		{
			DateTime startTime = DateTime.UtcNow;
			WebRequest request = WebRequest.Create("https://wds.gcdn.co/wgc/releases_tTrHgLCKHBRiaL/wgc_21.03.00.5224_na/world_of_warships_ww_install_na.exe?enctid=cc7lgfnncntt");
			WebResponse response = request.GetResponse();
			using (Stream responseStream = response.GetResponseStream())
			{
				using (Stream fileStream = File.OpenWrite(@"f:\\my_downloads\\world_of_warships_ww_install_na.exe"))
				{
					byte[] buffer = new byte[4096];
					int bytesRead = responseStream.Read(buffer, 0, 4096);
					while (bytesRead > 0)
					{
						fileStream.Write(buffer, 0, bytesRead);
						DateTime nowTime = DateTime.UtcNow;
						if ((nowTime - startTime).TotalMinutes > 5)
						{
							throw new ApplicationException(
								"Download timed out");
						}
						bytesRead = responseStream.Read(buffer, 0, 4096);
					}
				}
			}
		}
	}
}
