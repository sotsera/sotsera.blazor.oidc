// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Sotsera.Blazor.Oidc.Core
{
    public class AuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        private IUserManager UserManager { get; }

        public AuthStateProvider(IUserManager userManager)
        {
            UserManager = userManager;
            UserManager.UserChanged += OnUserChanged;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await UserManager.InitAsync();
            return UserManager.User.ToAuthenticationState();
        }

        private void OnUserChanged(OidcUser user) => Notify(user);

        private void Notify(OidcUser user)
        {
            var state = user.ToAuthenticationState();
            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        public void Dispose()
        {
            UserManager.UserChanged -= OnUserChanged;
        }
    }
}