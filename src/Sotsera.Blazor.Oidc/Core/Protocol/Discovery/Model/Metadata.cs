// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model
{
    internal class Metadata
    {
        public string Issuer { get; set; }
        public string MetadataEndpoint { get; set; }
        public OpenidEndpoints Endpoints { get; set; }
        public Jwks Jwks { get; set; }

        public Metadata(OidcSettings settings)
        {
            Issuer = settings.Issuer;
            MetadataEndpoint = settings.MetadataEndpoint;
            Endpoints = settings.Endpoints ?? new OpenidEndpoints();
            Jwks = settings.Jwks;
        }
    }
}