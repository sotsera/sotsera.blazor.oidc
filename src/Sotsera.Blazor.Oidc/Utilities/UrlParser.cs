// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;

namespace Sotsera.Blazor.Oidc.Utilities
{
    internal class UrlParser
    {
        public ResponseUri Parse(string url)
        {
            if (url.IsEmpty()) throw new ArgumentNullException(nameof(url));
            var uri = new Uri(url);

            return new ResponseUri
            {
                Fragment = ParseUriParts(uri.Fragment),
                Query = ParseUriParts(uri.Query)
            };
        }

        private static Dictionary<string, string> ParseUriParts(string part)
        {
            if (part.StartsWith("?") || part.StartsWith("#")) part = part.Skip(1);

            var parameters = new Dictionary<string, string>();
            if (part.IsEmpty()) return parameters;

            foreach (var couple in part.Split('&').Where(x => x.IsNotEmpty()).Split('='))
            {
                if (couple.Length != 2 || couple[0].IsEmpty() || couple[1].IsEmpty())
                {
                    throw new OidcException("Malformed callback URL.");
                }

                parameters[WebUtility.UrlDecode(couple[0])] = WebUtility.UrlDecode(couple[1]);
            }

            return parameters;
        }
    }
}