// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;

namespace Sotsera.Blazor.Oidc
{
    internal static class StringExtensions
    {
        
        [DebuggerStepThrough]
        public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);
        [DebuggerStepThrough]
        public static bool IsNotEmpty(this string value) => !string.IsNullOrWhiteSpace(value);
        [DebuggerStepThrough]
        public static string Trimmed(this string value) => value.IsEmpty() ? string.Empty : value.Trim();

        [DebuggerStepThrough]
        public static string Skip(this string value, int charsToSkip)
        {
            return value == null || value.Length <= charsToSkip
                ? string.Empty
                : value.Substring(charsToSkip);
        }

        [DebuggerStepThrough]
        public static string[] SplitBy(this string value, params char[] separators)
        {
            return value.IsEmpty()
                ? new string[0]
                : value.Split(separators , StringSplitOptions.RemoveEmptyEntries);
        }
    }
}