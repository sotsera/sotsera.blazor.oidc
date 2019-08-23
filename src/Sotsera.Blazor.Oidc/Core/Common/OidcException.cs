// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;

namespace Sotsera.Blazor.Oidc.Core.Common
{
    public class OidcException : Exception
    {
        internal bool Logged { get; }

        public OidcException(string message, bool logged = false) : base(message)
        {
            Logged = logged;
        }
    }
}