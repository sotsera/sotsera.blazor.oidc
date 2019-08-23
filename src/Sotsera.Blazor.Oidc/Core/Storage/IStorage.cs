// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Threading.Tasks;

namespace Sotsera.Blazor.Oidc.Core.Storage
{
    internal interface IStorage
    {
        bool CanSurvivePageRefresh { get; }
        Task<T> Get<T>(string key);
        Task Set(string key, object item);
        Task Remove(string key);
    }
}