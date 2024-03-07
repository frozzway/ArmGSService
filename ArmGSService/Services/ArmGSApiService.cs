using ArmGSService.Interfaces;
using ArmGSService.Models;

namespace ArmGSService.Services;

/// <summary>
/// Сервис для взаимодействия с ArmGS API
/// </summary>
public class ArmGsApiService(IConfiguration configuration, IWebRequestService webRequestService): IArmGsApiService
{
    private readonly string _apiUrl = configuration["apiUrl"]!;

    public Task<TokenValidationVm> ValidateToken(string token)
        => webRequestService.SendRequestWithResponse<TokenValidationVm>($"{_apiUrl}/self/get?token={token}", HttpMethod.Get);

    public Task<MessageVm> SendText(TextDto dto)
        => webRequestService.SendRequestWithResponse<TextDto, MessageVm>($"{_apiUrl}/messages/sendText", HttpMethod.Get, dto);

    public Task<MessageVm> SendFile(FileTextDto dto, IFormFile file)
        => webRequestService.SendFileRequestWithResponse<FileTextDto, MessageVm>($"{_apiUrl}/messages/sendFile", dto, file);
}