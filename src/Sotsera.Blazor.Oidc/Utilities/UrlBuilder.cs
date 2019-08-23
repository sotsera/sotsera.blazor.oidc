// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Sotsera.Blazor.Oidc.Utilities
{
    internal class UrlBuilder
    {
        private StringBuilder Builder { get; }
        private bool FirstParameter { get; set; }

        public UrlBuilder(string baseUrl)
        {
            if (baseUrl.IsEmpty()) throw new ArgumentNullException(nameof(baseUrl));

            baseUrl = baseUrl.Trimmed().TrimEnd('?', '&');
            Builder = new StringBuilder(baseUrl);
            FirstParameter = baseUrl.IndexOf('?') < 0;
        }

        public UrlBuilder Add(string name, string value)
        {
            if (name.IsEmpty()) throw new ArgumentNullException(nameof(name));
            if (value.IsEmpty()) return this;

            Builder.Append(FirstParameter ? "?" : "&");
            Builder.Append(WebUtility.UrlEncode(name));
            Builder.Append("=");
            Builder.Append(WebUtility.UrlEncode(value));

            FirstParameter = false;
            return this;
        }

        public UrlBuilder Add(string name, string value, bool condition)
        {
            return condition ? Add(name, value) : this;
        }

        public UrlBuilder Add(NameValueCollection values)
        {
            if (values == null || values.Count == 0) return this;

            for (var i = 0; i < values.Count; i++)
            {
                Add(values.GetKey(i), values.Get(i));
            }

            return this;
        }

        public override string ToString()
        {
            return Builder.ToString();
        }
    }
}