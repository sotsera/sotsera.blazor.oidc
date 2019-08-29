// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model
{
    internal class CheckSessionResult
    {
        public CheckSessionResultType Type { get; set; }
        public string Message { get; }

        public bool IsValid => Type == CheckSessionResultType.Valid;

        public CheckSessionResult(CheckSessionResultType type, string message)
        {
            Type = type;
            Message = $"CheckSessionIFrame: \"{message}\" message from check session op iframe";
        }
    }
}