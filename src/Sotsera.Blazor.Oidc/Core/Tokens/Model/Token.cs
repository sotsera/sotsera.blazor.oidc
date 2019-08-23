// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using Sotsera.Blazor.Oidc.Core.Tokens.Crypto.Algorithms;

namespace Sotsera.Blazor.Oidc.Core.Tokens.Model
{
    internal class Token
    {
        public JwsAlgorithm Algorithm { get; set; }
        public TokenHeader Header { get; set; }
        public TokenPayload Payload { get; set; }
    }
}