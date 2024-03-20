using ArmGSService.Interfaces;
using ArmGSService.Models;
using ArmGSService.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IArmGsApiService, ArmGsApiService>();
builder.Services.AddScoped<IWebRequestService, WebRequestService>();
builder.Services.AddTransient<ILogger>(s => s.GetRequiredService<ILogger<Program>>());
builder.Services.AddLogging(b => b.AddConsole());

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/validateToken", (string token, IArmGsApiService service)
    => service.ValidateToken(token));

app.MapPost("/sendText", (TextDto dto, IArmGsApiService service)
    => service.SendText(dto));

app.MapPost("/sendFile", ([FromForm] FileTextDto dto, IFormFile file, IArmGsApiService service)
    => service.SendFile(dto, file)).DisableAntiforgery();

app.Run();