// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sotsera.Blazor.Oidc
{
    // ReSharper disable once InconsistentNaming
    internal static class IListExtensions
    {
        [DebuggerStepThrough]
        public static bool IsEmpty<T>(this IList<T> list) => list == null || list.Count == 0;
        [DebuggerStepThrough]
        public static bool IsNotEmpty<T>(this IList<T> list) => list != null && list.Count > 0;
        [DebuggerStepThrough]
        public static bool Contains<T>(this IList<T> list, T value) => list.Any(x => x.Equals(value));

        [DebuggerStepThrough]
        public static IEnumerable<string[]> Split(this IEnumerable<string> values, params char[] separators)
        {
            if (values == null) yield break;
            if (separators == null) yield break;

            foreach (var value in values)
            {
                yield return value.Split(separators);
            }
        }

        [DebuggerStepThrough]
        public static IList<T> AddIfNotEmpty<T>(this IList<T> values, T value)
        {
            if(values == null) values = new List<T>();
            if(value != null) values.Add(value);
            return values;
        }
    }
}