namespace ArmGSService.Models;

/// <summary>
/// Ответ с API на команду валидации токена
/// </summary>
public class TokenValidationVm
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public string UserId { get; set; }
    
    /// <summary>
    /// Уникальный ник
    /// </summary>
    public string Nick { get; set; }
    
    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    /// Описание бота
    /// </summary>
    public string About { get; set; }
    
    /// <summary>
    /// Статус запроса
    /// </summary>
    public bool Ok { get; set; }
}