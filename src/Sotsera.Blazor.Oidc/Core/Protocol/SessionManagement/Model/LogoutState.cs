// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model
{
    internal class LogoutState
    {
        public string State { get; set; }

        public OidcRequestState OidcRequestState => Base64Url.Deserialize<OidcRequestState>(State, "oidc logout request state");
    }
}