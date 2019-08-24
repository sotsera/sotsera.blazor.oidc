// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;

namespace Sotsera.Blazor.Oidc
{
    public interface IUserManager
    {
        UserState UserState { get; }
        OidcUser User { get; }
        string Version { get; }

        event Action<OidcUser> UserChanged;
        event Action<string> OnError;

        Task InitAsync(bool skipInitialStateValidation = false);
        //Task CheckSessionAsync(bool raiseEvent = true);
        //Task SilentLoginAsync(bool raiseEvent = true);

        Task BeginAuthenticationAsync(Action<AuthParameters> configureParameters = null);
        Task BeginLogoutAsync(Action<LogoutParameters> configureParameters= null);
        Task CompleteAuthenticationAsync(string url);
        Task CompleteLogoutAsync(string url);
    }
}