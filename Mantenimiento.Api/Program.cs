using Mantenimiento.Application.Interfaces;
using Mantenimiento.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient(
    "MantenimientoApi",
    client =>
    {
        var baseUrl =
            builder.Configuration["Api:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "Falta configurar Api:BaseUrl."
            );
        }

        client.BaseAddress = new Uri(baseUrl);

        client.Timeout = TimeSpan.FromSeconds(30);
    }
);


builder.Services.AddHttpClient<IEquipoService, EquipoService>(
    (provider, client) =>
    {
        var configuration =
            provider.GetRequiredService<IConfiguration>();

        var baseUrl = configuration["N8n:BaseUrl"];

        var apiKey = configuration["N8n:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "Falta configurar N8n:BaseUrl."
            );
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Falta configurar N8n:ApiKey."
            );
        }

        client.BaseAddress = new Uri(
            baseUrl.TrimEnd('/') + "/"
        );

        client.DefaultRequestHeaders.Add(
            "x-api-key",
            apiKey
        );

        client.Timeout = TimeSpan.FromSeconds(30);
    }
);

builder.Services.AddHttpClient<IFallaService, FallaService>(
    (provider, client) =>
    {
        var configuration =
            provider.GetRequiredService<IConfiguration>();

        var baseUrl = configuration["N8n:BaseUrl"];
        var apiKey = configuration["N8n:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl) ||
            string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Falta configurar la conexión con n8n."
            );
        }

        client.BaseAddress = new Uri(
            baseUrl.TrimEnd('/') + "/"
        );

        client.DefaultRequestHeaders.Add(
            "x-api-key", apiKey
        );

        client.Timeout = TimeSpan.FromSeconds(30);
    }
);

builder.Services.AddHttpClient<IMantenimientoService, MantenimientoService>(
    (provider, client) =>
    {
        var configuration =
            provider.GetRequiredService<IConfiguration>();

        var baseUrl = configuration["N8n:BaseUrl"];
        var apiKey = configuration["N8n:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "Falta configurar N8n:BaseUrl."
            );
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Falta configurar N8n:ApiKey."
            );
        }

        client.BaseAddress = new Uri(
            baseUrl.TrimEnd('/') + "/"
        );

        client.DefaultRequestHeaders.Add(
            "x-api-key",
            apiKey
        );

        client.Timeout = TimeSpan.FromSeconds(30);
    }
);


builder.Services.AddHttpClient<IReporteService, ReporteService>(
    (provider, client) =>
    {
        var configuration =
            provider.GetRequiredService<IConfiguration>();

        var baseUrl = configuration["N8n:BaseUrl"];
        var apiKey = configuration["N8n:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "Falta configurar N8n:BaseUrl."
            );
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Falta configurar N8n:ApiKey."
            );
        }

        client.BaseAddress = new Uri(
            baseUrl.TrimEnd('/') + "/"
        );

        client.DefaultRequestHeaders.Add(
            "x-api-key",
            apiKey
        );

        client.Timeout = TimeSpan.FromSeconds(30);
    }
);




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Servicios de consulta a PostgreSQL
builder.Services.AddScoped<
    IConsultaMantenimientoService,
    ConsultaMantenimientoService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
