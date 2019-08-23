// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model
{
    internal class LogoutRequest
    {
        public string Url { get; set; }
        public LogoutParameters Parameters { get; set; }
        public LogoutState State { get; set; }
    }
}