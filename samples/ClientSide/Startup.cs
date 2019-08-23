using System;
using Microsoft.AspNetCore.Blazor.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc;
using Sotsera.Blazor.Oidc.Configuration.Model;

namespace ClientSide
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var baseUri = new Uri(WebAssemblyUriHelper.Instance.GetBaseUri());
            var identityserver = new Uri("https://demo.identityserver.io");

            services.AddOidc(OidcSample.Code(new OidcSettings(identityserver, baseUri)
            {
                Scope = "openid profile email api",
                MinimumLogeLevel = LogLevel.Warning,
                StorageType = StorageType.SessionStorage,
                InteractionType = InteractionType.Popup
            }));
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
