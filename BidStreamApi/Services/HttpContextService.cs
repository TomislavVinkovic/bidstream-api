namespace BidStream.Services;

using BidStream.Services.Interface;

public class HttpContextService : IHttpContextService
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public HttpContextService(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }
    public string GetBaseUrl()
    {
        var context = _httpContextAccessor.HttpContext;
    
        if (context == null)
        {
            return _configuration["AppConfig:BaseUrl"]!; 
        }

        var request = context.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}