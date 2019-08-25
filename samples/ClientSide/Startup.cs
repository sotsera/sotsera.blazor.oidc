using System;
using Microsoft.AspNetCore.Blazor.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Toaster.Core.Models;

namespace ClientSide
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var issuerUri = new Uri("https://demo.identityserver.io");
            var baseUri = new Uri(WebAssemblyUriHelper.Instance.GetBaseUri());

            services.AddOidc(OidcSample.CodeWithShortLivedToken(new OidcSettings(issuerUri, baseUri)
            {
                Scope = "openid profile email api",
                MinimumLogeLevel = LogLevel.Warning,
                StorageType = StorageType.SessionStorage,
                InteractionType = InteractionType.Popup
            }));
            services.AddToaster(c => c.PositionClass = Defaults.Classes.Position.BottomRight);
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
