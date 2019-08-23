// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Core.Protocol.Common.Model
{
    public class OidcRequest
    {
        public string Url { get; set; }
        public int Timeout { get; set; }
        public string WindowName { get; set; }
        public string WindowFeatures { get; set; }
    }
}