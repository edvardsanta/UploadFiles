using Microsoft.AspNetCore.Antiforgery;

namespace UploadFiles.Configurations
{
    public static class AntiforgeryConfiguration
    {
        public static void ConfigureAntiforgery(AntiforgeryOptions options)
        {
            options.HeaderName = "X-XSRF-TOKEN";
        }
    }
}
