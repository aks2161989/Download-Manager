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
		string downloadPath = "https://ia803007.us.archive.org/35/items/traque/traque.zip";
		string filePath = "f:\\my_downloads\\cernaja-Metka.zip";
		public DownloaderLogic()
		{
			IsPaused = false;
			IsDownloading = false;
		}
		public void Download()
		{
			IsDownloading = true;
			DateTime startTime = DateTime.UtcNow;
			WebRequest request = WebRequest.Create(downloadPath);
			if (ResumeDownloadIfExists(request, filePath) == true)
			{
				return;
			}
			WebResponse response = request.GetResponse();
			using (Stream responseStream = response.GetResponseStream())
			{
				using (Stream fileStream = File.OpenWrite(filePath))
				{
					byte[] buffer = new byte[4096];
					int bytesRead = responseStream.Read(buffer, 0, 4096);
					while (bytesRead > 0 && IsPaused == false)
					{
						 fileStream.WriteAsync(buffer, 0, bytesRead);
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

		private bool ResumeDownloadIfExists(WebRequest webRequest, string v)
		{
			if (File.Exists(v))
			{
				webRequest.Method = "HEAD";
				long downloadFileSize = -1;
				using (WebResponse webResponse = webRequest.GetResponse())
				{
					if (long.TryParse(webResponse.Headers.Get("Content-Length"), out long contentLength))
					{
						downloadFileSize = contentLength; //Gets the size of the ownload in bytes
					}
				}

				FileInfo fileInfo = new FileInfo(v);
				long existingFileSize = fileInfo.Length; // The size of the existing file in bytes
				if (downloadFileSize <= existingFileSize)
				{
					return true; // If existing file is of the same size as the download or of larger size, no need to download
				}

				// File exists but is smaller than download, so we can resume download
				DateTime startTime = DateTime.UtcNow;
				WebRequest request = WebRequest.Create(downloadPath);
				WebResponse response = request.GetResponse();
				using (Stream responseStream = response.GetResponseStream())
				{
                    byte[] bufferDownload = new byte[4096];
                    int bytesReadFromDownload = 0;
                    byte[] bufferFile = new byte[4096];
                    int bytesReadFromFile = 0;
                    using (Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
					{
						
						 bytesReadFromDownload = responseStream.Read(bufferDownload, 0, 4096);

						
						bytesReadFromFile = fileStream.Read(bufferFile, 0, 4096);
						while (bytesReadFromDownload > 0 && IsPaused == false)
						{
							IsDownloading = true;
							if (bytesReadFromFile > 0 && bufferDownload.SequenceEqual(bufferFile))
							{
								bytesReadFromDownload = responseStream.Read(bufferDownload, 0, 4096);
								bytesReadFromFile = fileStream.Read(bufferFile, 0, 4096);
								continue;
							}
							else if (bytesReadFromFile == 0)
							{
								using (var stream = new FileStream(filePath, FileMode.Append))
								{
									DateTime nowTime = DateTime.UtcNow;
									if ((nowTime - startTime).TotalMinutes > 5)
									{
										throw new ApplicationException(
									"Download timed out");
									}
									stream.Write(bufferDownload, 0, bytesReadFromDownload);
								}
								bytesReadFromDownload = responseStream.Read(bufferDownload, 0, 4096);
							}
						}
						IsDownloading = false;
					}
				}
				return true;
			}
			else
			{
				return false;
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
