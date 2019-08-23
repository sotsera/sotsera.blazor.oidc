// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model
{
    public class Jwk
    {
        public string Alg { get; set; }
        public string Crv { get; set; }
        public string D { get; set; }
        public string E { get; set; }
        public string K { get; set; }
        public string Kid { get; set; }
        public string Kty { get; set; }
        public string N { get; set; }
        public string Use { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
    }
}