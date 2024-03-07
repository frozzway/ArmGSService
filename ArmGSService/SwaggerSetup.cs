using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ArmGSService;

public static class SwaggerSetup
{
    /// <summary>
    /// Метод добавления конфигурация для swagger
    /// </summary>
    /// <param name="configuration">Интерфейс конфигурации</param>
    /// <param name="services">Интерфейс для подключения сервисов к сборке</param>
    public static void AddSwagger(IConfiguration configuration, IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            SetupSwaggerDocs(option);
        });
    }

    /// <summary>
    /// Настройка определения путей для xml документации
    /// </summary>
    /// <param name="swaggerGenOptions"></param>
    private static void SetupSwaggerDocs(SwaggerGenOptions swaggerGenOptions)
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var xmlFile = $"{assemblyName}.xml";
        var baseDirectory = AppContext.BaseDirectory;
        var baseDirectoryApplication = baseDirectory.Replace(assemblyName, "Application");
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        var xmlPathApplication = Path.Combine(baseDirectoryApplication, xmlFile);
        swaggerGenOptions.IncludeXmlComments(xmlPath);
        swaggerGenOptions.IncludeXmlComments(xmlPathApplication);
        swaggerGenOptions.CustomSchemaIds(type => type.FullName);
    }
}