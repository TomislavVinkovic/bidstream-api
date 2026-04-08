namespace BidStream.Services;

using BidStream.Services.Interface;

public class FileService : IFileService
{
    private readonly IHttpContextService _httpContextService;
    public FileService(
        IHttpContextService httpContextService
    )
    {
        _httpContextService = httpContextService;
    }

    public string? GetAbsoluteFileUrl(string? relativeUrl)
    {
        var baseUrl = _httpContextService.GetBaseUrl();
        var fullImageUrl = relativeUrl;
        if (!string.IsNullOrEmpty(relativeUrl) && relativeUrl.StartsWith("/"))
        {
            fullImageUrl = $"{baseUrl}{relativeUrl}";
        }

        return fullImageUrl;
    }

    public async Task<string> UploadAsync(Stream fileStream, string extension)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if(!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using(var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

       return $"/uploads/{uniqueFileName}";
    }

    public async Task<List<string>> UploadMultipleAsync(IEnumerable<(Stream fileStream, string extension)> files)
    {
        var uploadTasks = new List<Task<string>>();
        foreach (var file in files)
        {
            uploadTasks.Add(UploadAsync(file.fileStream, file.extension));
        }

        var uploadedUrls = await Task.WhenAll(uploadTasks);

        return uploadedUrls.ToList();
    }
}