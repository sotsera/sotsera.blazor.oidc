// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;
using Sotsera.Blazor.Oidc.Utilities;

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
        public Dictionary<string, string> AuthenticationStateData { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> LogoutStateData { get; } = new Dictionary<string, string>();

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

        public Func<AuthParameters, IServiceProvider, Task> PreAuthentication;
        public Func<LogoutParameters, IServiceProvider, Task> PreLogout;

        public Func<OidcUser, Dictionary<string, string>, IServiceProvider, Task> PostAuthenticationRedirect;
        public Func<OidcUser, Dictionary<string, string>, IServiceProvider, Task> PostAuthenticationPopup;
        public Func<OidcUser, Dictionary<string, string>, IServiceProvider, Task> PostLogoutRedirect;
        public Func<OidcUser, Dictionary<string, string>, IServiceProvider, Task> PostLogoutPopup;

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

        public static OidcSettings UseDefaultActions(this OidcSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            settings.PreAuthentication = (parameters, provider) => DoNothing(provider);
            settings.PreLogout = (parameters, provider) => DoNothing(provider);

            settings.PostAuthenticationRedirect = (user, state, provider) => NavigateToRoot(provider);
            settings.PostAuthenticationPopup = (user, state, provider) => DoNothing(provider);
            settings.PostLogoutRedirect = (user, state, provider) => NavigateToRoot(provider);
            settings.PostLogoutPopup = (user, state, provider) => DoNothing(provider);

            return settings;
        }

        public static OidcSettings UseRedirectToCallerActionsForAuthenticationRedirect(this OidcSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            settings.PreAuthentication = (parameters, serviceProvider) => RegisterCurrentPage(parameters.StateData, serviceProvider);
            settings.PostAuthenticationRedirect = (user, stateData, serviceProvider) => NavigateToCallerPage(stateData, serviceProvider);
            return settings;
        }

        public static OidcSettings UseRedirectToCallerActionsForLogoutRedirect(this OidcSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            settings.PreLogout = (parameters, serviceProvider) => RegisterCurrentPage(parameters.StateData, serviceProvider);
            settings.PostLogoutRedirect = (user, stateData, serviceProvider) => NavigateToCallerPage(stateData, serviceProvider);
            return settings;
        }

        public static Task NavigateToRoot(IServiceProvider serviceProvider)
        {
            var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
            serviceProvider.GetRequiredService<ILogger<OidcSettings>>().LogInformation($"Action {nameof(NavigateToRoot)}");
            navigationManager.NavigateTo(navigationManager.BaseUri);
            return Task.CompletedTask;
        }

        public static Task NavigateToCallerPage(Dictionary<string, string> stateData, IServiceProvider serviceProvider)
        {
            var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
            var logger = serviceProvider.GetRequiredService<ILogger<OidcSettings>>();

            var redirectPage = stateData?["redirectPage"];

            if (string.IsNullOrWhiteSpace(redirectPage))
            {
                logger.LogWarning($"Action {nameof(NavigateToCallerPage)}: redirectPage url is missing. Fallback to root.");
                NavigateToRoot(serviceProvider);
                return Task.CompletedTask;
            }

            logger.LogInformation($"Action {nameof(NavigateToCallerPage)}: navigate to {redirectPage}");
            navigationManager.NavigateTo(redirectPage);
            return Task.CompletedTask;
        }

        public static Task DoNothing(IServiceProvider serviceProvider)
        {
            serviceProvider.GetRequiredService<ILogger<OidcSettings>>().LogInformation($"Action {nameof(DoNothing)}");
            return Task.CompletedTask;
        }

        public static Task RegisterCurrentPage(Dictionary<string, string> stateData, IServiceProvider serviceProvider)
        {
            var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
            var pageUrl = navigationManager.ToBaseRelativePath(navigationManager.Uri);

            serviceProvider.GetRequiredService<ILogger<OidcSettings>>().LogDebug($"Registering the current page redirect url: {pageUrl}");

            stateData["redirectPage"] = pageUrl;
            return Task.CompletedTask;
        }
    }
}