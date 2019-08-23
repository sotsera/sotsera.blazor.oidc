// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Configuration.Model
{
    public enum StorageType
    {
        LocalStorage,
        SessionStorage,
        MemoryStorage
    }

    internal static class StorageTypeExtensions
    {
        public static bool IsMemory(this StorageType type) => type == StorageType.MemoryStorage;
        public static bool IsLocal(this StorageType type) => type == StorageType.LocalStorage;
        public static bool IsSession(this StorageType type) => type == StorageType.SessionStorage;
    }
}