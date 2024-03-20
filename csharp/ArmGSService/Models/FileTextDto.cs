namespace ArmGSService.Models;

/// <summary>
/// Объект передачи данных для отправки файла в чат
/// </summary>
public class FileTextDto
{
    /// <summary>
    /// Токен бота
    /// </summary>
    public string Token { get; set; }
    
    /// <summary>
    /// Уникальный ник или id чата или пользователя. 
    /// </summary>
    public string ChatId { get; set; }
    
    /// <summary>
    /// Текст к прикладываемому файлу. Можно упомянуть пользователя, добавив в текст его userId в следующем формате @[userId].
    /// </summary>
    public string? Caption { get; set; }
    
    /// <summary>
    /// Режим обработки форматирования из текста сообщения
    /// </summary>
    public ParseMode? ParseMode { get; set; }
}