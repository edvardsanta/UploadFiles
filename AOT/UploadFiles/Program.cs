using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using InfluxDB.Client;
using MassTransit;
using Microsoft.AspNetCore.Antiforgery;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using UploadFiles.Configurations;
using UploadFiles.Internal;
using UploadFiles.Services.Services.Upload;
using UploadFiles.Services.Services.Upload.Models;
using UploadFiles.Shared.Contracts;

var builder = WebApplication.CreateSlimBuilder(args);

ServicesConfiguration.ConfigureServices(builder.Services);
builder.Services.AddCors(CorsConfiguration.ConfigureCors);
builder.Services.AddAntiforgery(AntiforgeryConfiguration.ConfigureAntiforgery);

WebApplication app = builder.Build();
app.MapGet("/auth/antiforgerytoken", async (IAntiforgery antiforgery, HttpContext httpContext) =>
{
    var tokens = antiforgery.GetAndStoreTokens(httpContext);
    httpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions { HttpOnly = false });
    return Results.Ok(new { token = tokens.RequestToken });
});

RouteGroupBuilder uploadMap = app.MapGroup("/upload");

uploadMap.MapPost("/", async (HttpContext httpContext, IFormFileCollection file, UploadManager uploadManager, IAntiforgery antiforgery) =>
{
    try
    {
        await antiforgery.ValidateRequestAsync(httpContext);

        if (file == null || file.Count() == 0)
        {
            return Results.BadRequest("No file uploaded or file is empty.");
        }

        IList<FileUploadResult>? fileuploadResult  = new List<FileUploadResult>();

        foreach(var item in file)
        {
            fileuploadResult.Add(await uploadManager.HandleUploadAsync(item));
        }

        return Results.Ok(fileuploadResult);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});


RouteGroupBuilder loginMap = app.MapGroup("/login");

RouteGroupBuilder statusMap = app.MapGroup("/status");
statusMap.MapGet("/{referenceId}", (string referenceId, IInfluxDBClient i) =>
{
    var status = GetStatusFromReferenceId(referenceId); 

    if (status == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(status);
});

object? GetStatusFromReferenceId(string referenceId)
{
    throw new NotImplementedException();
}
app.UseCors("AllowSpecificOrigin");
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await new WSHandler().HandleWebSocketAsync(webSocket);
            
            var idSocket = WebSocketConnectionManager.AddSocket(webSocket);

            await WebSocketConnectionManager.WaitForSocketToClose(webSocket);

            WebSocketConnectionManager.RemoveSocket(idSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});
app.UseAntiforgery();
app.Run();

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(NormalizeTextMessage))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
