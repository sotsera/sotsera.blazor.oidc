// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using Sotsera.Blazor.Oidc;

namespace ClientSide
{
    /// <summary>
    /// https://github.com/IdentityServer/IdentityServer4.Demo/blob/master/src/IdentityServer4Demo/Config.cs
    /// </summary>
    public static class OidcSample
    {
        public static OidcSettings Code(OidcSettings settings)
        {
            settings.ClientId = "spa";
            settings.ResponseType = "code";
            return settings;
        }

        public static OidcSettings CodeWithShortLivedToken(OidcSettings settings)
        {
            settings.ClientId = "spa.short";
            settings.ResponseType = "code";
            return settings;
        }

        public static OidcSettings Implicit(OidcSettings settings)
        {
            settings.ClientId = "implicit";
            settings.ResponseType = "id_token token";
            return settings;
        }

        public static OidcSettings ImplicitReference(OidcSettings settings)
        {
            settings.ClientId = "implicit.reference";
            settings.ResponseType = "id_token token";
            return settings;
        }

        public static OidcSettings ImplicitWithShortLivedToken(OidcSettings settings)
        {
            settings.ClientId = "implicit.shortlived";
            settings.ResponseType = "id_token token";
            return settings;
        }
    }
}