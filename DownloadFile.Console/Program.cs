
using HttpDownloader;

Uri link = new Uri(@"https://github.com/Under4groos/SmdCompile.View/releases/download/1.3.3.2/SMDCompile.zip");
using (HttpFile file = new HttpFile())
{
    file.DownloadSuccessfully += (o, e) =>
    {
        Console.WriteLine(e.RequestUri);
        Console.WriteLine(e.DestinationPath);
    };
    file.DownloadProgressChanged += (o, e) =>
    {
        Console.WriteLine(e.ProgressPercentage);
    };
    file.DownloadException += (o, e) =>
    {
        Console.WriteLine($"[Error]: {e.Message}");
    };
    await file.DwinloadAsync(link, "SMDCompile.zip");
}