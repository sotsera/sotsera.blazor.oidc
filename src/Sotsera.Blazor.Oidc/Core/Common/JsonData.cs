// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Common
{
    public class JsonData: Dictionary<string, JsonElement>
    {
        public T Value<T>(string key)
        {
            if (key.IsEmpty()) throw new ArgumentNullException(nameof(key));
            return ContainsKey(key) ? Json.Deserialize<T>(this[key], key) : default;
        }

        public T[] ValueAsArray<T>(string key)
        {
            if (key.IsEmpty()) throw new ArgumentNullException(nameof(key));
            if (!ContainsKey(key)) return default;

            var element = this[key];

            return element.ValueKind == JsonValueKind.Array 
                ? Json.Deserialize<T[]>(element, key) 
                : new[] {Json.Deserialize<T>(element, key)};
        }

        public void Merge(IDictionary<string, JsonElement> source)
        {
            if (source.IsEmpty()) return;

            foreach (var key in source.Keys)
            {
                if(!ContainsKey(key)) Add(key, source[key]);
            }
        }
    }
}