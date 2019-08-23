// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Common;

namespace Sotsera.Blazor.Oidc
{
    // ReSharper disable once InconsistentNaming
    internal static class ILoggerExtensions
    {
        [DebuggerStepThrough]
        internal static OidcException Exception(this ILogger logger, string errorTemplate, params object[] args)
        {
            var errorMessage = string.Format(errorTemplate, args);
            logger.LogError(errorMessage);
            return new OidcException(errorMessage, true);
        }

        [DebuggerStepThrough]
        public static void ThrowIf(this ILogger logger, bool condition, string errorTemplate, params object[] args)
        {
            if (!condition) return;
            throw Exception(logger, errorTemplate, args);
        }
    }
}