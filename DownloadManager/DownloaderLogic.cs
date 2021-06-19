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
        private volatile bool _allowedToRun; // volatile keyword indicates that this field may be modified by multiple threads that are executing at the same time

        private string _source;
        private string _destination;
        private int _chunkSize;

        private Lazy<int> _contentLength; // Lazy<int> defers the initialization of _contentLength until it is first used

        public int BytesWritten { get; private set; }
        public int ContentLength { get { return _contentLength.Value; } }

        public bool Done { get { return ContentLength == BytesWritten; } }

        public DownloaderLogic(string source, string destination, int chunkSize)
        {
            _allowedToRun = true;

            _source = source;
            _destination = destination;
            _chunkSize = chunkSize;
            _contentLength = new Lazy<int>(() => Convert.ToInt32(GetContentLength()));

            BytesWritten = 0;
        }

        private long GetContentLength()
        {
            var request = (HttpWebRequest)WebRequest.Create(_source); // Initializes a new WebRequest for a specified URI scheme

            request.Method = "HEAD";
            /* HTTP 'HEAD' method requests headers that would be returned if the 'HEAD' request's URL was instead requested with the HTTP 'GET' method. For instance, if the URL might produce a large download, a HEAD request could read its Content-Length header to check the filesize without actually downloading the file
		*/

            using(var response = request.GetResponse())// Returns a response from an internet request
            {
                return response.ContentLength; // When overriden in a descendant class, gets or sets the content length of data being received 
            }
        }
        private async Task Start(int range)
        {
            if (!_allowedToRun)
                throw new InvalidOperationException();

            var request = (HttpWebRequest)WebRequest.Create(_source);// Create method initializes a new WebRequest, which makes a request to a URI

            request.Method = "GET";// Method is a property of HttpWebRequest. GET is an HTTP 1.1 protocol verb which means retrieve whatever information is identified by the Request-URI

            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;) ";
            /* UserAgent is a property of HttpWebRequest which gets or sets the value of the User-agent HTTP header. The User-agent HTTP header is a characteristic string that lets servers and network peers identify the application, operating system, vendor and/or version of the requesting user agent. A user agent is a computer program representing a person, for example a browser in a web content. */

            request.AddRange(range);
            // AddRange method adds a range header to the request. The Range header on a request allows a client to request that it only wants to receive some part of the specified range of bytes in an HTTP entity.

            using (var response = await request.GetResponseAsync())// GetResponseAsync method, when overridden in a descendant class, returns a response to an Internet request as an asynchronous operation
            {
                using (var responseStream = response.GetResponseStream())// GetResponseStream method, when overridden in a descendant class, returns the data stream from the Internet resource
                {
                    using (var fs = new FileStream(_destination, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))// Initializes a new instance of the FileStream class with the specified path, creation mode, read/write permission and share permission
                    {
                        while (_allowedToRun)
                        {
                            var buffer = new byte[_chunkSize];
                            var bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);

                            if (bytesRead == 0) break;

                            await fs.WriteAsync(buffer, 0, bytesRead);
                            BytesWritten += bytesRead;
                        }
                        await fs.FlushAsync();
                    }
                }
            }

        }
        public Task Start()
        {
            _allowedToRun = true;
            return Start(BytesWritten);
        }
        public void Pause()
        {
            _allowedToRun = false;
        }
    }
}
