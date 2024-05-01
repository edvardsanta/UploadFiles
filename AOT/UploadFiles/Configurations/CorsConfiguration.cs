using Microsoft.AspNetCore.Cors.Infrastructure;

namespace UploadFiles.Configurations
{
    public static class CorsConfiguration
    {
        public static void ConfigureCors(CorsOptions options)
        {
            options.AddPolicy("AllowSpecificOrigin", builder =>
            {
                builder.SetIsOriginAllowed(origin =>
                {
                    var isLocalhost = new Uri(origin).Host.Equals("localhost", StringComparison.OrdinalIgnoreCase);
                    return isLocalhost;
                })
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        }
    }
}
