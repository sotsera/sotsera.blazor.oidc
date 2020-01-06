// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Collections.Specialized;

namespace Sotsera.Blazor.Oidc.Core.Protocol.Common.Model
{
    public class OidcRequestState
    {
        public string Id { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}