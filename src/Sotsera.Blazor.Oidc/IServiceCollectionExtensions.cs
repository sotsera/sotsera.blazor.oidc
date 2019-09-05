// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sotsera.Blazor.Oidc;
using Sotsera.Blazor.Oidc.BrowserInterop;
using Sotsera.Blazor.Oidc.Configuration;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement;
using Sotsera.Blazor.Oidc.Core.Protocol.TokenRevocation;
using Sotsera.Blazor.Oidc.Core.Storage;
using Sotsera.Blazor.Oidc.Core.Tokens;
using Sotsera.Blazor.Oidc.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddOidc(this IServiceCollection services, Uri issuer, Action<OidcSettings, Uri> configure)
        {
            services.AddAuthorizationCore();
            services.TryAddScoped<Metadata>();
            services.TryAddScoped<Interop>(); //TODO: ensure that IJSRuntime is singleton too
            services.TryAddScoped<AuthenticationStateProvider, AuthStateProvider>();
            services.TryAddScoped<OidcHttpClient>();
            services.TryAddScoped<IUserManager, UserManager>();

            services.TryAddScoped(typeof(IOidcLogger<>), typeof(OidcLogger<>)); // remove this when this will be published (https://github.com/aspnet/AspNetCore/pull/12928)
            services.TryAddScoped<IMetadataService, MetadataService>();
            services.TryAddScoped<ISignatureValidatorFactory, SignatureValidatorFactory>();
            services.TryAddScoped<IJwtValidator, JwtValidator>();
            services.TryAddScoped<ITokenParser, TokenParser>();
            services.TryAddScoped<ITokenRevocationClient, TokenRevocationClient>();
            services.TryAddScoped<IAuthRequestBuilder, AuthRequestBuilder>();
            services.TryAddScoped<IAuthResponseParser, AuthResponseParser>();
            services.TryAddScoped<IUserManagerHelper, UserManagerHelper>();
            services.TryAddScoped<IOidcClient, OidcClient>();
            services.TryAddScoped<ISessionMonitor, SessionMonitor>();
            services.TryAddScoped<ILogoutRequestBuilder, LogoutRequestBuilder>();
            services.TryAddScoped<ILogoutResponseParser, LogoutResponseParser>();
            services.TryAddScoped<ILogoutClient, LogoutClient>();
            services.TryAddScoped<IStore, Store>();

            services.TryAddScoped<IStorage>(p =>
            {
                var settings = p.GetRequiredService<OidcSettings>();
                
                if (settings.StorageType.IsMemory())
                    return new MemoryStorage();
                
                var interop = p.GetRequiredService<Interop>();
                var logger = p.GetRequiredService<IOidcLogger<BrowserStorage>>();
                return new BrowserStorage(settings, interop, logger);
            });

            services.TryAddScoped(b =>
            {
                var navigationManager = b.GetRequiredService<NavigationManager>();
                var settings = new OidcSettings(issuer);
                configure?.Invoke(settings, new Uri(navigationManager.BaseUri));
                new OidcSettingsValidator().EnsureValidSettings(services, settings);
                return settings;
            });

            return services;
        }
    }
}