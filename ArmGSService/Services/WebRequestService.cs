using System.Net.Mime;
using System.Text;
using System.Web;
using ArmGSService.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ArmGSService.Services;

public class WebRequestService: IWebRequestService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger _logger;
    
    public WebRequestService(IHttpClientFactory httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<TResponse> SendFileRequestWithResponse<TQuery, TResponse>(string url, TQuery queryParams, IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClient.CreateClient();
        client.DefaultRequestHeaders.Clear();
        
        var uriBuilder = new UriBuilder(url)
        {
            Query = ToQueryString(queryParams)
        };
        
        using var multipartFormContent = new MultipartFormDataContent();

        var fileStreamContent = new StreamContent(file.OpenReadStream());
        multipartFormContent.Add(fileStreamContent, name: "file", fileName: file.Name);
        
        var apiResponse = await client.PostAsync(uriBuilder.Uri, multipartFormContent, cancellationToken);
        
        if (apiResponse.IsSuccessStatusCode) 
            return await GetWebResponse<TResponse>(apiResponse, cancellationToken);
        
        var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);
        
        LogException(url, apiContent);
        throw new BadHttpRequestException("Произошла ошибка во время выполнения запроса");
    }

    public async Task<TResponse> SendRequestWithResponse<TQuery, TResponse>(string url, HttpMethod method, TQuery queryParams, object? data = null,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClient.CreateClient();
        client.DefaultRequestHeaders.Clear();
        
        var message = GetMessage(url, method, queryParams, data);
        var apiResponse = await client.SendAsync(message, cancellationToken);
        
        if (apiResponse.IsSuccessStatusCode) 
            return await GetWebResponse<TResponse>(apiResponse, cancellationToken);
        
        var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);
        
        LogException(url, apiContent);
        throw new BadHttpRequestException("Произошла ошибка во время выполнения запроса");
    }
    
    public async Task<TResponse> SendRequestWithResponse<TResponse>(string url, HttpMethod method, object? data = null,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClient.CreateClient();
        client.DefaultRequestHeaders.Clear();
        
        var message = GetMessage(url, method, data);
        var apiResponse = await client.SendAsync(message, cancellationToken);
        
        if (apiResponse.IsSuccessStatusCode) 
            return await GetWebResponse<TResponse>(apiResponse, cancellationToken);
        
        var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);
        
        LogException(url, apiContent);
        throw new BadHttpRequestException("Произошла ошибка во время выполнения запроса");
    }

    private static HttpRequestMessage GetMessage<TQuery>(string url, HttpMethod method, TQuery queryParams, object? data = null)
    {
        var request = new HttpRequestMessage(method, url);
        var uriBuilder = new UriBuilder(url)
        {
            Query = ToQueryString(queryParams)
        };
        request.RequestUri = uriBuilder.Uri;
        if (data is not null)
            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                MediaTypeNames.Application.Json);
        return request;
    }
    
    private static HttpRequestMessage GetMessage(string url, HttpMethod method, object? data = null)
    {
        var request = new HttpRequestMessage(method, url);
        var uriBuilder = new UriBuilder(url);
        request.RequestUri = uriBuilder.Uri;
        if (data is not null)
            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                MediaTypeNames.Application.Json);
        return request;
    }
    
    private static string ToQueryString<T>(T obj)
    {
        string jsonString = JsonConvert.SerializeObject(obj);
        var jsonObject = JObject.Parse(jsonString);
        var properties = jsonObject
            .Properties()
            .Where(p => p.Value.Type != JTokenType.Null)
            .Select(p =>
                $"{HttpUtility.UrlEncode(p.Name)}={HttpUtility.UrlEncode(p.Value.ToString())}");
        return string.Join("&", properties);
    }
    
    private async Task<T> GetWebResponse<T>(HttpResponseMessage apiResponse,
        CancellationToken cancellationToken)
    {
        try
        {
            var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);
            var deserialized = JsonSerializer.Deserialize<T>(apiContent);
            if (deserialized is null)
                throw new BadHttpRequestException("Отсутствует тело ответа");
            return deserialized;
        }
        catch
        {
            _logger.LogError($"Ошибка получения данных, при конвертации в объект {typeof(T)}");
            throw new BadHttpRequestException("Ошибка конвертации данных");
        }
    }
    
    private void LogException(string url, string apiContent)
    {
        _logger.LogError($"Ошибка при обращении по ссылке {url}. Ответ от сервера " +
                         $"{string.Concat(apiContent.AsSpan(0, 1000), "...")}");
    }
}