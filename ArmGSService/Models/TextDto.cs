namespace ArmGSService.Models;

/// <summary>
/// Объект передачи данных для отправки сообщения в чат
/// </summary>
public class TextDto
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
    /// Текст сообщения. Можно упомянуть пользователя, добавив в текст его userId в следующем формате @[userId].
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Режим обработки форматирования из текста сообщения
    /// </summary>
    public ParseMode? ParseMode { get; set; }
}