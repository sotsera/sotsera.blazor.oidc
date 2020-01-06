// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;

namespace Sotsera.Blazor.Oidc
{
    public class OidcSettings
    {
        #region Client

        public string Issuer { get; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ResponseType { get; set; } = "code";
        public string Scope { get; set; } = "openid profile";
        public Prompt? Prompt { get; set; }
        public Display? Display { get; set; }
        public int? MaxAge { get; set; }
        public string UiLocales { get; set; }
        public string AcrValues { get; set; }
        public string ResponseMode { get; set; }
        public string Resource { get; set; }
        public NameValueCollection AdditionalParameters { get; } = new NameValueCollection();
        public NameValueCollection AuthenticationStateData { get; } = new NameValueCollection();
        public NameValueCollection LogoutStateData { get; } = new NameValueCollection();

        #endregion

        #region Behaviour

        // Protocol
        public InteractionType InteractionType { get; set; } = InteractionType.Popup;

        // Client
        public bool FilterProtocolClaims { get; set; } = true;
        public bool LoadUserInfo { get; set; } = true;

        // Claims
        public string NameClaimKey { get; set; } = Consts.ClaimNames.Name;
        public string RoleClaimKey { get; set; } = Consts.ClaimNames.Role;

        // Timeouts
        public TimeSpan HttpClientTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan OpenPopupTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan CheckSessionTimeout { get; set; } = TimeSpan.FromSeconds(5);

        // Storage
        public string StoragePrefix { get; set; } = "oidc";
        public StorageType StorageType { get; set; } = StorageType.SessionStorage;

        // Popup 
        public string PopupWindowName { get; set; } = "_blank";
        public string PopupWindowFeatures { get; set; } = "location=no,toolbar=no,width=500,height=500,left=100,top=100;";

        // Tokens
        public int MaxTokenSize { get; set; } = 1024 * 1024 * 2; // 2 Mb
        public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);
        public bool RevokeAccessTokenOnSignout { get; set; } = false;

        // Session Monitor

        public bool MonitorSession { get; set; } = true;
        public int CheckSessionInterval { get; set; } = 2000;
        public bool CheckSessionStopOnError { get; set; } = true;
        
        // Logging
        public LogLevel MinimumLogeLevel { get; set; } = LogLevel.Warning;

        #endregion

        #region Metadata

        public string MetadataEndpoint { get; set; }
        public OpenidEndpoints Endpoints { get; set; } = new OpenidEndpoints();
        public Jwks Jwks { get; set; }

        #endregion

        #region Callback Uris

        public string AuthenticationRedirectCallbackUri { get; set; }
        public string AuthenticationPopupCallbackUri { get; set; }
        public string LogoutRedirectCallbackUri { get; set; }
        public string LogoutPopupCallbackUri { get; set; }
        public string SilentRenewCallbackUri { get; set; }

        #endregion

        #region actions

        public Func<OidcUser, NameValueCollection, IServiceProvider, Task> PostAuthenticationRedirect;
        public Func<OidcUser, NameValueCollection, IServiceProvider, Task> PostAuthenticationPopup;
        public Func<OidcUser, NameValueCollection, IServiceProvider, Task> PostLogoutRedirect;
        public Func<OidcUser, NameValueCollection, IServiceProvider, Task> PostLogoutPopup;

        #endregion

        public OidcSettings(Uri issuer)
        {
            if (issuer == null) throw new ArgumentNullException(nameof(issuer));

            Issuer = issuer.AbsoluteUri;
            MetadataEndpoint = $"{Issuer}.well-known/openid-configuration";
        }
    }

    public static class OidcSettingsExtensions
    {
        public static OidcSettings UseDefaultCallbackUris(this OidcSettings settings, Uri baseCallbackUri)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (baseCallbackUri == null) throw new ArgumentNullException(nameof(baseCallbackUri));

            const string routePath = "oidc/callbacks/";
            const string pagePath = "_content/Sotsera.Blazor.Oidc/";

            settings.AuthenticationRedirectCallbackUri = $"{baseCallbackUri}{routePath}authentication-redirect";
            settings.LogoutRedirectCallbackUri = $"{baseCallbackUri}{routePath}logout-redirect";
            settings.AuthenticationPopupCallbackUri = $"{baseCallbackUri}{pagePath}authentication-popup.html";
            settings.LogoutPopupCallbackUri = $"{baseCallbackUri}{pagePath}logout-popup.html";
            settings.SilentRenewCallbackUri = $"{baseCallbackUri}{pagePath}silent-renew.html";

            return settings;
        }

        public static OidcSettings UseDefaultActions(this OidcSettings settings, IServiceProvider serviceProvider)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            settings.PostAuthenticationRedirect = (user, state, provider) => RedirectToRoot(provider, nameof(OidcSettings.PostAuthenticationRedirect));
            settings.PostAuthenticationPopup = (user, state, provider) => DoNothing(provider, nameof(OidcSettings.PostAuthenticationPopup));
            settings.PostLogoutRedirect = (user, state, provider) => RedirectToRoot(provider, nameof(OidcSettings.PostLogoutRedirect));
            settings.PostLogoutPopup = (user, state, provider) => DoNothing(provider, nameof(OidcSettings.PostLogoutPopup));

            return settings;
        }

        public static Task RedirectToRoot(IServiceProvider serviceProvider, string actionName)
        {
            var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
            serviceProvider.GetRequiredService<ILogger<OidcSettings>>().LogInformation($"Default {actionName} action: redirect to root");
            navigationManager.NavigateTo(navigationManager.BaseUri);
            return Task.CompletedTask;
        }

        public static Task DoNothing(IServiceProvider serviceProvider, string actionName)
        {
            serviceProvider.GetRequiredService<ILogger<OidcSettings>>().LogInformation($"Default {actionName} action: do nothing");
            return Task.CompletedTask;
        }
    }
}