// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;
using System.Reflection;

namespace Sotsera.Blazor.Oidc
{
    internal static class TypeExtensions
    {
        [DebuggerStepThrough]
        public static string InformationalVersion(this Type type)
        {
            return type.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
    }
}