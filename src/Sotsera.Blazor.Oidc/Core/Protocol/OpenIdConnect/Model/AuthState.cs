// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Text.Json.Serialization;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;

namespace Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model
{
    internal class AuthState
    {
        public string Issuer { get; set; }
        public string ClientId { get; set; }
        public string ResponseType { get; set; }
        public string State { get; set; }
        public string Nonce { get; set; }
        public string RedirectUri { get; set; }
        public string ResponseMode { get; set; }
        public string CodeVerifier { get; set; }
        
        [JsonIgnore]
        public string CodeChallenge { get; set; }

        internal bool IsImplicitFlow => ResponseType.StartsWith("id_token");
        internal bool IsCodeFlow => ResponseType == "code";

        public UrlParsingType RequiredResponseMode =>
            ResponseMode == "query" || ResponseMode.IsEmpty() && IsCodeFlow 
                ? UrlParsingType.Query 
                : UrlParsingType.Fragment;
    }
}