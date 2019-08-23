// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Diagnostics;

namespace Sotsera.Blazor.Oidc
{
    // ReSharper disable once InconsistentNaming
    internal static class IDictionaryExtensions
    {
        [DebuggerStepThrough]
        public static bool IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> values)
        {
            return values == null || values.Keys.Count == 0;
        }

        [DebuggerStepThrough]
        public static T Get<T>(this IDictionary<string, object> values, string key)
        {
            if (values.IsEmpty()) return default;
            if (!values.ContainsKey(key)) return default;

            return (T)values[key];
        }

        [DebuggerStepThrough]
        public static string Get(this IDictionary<string, string> values, string key)
        {
            return values.IsEmpty() || !values.ContainsKey(key) ? default : values[key];
        }
    }
}