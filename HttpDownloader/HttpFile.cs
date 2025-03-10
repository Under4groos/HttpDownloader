using System.ComponentModel;

namespace HttpDownloader
{
    public class HttpDownloadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        internal HttpDownloadProgressChangedEventArgs(int progressPercentage, object? userToken, long bytesReceived, long totalBytesToReceive) :
            base(progressPercentage, userToken)
        {
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
        }

        public long BytesReceived { get; }
        public long TotalBytesToReceive { get; }
    }


    public class HttpDownloadProgressSuccessfullyEventArgs : EventArgs
    {
        public HttpDownloadProgressSuccessfullyEventArgs(Uri requestUri, string destinationPath)
        {
            RequestUri = requestUri;
            DestinationPath = destinationPath;
        }

        public Uri RequestUri { get; }
        public string DestinationPath { get; }
    }
    public delegate void HttpDownloadProgressChangedEventHandler(object sender, HttpDownloadProgressChangedEventArgs e);
    public delegate void HttpDownloadProgressSuccessfullyEventHandler(object sender, HttpDownloadProgressSuccessfullyEventArgs e);
    public delegate void HttpDownloadExceptionEventHandler(object sender, Exception e);


    public class HttpFile : HttpClient
    {
        public event HttpDownloadProgressChangedEventHandler? DownloadProgressChanged;
        public event HttpDownloadProgressSuccessfullyEventHandler? DownloadSuccessfully;
        public event HttpDownloadExceptionEventHandler? DownloadException;



        public async Task DwinloadAsync(
            Uri? requestUri,
            string destinationPath)
        {
            try
            {

                using (HttpResponseMessage response = await GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
                {

                    response.EnsureSuccessStatusCode(); // Throw if not a success code.
                    long totalBytes = response.Content.Headers.ContentLength ?? -1;
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                   fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[8192];
                        long totalReadBytes = 0;
                        int readBytes;
                        while ((readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, readBytes);
                            totalReadBytes += readBytes;
                            if (totalBytes != -1)
                            {
                                int progressPercentage = (int)((totalReadBytes * 100) / totalBytes);

                                DownloadProgressChanged?.Invoke(this, new HttpDownloadProgressChangedEventArgs(progressPercentage, null, totalBytes, totalReadBytes));
                            }

                        }
                    }
                }
                DownloadSuccessfully?.Invoke(this, new HttpDownloadProgressSuccessfullyEventArgs(requestUri, Path.GetFullPath(destinationPath)));

            }
            catch (Exception ex)
            {
                DownloadException?.Invoke(this, ex);

            }
        }
    }
}
