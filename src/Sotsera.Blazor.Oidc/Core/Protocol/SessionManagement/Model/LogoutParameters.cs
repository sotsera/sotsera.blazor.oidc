// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;

namespace Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model
{
    public class LogoutParameters: CallbackRequestParameters
    {
        public string EndSessionEndpoint { get; set; }
        public string IdTokenHint { get; set; }
        public TimeSpan OpenPopupTimeout { get; set; }
        public bool RevokeAccessTokenOnSignout { get; set; }

        public NameValueCollection AdditionalParameters { get; internal set; }
        public Dictionary<string, string> StateData { get; internal set; }
    }
}