// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.BrowserInterop;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;
using Sotsera.Blazor.Oidc.Core.Storage;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core
{
    internal class UserManager: IUserManager, IDisposable
    {
        private bool Initialized { get; set; }
        private IOidcLogger<UserManager> Logger { get; }
        private IOidcClient OidcClient { get; }
        private ILogoutClient LogoutClient { get; }
        private ISessionMonitor Monitor { get; }
        private IStore Store { get; }
        private Interop Interop { get; }
        private OidcHttpClient HttpClient { get; }
        private IUriHelper UriHelper { get; }

        public OidcUser User => UserState?.User;
        public UserState UserState { get; private set; }
        public string Version { get; }
        public event Action<OidcUser> UserChanged;
        public event Action<UserSessionExpiringArgs> OnUserSessionExpiring;
        public event Action<string> OnError;

        public UserManager(
            IOidcClient oidcClient, ILogoutClient logoutClient, ISessionMonitor monitor,
            IStore store, IUriHelper uriHelper, Interop interop, 
            OidcHttpClient httpClient, IOidcLogger<UserManager> logger
        )
        {
            OidcClient = oidcClient;
            LogoutClient = logoutClient;
            Monitor = monitor;
            Store = store;
            UriHelper = uriHelper;
            Interop = interop;
            HttpClient = httpClient;
            Logger = logger;
            Version = GetType().InformationalVersion();

            Monitor.OnSessionChanged += SessionChanged;
        }

        public Task InitAsync(bool skipInitialStateValidation = false)
        {
            return HandleErrors(nameof(InitAsync), async () =>
            {
                if (Initialized) return;

                try
                {
                    await Interop.Init(this);
                }
                catch (Exception)
                {
                    RaiseInitializationError();
                }

                if (!skipInitialStateValidation)
                {
                    var userState = await Store.GetUserState();
                    if (userState != null)
                    {
                        var sessionState = await Monitor.CheckSession(userState);
                        Logger.LogDebug(sessionState.Message);
                        if (sessionState.Type == CheckSessionResultType.Valid)
                        {
                            UpdateUserState(userState, false);
                            await Monitor.Start(UserState);
                        }
                        //await SilentLoginAsync(false);
                    }
                }

                Initialized = true;
            });
        }

        public Task CheckSessionAsync(bool raiseEvent = true)
        {
            return HandleErrors(nameof(CheckSessionAsync), () => Task.CompletedTask);
        }

        public Task SilentLoginAsync(bool raiseEvent = true)
        {
            return HandleErrors(nameof(SilentLoginAsync), () => Task.CompletedTask);
        }

        public Task BeginAuthenticationAsync(Action<AuthParameters> configureParameters = null)
        {
            return HandleErrors(nameof(BeginAuthenticationAsync), async () =>
            {
                var request = await OidcClient.CreateAuthenticationRequest(configureParameters);

                if (request.Parameters.InteractionType.IsRedirect())
                    UriHelper.NavigateTo(request.Url);
                else
                    await Interop.OpenPopup(OidcClient.CreatePopupRequest(request));
            });
        }

        public Task CompleteAuthenticationAsync(string url)
        {
            return HandleErrors(nameof(CompleteAuthenticationAsync), async () =>
            {
                await InitAsync(true); //Needed for redirect callback

                var userState = await OidcClient.ParseResponse(url);

                await Store.SetUserState(userState);
                UpdateUserState(userState, true);
                await Monitor.Start(userState);
            });
        }

        public Task BeginLogoutAsync(Action<LogoutParameters> configureParameters = null)
        {
            if (UserState == null) return Task.CompletedTask;
            Monitor.Stop();
            
            return HandleErrors(nameof(BeginLogoutAsync), async () =>
            {
                await InitAsync(true); //Needed for redirect callback

                var idToken = UserState.IdToken;
                
                var request = await LogoutClient.CreateLogoutRequest(idToken, configureParameters);
                
                UpdateUserState(null, true);
                await Store.RemoveUserState();

                if (request.Parameters.InteractionType.IsRedirect())
                {
                    UriHelper.NavigateTo(request.Url);
                }
                else
                {
                    UserChanged?.Invoke(User);
                    await Interop.OpenPopup(LogoutClient.CreatePopupRequest(request));
                }
            });
        }

        public Task CompleteLogoutAsync(string url)
        {
            return HandleErrors(nameof(CompleteLogoutAsync), async () =>
            {
                await InitAsync(true); //Needed for redirect callback
                await LogoutClient.ParseResponse(url);
            });
        }

        private async Task HandleErrors(string methodName, Func<Task> action)
        {
            try
            {
                Logger.LogTrace($"{nameof(UserManager)}.{methodName}");
                await action.Invoke();
            }
            catch (Exception ex)
            {
                if (!(ex is OidcException oidcException) || !oidcException.Logged) Logger.LogError(ex.Message);
                OnError?.Invoke(ex.Message);
            }
        }

        private void UpdateUserState(UserState userState, bool raiseEvent)
        {
            UserState = userState;
            if(raiseEvent) UserChanged?.Invoke(userState?.User);
            HttpClient.SetToken(userState?.AccessToken);
        }

        private void RaiseInitializationError()
        {
            var projectName = typeof(IUserManager).Namespace;
            var fileVersion = $"_content/{projectName}/{projectName.ToLower()}-{Version}.js";
            const string errorMessage = "Check if the index.html file references the correct js version:";
            var line = new string('-', Math.Max(errorMessage.Length, fileVersion.Length));

            Logger.LogError(line);
            Logger.LogError(errorMessage);
            Logger.LogError(fileVersion);
            Logger.LogError(line);
            throw Logger.Exception($"Unable to initialize the {projectName} javascript library");
        }

        private void SessionChanged(CheckSessionResult sessionState)
        {
            if (sessionState.Type == CheckSessionResultType.Changed)
            {
                var args = new UserSessionExpiringArgs();
                OnUserSessionExpiring?.Invoke(args);

                if (!args.Cancel)
                {
                    UpdateUserState(null, true);
                }
            }
        }

        public void Dispose()
        {
            Monitor.OnSessionChanged -= SessionChanged;
            Monitor?.Dispose();
            HttpClient?.Dispose();
        }
    }
}