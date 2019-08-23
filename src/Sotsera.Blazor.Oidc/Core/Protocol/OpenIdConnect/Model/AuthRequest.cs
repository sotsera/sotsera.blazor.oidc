// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model
{
    internal class AuthRequest
    {
        public string Url { get; set; }
        public AuthParameters Parameters { get; set; }
        public AuthState State { get; set; }
    }
}