// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace Sotsera.Blazor.Oidc
{
    public class OidcHttpClient : HttpClient
    {
        public OidcHttpClient(OidcSettings settings, IUriHelper uriHelper)
        {
            BaseAddress = new Uri(uriHelper.GetBaseUri());
            Timeout = settings.HttpClientTimeout;
        }

        public void SetToken(string token)
        {

            DefaultRequestHeaders.Authorization = token.IsEmpty()
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
        }

        public void RemoveToken()
        {
            DefaultRequestHeaders.Authorization = null;
        }
    }
}