using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using ArmGSService.Extensions;
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

    private HttpClient GetHttpClient()
    {
        var client = _httpClient.CreateClient();
        client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.36.1");
        client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        client.DefaultRequestHeaders.Add("Postman-Token", "0514f097-ee5b-4d1d-998a-0b49d3bd07aa");
        return client;
    }
    
    public async Task<TResponse> SendFileRequestWithResponse<TQuery, TResponse>(string url, TQuery queryParams, IFormFile file,
        CancellationToken cancellationToken = default)
    {
         var client = GetHttpClient();
        
         var uriBuilder = new UriBuilder(url)
         {
             Query = ToQueryString(queryParams)
         };
        
         using var multipartFormContent = new MultipartFormDataContent();
        
         var fileContent = new StreamContent(file.OpenReadStream());
         fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
         fileContent.Headers.TryAddWithoutValidation("Content-Disposition", $"form-data; name=\"file\"; filename=\"{file.FileName}\"");
         multipartFormContent.Add(fileContent);
         multipartFormContent.Headers.ContentDisposition = null;
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
        var client = GetHttpClient();
        
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
        var client = GetHttpClient();
        
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
        var opts = new JsonSerializerOptions();
        opts.Converters.Add(new JsonStringEnumConverter());
        var jsonString = JsonSerializer.Serialize(obj, opts);
        var jsonObject = JObject.Parse(jsonString);
        var properties = jsonObject
            .Properties()
            .Where(p => p.Value.Type != JTokenType.Null)
            .Select(p =>
                $"{p.Name.ToString().ReplaceFirstCharToLowercase()}={HttpUtility.UrlEncode(p.Value.ToString())}");
        return string.Join("&", properties);
    }
    
    private async Task<T> GetWebResponse<T>(HttpResponseMessage apiResponse,
        CancellationToken cancellationToken)
    {
        try
        {
            var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);
            var deserialized = JsonSerializer.Deserialize<T>(apiContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                         $"{apiContent}");
    }
}