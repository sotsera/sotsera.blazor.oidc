// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Storage
{
    internal interface IStore
    {
        Task<AuthState> GetAuthState();
        Task SetAuthState(AuthState state);
        Task RemoveAuthState();

        Task<LogoutState> GetLogoutState();
        Task SetLogoutState(LogoutState state);
        Task RemoveLogoutState();

        Task<UserState> GetUserState();
        Task SetUserState(UserState user);
        Task RemoveUserState();
    }

    internal class Store: ThrowsErrors<Store>, IStore
    {
        public const string AuthStateKey = "auth-request-state";
        public const string LogoutStateKey = "logout-request-state";
        public const string UserStateKey = "user-state";

        private OidcSettings Settings { get; }
        private IMetadataService Metadata { get; }
        private IStorage Storage { get; }
        protected override IOidcLogger<Store> Logger { get; }

        public Store(OidcSettings settings, IMetadataService metadata, IStorage storage, IOidcLogger<Store> logger)
        {
            Settings = settings;
            Metadata = metadata;
            Storage = storage;
            Logger = logger;
        }

        public Task<AuthState> GetAuthState()
        {
            return HandleErrors(nameof(AuthState), async () =>
            {
                var state = await Storage.Get<AuthState>(AuthStateKey);
                var issuer = await Metadata.Issuer();
                var clientId = Settings.ClientId;

                const string prefix = "State from storage does not contain the expected";

                if (state == null)
                    throw Logger.Exception("Storage does not contain the athentication state");
                if (state.Issuer != issuer)
                    throw Logger.Exception($@"{prefix} Issuer: ""{state.Issuer}"" instead of ""{issuer}""");
                if (state.ClientId != clientId)
                    throw Logger.Exception($@"{prefix} ClientId: ""{state.ClientId}"" instead of ""{clientId}""");

                await Storage.Remove(AuthStateKey);

                return state;
            });
        }

        public Task SetAuthState(AuthState state)
        {
            return HandleErrors(nameof(SetAuthState), () => Storage.Set(AuthStateKey, state));
        }
        
        public Task RemoveAuthState()
        {
            return HandleErrors(nameof(RemoveUserState), () => Storage.Remove(AuthStateKey));
        }

        public Task<LogoutState> GetLogoutState()
        {
            return HandleErrors(nameof(LogoutState), async () =>
            {
                var state = await Storage.Get<LogoutState>(LogoutStateKey);

                if (state == null)
                    throw Logger.Exception("Storage does not contain the state");

                return state;
            });
        }

        public Task SetLogoutState(LogoutState state)
        {
            return HandleErrors(nameof(SetLogoutState), () => Storage.Set(LogoutStateKey, state));
        }
        
        public Task RemoveLogoutState()
        {
            return HandleErrors(nameof(RemoveUserState), () => Storage.Remove(LogoutStateKey));
        }

        public Task<UserState> GetUserState()
        {
            return HandleErrors(nameof(GetUserState), () => Storage.Get<UserState>(UserStateKey));
        }

        public Task SetUserState(UserState userState)
        {
            return HandleErrors(nameof(SetUserState), () => Storage.Set(UserStateKey, userState));
        }

        public Task RemoveUserState()
        {
            return HandleErrors(nameof(RemoveUserState), () => Storage.Remove(UserStateKey));
        }
    }
}