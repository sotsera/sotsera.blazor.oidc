// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model
{
    public class Jwks
    {
        public Jwk[] Keys { get; set; }
    }

    internal static class JwksExtensions
    {
        public static bool IsEmpty(this Jwks keys) => keys?.Keys == null || keys.Keys.Length == 0;
        public static bool IsNotEmpty(this Jwks keys) => keys?.Keys != null && keys.Keys.Length > 0;
    }
}