// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.BrowserInterop;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Core.Storage;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core
{
    internal interface IUserManagerHelper
    {
        Task Init(UserManager userManager);
        Task<UserState> UserState();
        Task ClearUserState();
        Task SetUserState(UserState userState);
        Task StartFlow(OidcRequest request);
    }

    internal class UserManagerHelper: IUserManagerHelper
    {
        private IStore Store { get; }
        private IUriHelper UriHelper { get; }
        private Interop Interop { get; }
        private OidcHttpClient HttpClient { get; }
        private IOidcLogger<UserManagerHelper> Logger { get; }

        public UserManagerHelper(IStore store, IUriHelper uriHelper, Interop interop,
            OidcHttpClient httpClient, IOidcLogger<UserManagerHelper> logger)
        {
            Store = store;
            UriHelper = uriHelper;
            Interop = interop;
            HttpClient = httpClient;
            Logger = logger;
        }

        public async Task Init(UserManager userManager)
        {
            try
            {
                await Interop.Init(userManager);
            }
            catch (Exception)
            {
                var projectName = typeof(IUserManager).Namespace;
                var fileVersion = $"_content/{projectName}/{projectName.ToLower()}-{userManager.Version}.js";
                const string errorMessage = "Check if the index.html file references the correct js version:";
                var line = new string('-', Math.Max(errorMessage.Length, fileVersion.Length));

                Logger.LogError(line);
                Logger.LogError(errorMessage);
                Logger.LogError(fileVersion);
                Logger.LogError(line);
                throw Logger.Exception($"Unable to initialize the {projectName} javascript library");
            }
        }

        public Task<UserState> UserState() => Store.GetUserState();

        public async Task ClearUserState()
        {
            await Store.RemoveUserState();
            HttpClient.RemoveToken();
        }

        public async Task SetUserState(UserState userState)
        {
            await Store.SetUserState(userState);
            HttpClient.SetToken(userState.AccessToken);
        }

        public Task StartFlow(OidcRequest request)
        {
            if (request.InteractionType.IsPopup())
            {
                return Interop.OpenPopup(request);
            }

            UriHelper.NavigateTo(request.Url);
            return Task.CompletedTask;
        }
    }
}