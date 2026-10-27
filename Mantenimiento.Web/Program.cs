
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Mantenimiento.Web.Components;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

// ==========================================
// 1. CREAR LA APLICACIÓN
// ==========================================

var builder = WebApplication.CreateBuilder(args);


// ==========================================
// 2. CONFIGURAR BLAZOR
// ==========================================

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();


// ==========================================
// 3. CONFIGURAR CONEXIÓN CON LA API
// ==========================================

builder.Services.AddHttpClient(
    "MantenimientoApi",
    client =>
    {
        var baseUrl = builder.Configuration["Api:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "Falta configurar Api:BaseUrl."
            );
        }

        client.BaseAddress = new Uri(
            baseUrl.TrimEnd('/') + "/"
        );

        client.Timeout = TimeSpan.FromSeconds(30);
    }
);


// ==========================================
// 4. CONFIGURAR AUTENTICACIÓN CON COOKIES
// ==========================================

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme
    )
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/login";

        opciones.AccessDeniedPath = "/login";

        opciones.Cookie.Name = "Mantenimiento.Auth";

        opciones.Cookie.HttpOnly = true;

        opciones.Cookie.SameSite = SameSiteMode.Lax;

        opciones.Cookie.SecurePolicy =
            CookieSecurePolicy.SameAsRequest;

        opciones.ExpireTimeSpan = TimeSpan.FromHours(2);

        opciones.SlidingExpiration = true;
    });


// ==========================================
// 5. CONFIGURAR AUTORIZACIÓN
// ==========================================

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();


// ==========================================
// 6. CONSTRUIR LA APLICACIÓN
// ==========================================

// IMPORTANTE:
// Todos los builder.Services deben estar
// registrados antes de esta línea.

var app = builder.Build();


// ==========================================
// 7. CONFIGURAR MIDDLEWARE
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true
    );

    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true
);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseAntiforgery();


// ==========================================
// 8. CONFIGURAR ARCHIVOS Y RAZOR
// ==========================================

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// ==========================================
// 9. INICIAR SESIÓN
// ==========================================

app.MapPost("/auth/login", async (
    HttpContext contexto,
    IConfiguration configuracion,
    IAntiforgery antiforgery) =>
{
    // VALIDAR TOKEN ANTIFORGERY

    try
    {
        await antiforgery.ValidateRequestAsync(contexto);
    }
    catch (AntiforgeryValidationException)
    {
        return Results.BadRequest(
            "Solicitud no válida."
        );
    }

    // OBTENER DATOS DEL FORMULARIO

    var formulario =
        await contexto.Request.ReadFormAsync();

    var usuario = formulario["usuario"]
        .ToString()
        .Trim();

    var contrasena = formulario["contrasena"]
        .ToString();

    var retorno = formulario["returnUrl"]
        .ToString();

    if (!EsRetornoLocal(retorno))
    {
        retorno = "/";
    }

    // OBTENER CREDENCIALES CONFIGURADAS

    var usuarioConfigurado =
        configuracion["WebAuth:Username"];

    var contrasenaConfigurada =
        configuracion["WebAuth:Password"];

    if (string.IsNullOrWhiteSpace(usuarioConfigurado) ||
        string.IsNullOrEmpty(contrasenaConfigurada))
    {
        return Results.Problem(
            "No se han configurado las credenciales del sistema.",
            statusCode: 503
        );
    }

    // COMPARAR CREDENCIALES

    static bool Comparar(
        string recibido,
        string esperado)
    {
        var a = SHA256.HashData(
            Encoding.UTF8.GetBytes(recibido)
        );

        var b = SHA256.HashData(
            Encoding.UTF8.GetBytes(esperado)
        );

        return CryptographicOperations.FixedTimeEquals(
            a,
            b
        );
    }

    static bool EsRetornoLocal(string? url)
    {
        if (string.IsNullOrWhiteSpace(url) ||
            !url.StartsWith('/') ||
            url.StartsWith("//", StringComparison.Ordinal) ||
            url.StartsWith("/\\", StringComparison.Ordinal) ||
            url.Contains('\\') ||
            url.Contains("://", StringComparison.Ordinal) ||
            url.Contains('\r') ||
            url.Contains('\n') ||
            url.StartsWith("/login", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("/auth/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    bool usuarioValido = Comparar(
        usuario,
        usuarioConfigurado
    );

    bool contrasenaValida = Comparar(
        contrasena,
        contrasenaConfigurada
    );

    if (!usuarioValido || !contrasenaValida)
    {
        var destinoError = retorno == "/"
            ? "/login?error=1"
            : "/login?error=1&returnUrl=" + Uri.EscapeDataString(retorno);

        return Results.Redirect(destinoError);
    }

    // CREAR IDENTIDAD AUTENTICADA

    var identidad = new ClaimsIdentity(
        new[]
        {
            new Claim(
                ClaimTypes.Name,
                usuario
            ),

            new Claim(
                ClaimTypes.Role,
                "Administrador"
            )
        },
        CookieAuthenticationDefaults.AuthenticationScheme
    );

    var principal = new ClaimsPrincipal(
        identidad
    );

    // GENERAR COOKIE DE SESIÓN

    await contexto.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal
    );

    // REDIRIGIR A LA PÁGINA SOLICITADA O AL INICIO

    return Results.Redirect(retorno);

}).AllowAnonymous();


// ==========================================
// 10. CERRAR SESIÓN
// ==========================================

app.MapPost("/auth/logout", async (
    HttpContext contexto,
    IAntiforgery antiforgery) =>
{
    try
    {
        await antiforgery.ValidateRequestAsync(contexto);
    }
    catch (AntiforgeryValidationException)
    {
        return Results.BadRequest(
            "Solicitud no válida."
        );
    }

    // ELIMINAR COOKIE DE AUTENTICACIÓN

    await contexto.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme
    );

    // VOLVER AL LOGIN

    return Results.Redirect("/login");

}).RequireAuthorization();


// ==========================================
// 11. EJECUTAR LA APLICACIÓN
// ==========================================

app.Run();
