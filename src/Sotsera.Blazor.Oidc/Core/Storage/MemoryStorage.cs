// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sotsera.Blazor.Oidc.Core.Storage
{
    internal class MemoryStorage : IStorage
    {
        private Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
        public bool CanSurvivePageRefresh => false;

        public Task<T> Get<T>(string key)
        {
            if (!Data.ContainsKey(key)) return default;
            var value = Data[key];
            if (value is T typedValue) return Task.FromResult(typedValue);
            return default;
        }

        public Task Set(string key, object item)
        {
            Data[key] = item;
            return Task.CompletedTask;
        }

        public Task Remove(string key)
        {
            Data.Remove(key);
            return Task.CompletedTask;
        }
    }
}