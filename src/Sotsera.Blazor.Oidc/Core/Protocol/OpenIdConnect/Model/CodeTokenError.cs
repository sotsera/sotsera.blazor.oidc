// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Text.Json.Serialization;

namespace Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model
{
    internal class CodeTokenError
    {
        [JsonPropertyName("error")]
        public string Description { get; set; }
    }
}