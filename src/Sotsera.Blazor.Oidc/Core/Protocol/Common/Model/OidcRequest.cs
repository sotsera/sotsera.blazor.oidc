// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using Sotsera.Blazor.Oidc.Configuration.Model;

namespace Sotsera.Blazor.Oidc.Core.Protocol.Common.Model
{
    public class OidcRequest
    {
        public InteractionType InteractionType { get; set; }
        public string Url { get; set; }
        public double Timeout { get; set; }
        public string WindowName { get; set; }
        public string WindowFeatures { get; set; }
    }
}