// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using Sotsera.Blazor.Oidc;

namespace ClientSide
{
    /// <summary>
    /// https://github.com/IdentityServer/IdentityServer4.Demo/blob/master/src/IdentityServer4Demo/Config.cs
    /// </summary>
    public static class DemoExtensions
    {
        public static FlowExtensions UseDemoFlow(this OidcSettings settings)
        {
            return new FlowExtensions(settings);
        }

        public class FlowExtensions
        {
            private OidcSettings Settings { get; }

            public FlowExtensions(OidcSettings settings)
            {
                Settings = settings;
            }

            public void Code()
            {
                Settings.ClientId = "spa";
                Settings.ResponseType = "code";
            }

            public void CodeWithShortLivedToken()
            {
                Settings.ClientId = "spa.short";
                Settings.ResponseType = "code";
            }

            public void Implicit()
            {
                Settings.ClientId = "implicit";
                Settings.ResponseType = "id_token token";
            }

            public void ImplicitReference()
            {
                Settings.ClientId = "implicit.reference";
                Settings.ResponseType = "id_token token";
            }

            public void ImplicitWithShortLivedToken()
            {
                Settings.ClientId = "implicit.shortlived";
                Settings.ResponseType = "id_token token";
            }
        }
    }
}