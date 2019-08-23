// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Specialized;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;

namespace Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model
{
    public class AuthParameters: CallbackRequestParameters
    {
        internal string Issuer { get; set; }
        internal string AuthorizationEndpoint { get; set; }

        public string Scope { get; set; }
        public string ResponseType { get; internal set; }
        public string[] ResponseTypes { get; internal set; }
        public string ClientId { get; internal set; }
        public string ResponseMode { get; internal set; }
        public Prompt? Prompt { get; set; }
        public int? MaxAge { get; set; }
        public string UiLocales { get; set; }
        public string IdTokenHint { get; set; }
        public string LoginHint { get; set; }
        public string AcrValues { get; set; }
        public string Resource { get; set; }
        public string Request { get; set; }
        public string RequestUri { get; set; }
        public TimeSpan OpenPopupTimeout { get; set; }
        public NameValueCollection AdditionalParameters { get; internal set; }

        public bool RequiresNonce => ResponseType.IsNotEmpty() && ResponseType.StartsWith("id_token");
        public bool RequiresCodeVerifier => ResponseType == "code";
    }
}