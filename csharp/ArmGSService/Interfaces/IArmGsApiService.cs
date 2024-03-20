using ArmGSService.Models;

namespace ArmGSService.Interfaces;

public interface IArmGsApiService
{
    /// <summary>
    /// Провести валидацию токена бота
    /// </summary>
    /// <param name="token">Токен</param>
    /// <returns>Объект ответа с API</returns>
    Task<TokenValidationVm> ValidateToken(string token);

    /// <summary>
    /// Отправить сообщение (текст) в чат
    /// </summary>
    /// <param name="dto">Объект передачи данных</param>
    /// <returns>Объект ответа с API на команду отправки сообщения</returns>
    Task<MessageVm> SendText(TextDto dto);

    /// <summary>
    /// Отправить сообщение (файл с или без подписи)
    /// </summary>
    /// <param name="dto">Объект передачи данных</param>
    /// <param name="file">Файл</param>
    /// <returns>Объект ответа с API на команду отправки файла</returns>
    Task<MessageVm> SendFile(FileTextDto dto, IFormFile file);
}