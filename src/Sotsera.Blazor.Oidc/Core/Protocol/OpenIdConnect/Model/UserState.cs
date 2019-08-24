// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;

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

        //public bool Expired => ExpiresAt <= DateTime.Now;
    }
}