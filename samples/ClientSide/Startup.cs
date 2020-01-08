using System;
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
            services.AddOidc(new Uri("https://demo.identityserver.io"), (settings, siteUri) =>
            {
                settings.UseDefaultCallbackUris(siteUri);
                settings.UseRedirectToCallerAfterAuthenticationRedirect();
                settings.UseRedirectToCallerAfterLogoutRedirect();
                settings.UseDemoFlow().Code(); // Just for this demo: allows to quickly change to one of the supported flows
                settings.Scope = "openid profile email api";

                settings.MinimumLogeLevel = LogLevel.Information;
                settings.StorageType = StorageType.SessionStorage;
                settings.InteractionType = InteractionType.Popup;
            });

            services.AddToaster(c => c.PositionClass = Defaults.Classes.Position.BottomRight);
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
