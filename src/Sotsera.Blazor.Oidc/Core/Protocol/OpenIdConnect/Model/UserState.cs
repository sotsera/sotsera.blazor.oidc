// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model
{
    public class UserState
    {
        public string AccessToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string IdToken { get; set; }
        public string State { get; set; }
        public string[] Scopes { get; set; }
        public string SessionState { get; set; }
        public string TokenType { get; set; }
        public OidcUser User { get; set; }

        public OidcRequestState OidcRequestState => Base64Url.Deserialize<OidcRequestState>(State, "oidc authentication request state");
        //public bool Expired => ExpiresAt <= DateTime.Now;
    }
}