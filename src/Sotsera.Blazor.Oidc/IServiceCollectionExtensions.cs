// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

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
        public static IServiceCollection AddOidc(this IServiceCollection services, OidcSettings settings)
        {
            new OidcSettingsValidator().EnsureValidSettings(services, settings);

            services.AddAuthorizationCore();

            services.TryAddScoped(settings);
            services.TryAddScoped<Metadata>();
            services.TryAddScoped<Interop>(); //TODO: ensure that IJSRuntime is singleton too
            services.TryAddScoped<AuthenticationStateProvider, AuthStateProvider>();
            services.TryAddScoped<OidcHttpClient>();
            services.TryAddScoped<IUserManager, UserManager>();

            services.TryAddScoped(typeof(IOidcLogger<>), typeof(OidcLogger<>));
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
            services.TryAddScopedStorage(settings);

            return services;
        }

        private static void TryAddScopedStorage(this IServiceCollection services, OidcSettings settings)
        {
            services.TryAddScoped<IStore, Store>();

            if (settings.StorageType.IsMemory())
                services.TryAddScoped<IStorage, MemoryStorage>();
            else
                services.TryAddTransient<IStorage, BrowserStorage>();
        }

        private static void TryAddScoped<T>(this IServiceCollection services, T instance) where T : class
        {
            services.TryAddScoped(provider => instance);
        }
    }
}