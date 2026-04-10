namespace BidStream.Services.Interface;

public interface IFileService
{
    public Task<string> UploadAsync(Stream fileStream, string extension);
    public Task<List<string>> UploadMultipleAsync(IEnumerable<(Stream fileStream, string extension)> files);
    public Task<bool> DeleteFile(string relativeUrl);
    public string? GetAbsoluteFileUrl(string? relativeUrl);
}