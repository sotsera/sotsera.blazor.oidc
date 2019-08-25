// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model
{
    internal class AuthResponse : OidcResponse
    {
        public string AccessToken { get; set; }
        public string Code { get; set; } // ??
        public DateTime? ExpiresAt { get; set; }
        public string IdToken { get; set; }
        public string Scope { get; set; }
        public string SessionState { get; set; }
        public string TokenType { get; set; }

        public bool Expired => ExpiresAt <= DateTime.Now;
        public IDictionary<string, object> Profile { get; set; }

        public string[] Scopes => Scope.SplitBy(' ');
        public bool ContainsToken => IdToken.IsNotEmpty() || Scope.Contains("openid");

        public bool LoadUserInfo { get; set; }

        public void Merge(CodeResponse response)
        {
            if (response == null) return;
            if (response.IdToken.IsNotEmpty()) IdToken = response.IdToken;
            if (response.AccessToken.IsNotEmpty()) AccessToken = response.AccessToken;
            ExpiresAt = Time.ParseExpiration(response.ExpiresIn);
            if (response.TokenType.IsNotEmpty()) TokenType = response.TokenType;
            if (response.Scope.IsNotEmpty()) Scope = response.Scope;
        }
    }
}