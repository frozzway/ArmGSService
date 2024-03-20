namespace ArmGSService.Interfaces;

public interface IWebRequestService
{
    /// <summary>
    /// Метод отправки запроса с получением ответа в виде сущности
    /// </summary>
    /// <param name="url">Ссылка</param>
    /// <param name="method">Http метод</param>
    /// <param name="queryParams">Query параметры</param>
    /// <param name="data">Данные, помещаемые в тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <typeparam name="TQuery">Класс сущности запроса</typeparam>
    /// <typeparam name="TResponse">Класс сущности ответа</typeparam>
    /// <returns>Ответ на веб запрос</returns>
    Task<TResponse> SendRequestWithResponse<TQuery, TResponse>(string url, HttpMethod method, TQuery queryParams,
        object? data = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Метод отправки запроса multipart/form-data с файлом и получением ответа в виде сущности
    /// </summary>
    /// <param name="url">Ссылка</param>
    /// <param name="method">Http метод</param>
    /// <param name="file">Файл</param>
    /// <param name="queryParams">Query параметры</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <typeparam name="TQuery">Класс сущности запроса</typeparam>
    /// <typeparam name="TResponse">Класс сущности ответа</typeparam>
    /// <returns>Ответ на веб запрос</returns>
    Task<TResponse> SendFileRequestWithResponse<TQuery, TResponse>(string url, TQuery queryParams,
        IFormFile file, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Метод отправки запроса с получением ответа в виде сущности
    /// </summary>
    /// <param name="url">Ссылка</param>
    /// <param name="method">Http метод</param>
    /// <param name="data">Данные, помещаемые в тело запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <typeparam name="TResponse">Класс сущности ответа</typeparam>
    /// <returns>Ответ на веб запрос</returns>
    Task<TResponse> SendRequestWithResponse<TResponse>(string url, HttpMethod method, object? data = null,
        CancellationToken cancellationToken = default);
}