// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;

namespace Sotsera.Blazor.Oidc.Utilities
{
    internal class Time
    {
        // TODO: Utc?
        private static DateTime Now => DateTime.Now;

        public static DateTime? ParseExpiration(int expiresIn)
        {
            return expiresIn <= 0 ? Now : Now.AddSeconds(expiresIn);
        }

        public static DateTime? ParseExpiration(string expiresIn)
        {
            if (expiresIn.IsEmpty()) return Now;
            return int.TryParse(expiresIn, out var seconds) ? ParseExpiration(seconds) : Now;
        }

        public static DateTimeOffset? FromNumericDate(int? epoch)
        {
            if (!epoch.HasValue) return null;
            // FromUnixTimeSeconds does not take leap seconds into account.
            return DateTimeOffset.FromUnixTimeSeconds(epoch.Value);
        }
    }
}