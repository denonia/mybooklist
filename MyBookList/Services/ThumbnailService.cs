namespace MyBookList.Services;

public class ThumbnailService
{
    private const string ThumbnailUrl = "https://covers.openlibrary.org/b/id/{0}-M.jpg";
    private const string DefaultUrl = "thumb/default.jpg";

    private readonly IWebHostEnvironment _environment;

    public ThumbnailService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> GetBookThumbnailUrlAsync(int? bookId)
    {
        if (bookId is null or -1)
            return DefaultUrl;

        var wwwroot = _environment.WebRootPath;
        var path = Path.Combine(wwwroot, "thumb", bookId + ".jpg");

        if (File.Exists(path))
            return $"thumb/{bookId}.jpg";

        var downloadUrl = string.Format(ThumbnailUrl, bookId);

        try
        {
            Task.Run(async () =>
            {
                using var client = new HttpClient();
                await using var stream = await client.GetStreamAsync(downloadUrl);
                await using var fs = new FileStream(path, FileMode.CreateNew);
                await stream.CopyToAsync(fs);
            }).ConfigureAwait(false);

            return downloadUrl;
        }
        catch (Exception e)
        {
            return DefaultUrl;
        }
    }
}