// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.Discovery
{
    internal interface IMetadataService
    {
        Task<string> Issuer();
        Task<string> AuthorizationEndpoint();
        Task<string> TokenEndpoint();
        Task<string> UserinfoEndpoint();
        Task<string> EndSessionEndpoint();
        Task<Jwks> JsonWebKeys();
    }

    internal class MetadataService : ThrowsErrors<MetadataService>, IMetadataService
    {
        private Metadata Metadata { get; }
        private HttpClient Client { get; }

        protected override IOidcLogger<MetadataService> Logger { get; }

        private OpenidEndpoints Endpoints => Metadata.Endpoints;

        public MetadataService(Metadata metadata, HttpClient client, IOidcLogger<MetadataService> logger)
        {
            Metadata = metadata;
            Client = client;
            Logger = logger;
        }

        public Task<string> Issuer()
        {
            return HandleErrors(nameof(Issuer), async () =>
            {
                if (Endpoints.Issuer.IsEmpty()) await LoadMetadata();
                return Endpoints.Issuer.IsNotEmpty()
                    ? Endpoints.Issuer
                    : throw Logger.Exception("Missing issuer");
            });
        }

        public Task<string> AuthorizationEndpoint()
        {
            return HandleErrors(nameof(AuthorizationEndpoint), async () =>
            {
                if (Endpoints.AuthorizationEndpoint.IsEmpty()) await LoadMetadata();
                return Endpoints.AuthorizationEndpoint.IsNotEmpty()
                    ? Endpoints.AuthorizationEndpoint
                    : throw Logger.Exception("Missing authorization enpoint");
            });
        }

        public Task<string> TokenEndpoint()
        {
            return HandleErrors(nameof(TokenEndpoint), async () =>
            {
                if (Endpoints.TokenEndpoint.IsEmpty()) await LoadMetadata();
                return Endpoints.TokenEndpoint.IsNotEmpty()
                    ? Endpoints.TokenEndpoint
                    : throw Logger.Exception("Missing token enpoint");
            });
        }

        public Task<string> UserinfoEndpoint()
        {
            return HandleErrors(nameof(UserinfoEndpoint), async () =>
            {
                if (Endpoints.UserinfoEndpoint.IsEmpty()) await LoadMetadata();
                return Endpoints.UserinfoEndpoint.IsNotEmpty()
                    ? Endpoints.UserinfoEndpoint
                    : throw Logger.Exception("Missing user info enpoint");
            });
        }

        public Task<string> EndSessionEndpoint()
        {
            return HandleErrors(nameof(EndSessionEndpoint), async () =>
            {
                if (Endpoints.EndSessionEndpoint.IsEmpty()) await LoadMetadata();
                return Endpoints.EndSessionEndpoint.IsNotEmpty()
                    ? Endpoints.EndSessionEndpoint
                    : throw Logger.Exception("Missing end session enpoint");
            });
        }

        public Task<Jwks> JsonWebKeys()
        {
            return HandleErrors(nameof(JsonWebKeys), async () =>
            {
                if (Metadata.Jwks.IsEmpty()) await LoadJwks();
                return Metadata.Jwks.IsNotEmpty()
                    ? Metadata.Jwks
                    : throw Logger.Exception("Missing authorization enpoint");
            });
        }

        private async Task LoadMetadata()
        {
            var uri = Metadata.MetadataEndpoint ?? throw Logger.Exception("Missing metadata endpoint");
            Logger.LogDebug($"Requesting the endpoints from {uri}");

            try
            {
                var metadata = await Client.GetJsonAsync<OpenidEndpoints>(uri);
                Endpoints.Merge(metadata);
            }
            catch (Exception ex)
            {
                throw Logger.Exception($"Error loading the endpoints from {uri}: {ex.Message}");
            }
        }

        private async Task LoadJwks()
        {
            var jwksUri = await JwksUri();
            Logger.LogDebug($"Requesting the jwks from {jwksUri}");

            try
            {
                Metadata.Jwks = await Client.GetJsonAsync<Jwks>(jwksUri);
            }
            catch (Exception ex)
            {
                throw Logger.Exception($"Error loading the jwks from {Metadata.MetadataEndpoint}: {ex.Message}");
            }
        }

        private async Task<string> JwksUri()
        {
            if (Endpoints.JwksUri.IsEmpty()) await LoadMetadata();
            return Endpoints.JwksUri.IsNotEmpty()
                ? Endpoints.JwksUri
                : throw Logger.Exception("Missing jwks enpoint");
        }
    }
}