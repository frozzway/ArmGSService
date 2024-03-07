namespace ArmGSService.Models;

/// <summary>
/// Ответ с API на команду отправки сообщения
/// </summary>
public class MessageVm
{
    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public string? MsgId { get; set; }
    
    /// <summary>
    /// Статус запроса
    /// </summary>
    public bool Ok { get; set; }
}