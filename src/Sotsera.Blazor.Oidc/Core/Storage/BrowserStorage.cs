// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Sotsera.Blazor.Oidc.BrowserInterop;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Storage
{
    internal class BrowserStorage : ThrowsErrors<BrowserStorage>,  IStorage
    {
        private string KeyPrefix { get; }
        private string StorageType { get; }
        private Interop Interop { get; }
        protected override IOidcLogger<BrowserStorage> Logger { get; }
        public bool CanSurvivePageRefresh => true;

        public BrowserStorage(OidcSettings settings, Interop interop, IOidcLogger<BrowserStorage> logger)
        {
            StorageType = settings.StorageType.IsLocal() ? "localStorage" : "sessionStorage";
            KeyPrefix = settings.StoragePrefix;
            Interop = interop;
            Logger = logger;
        }

        public Task<T> Get<T>(string key)
        {
            return HandleErrors(nameof(Get), async () =>
            {
                var value = await Interop.GetAsync<string>(StorageType, FormatKey(key));
                return Json.Deserialize<T>(value, key);
            });
        }

        public Task Set(string key, object item)
        {
            return HandleErrors(nameof(Set), () =>
            {
                var stringValue = Json.Serialize(item, key);
                return Interop.SetAsync(StorageType, FormatKey(key), stringValue);
            });
        }

        public Task Remove(string key)
        {
            return HandleErrors(nameof(Remove), () => Interop.RemoveAsync(StorageType, FormatKey(key)));
        }

        private string FormatKey(string key)
        {
            if (key.IsEmpty()) throw new ArgumentNullException(nameof(key));
            return $"{KeyPrefix}.{key}";
        }
    }
}