// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Text.Json.Serialization;

namespace Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model
{
    public class OpenidEndpoints
    {
        public string Issuer { get; set; }
        [JsonPropertyName("jwks_uri")]
        public string JwksUri { get; set; }
        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }
        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }
        [JsonPropertyName("userinfo_endpoint")]
        public string UserinfoEndpoint { get; set; }
        [JsonPropertyName("end_session_endpoint")]
        public string EndSessionEndpoint { get; set; }
        [JsonPropertyName("check_session_iframe")]
        public string CheckSessionIframe { get; set; }
        [JsonPropertyName("revocation_endpoint")]
        public string RevocationEndpoint { get; set; }

        public void Merge(OpenidEndpoints endpoints)
        {
            if (endpoints == null) return;

            if (Issuer.IsEmpty()) Issuer = endpoints.Issuer;
            if (JwksUri.IsEmpty()) JwksUri = endpoints.JwksUri;
            if (AuthorizationEndpoint.IsEmpty()) AuthorizationEndpoint = endpoints.AuthorizationEndpoint;
            if (TokenEndpoint.IsEmpty()) TokenEndpoint = endpoints.TokenEndpoint;
            if (UserinfoEndpoint.IsEmpty()) UserinfoEndpoint = endpoints.UserinfoEndpoint;
            if (EndSessionEndpoint.IsEmpty()) EndSessionEndpoint = endpoints.EndSessionEndpoint;
            if (CheckSessionIframe.IsEmpty()) CheckSessionIframe = endpoints.CheckSessionIframe;
            if (RevocationEndpoint.IsEmpty()) RevocationEndpoint = endpoints.RevocationEndpoint;
        }
    }
}