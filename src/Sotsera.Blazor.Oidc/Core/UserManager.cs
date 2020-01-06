// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core
{
    internal class UserManager : IUserManager, IDisposable
    {
        private IServiceProvider ServiceProvider { get; }
        private bool Initialized { get; set; }
        private bool SessionIsValid { get; set; }
        private IOidcLogger<UserManager> Logger { get; }
        private OidcSettings Settings { get; }
        private IOidcClient OidcClient { get; }
        private ILogoutClient LogoutClient { get; }
        private ISessionMonitor Monitor { get; }
        private IUserManagerHelper Helper { get; }

        public OidcUser User => UserState?.User;
        public UserState UserState { get; private set; }
        public string Version { get; }
        public event Action<OidcUser> UserChanged;
        public event Action<UserSessionExpiringArgs> OnUserSessionExpiring;
        public event Action<string> OnError;

        public UserManager(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Settings = serviceProvider.GetRequiredService<OidcSettings>();
            OidcClient = serviceProvider.GetRequiredService<IOidcClient>();
            LogoutClient = serviceProvider.GetRequiredService<ILogoutClient>();
            Monitor = serviceProvider.GetRequiredService<ISessionMonitor>();
            Helper = serviceProvider.GetRequiredService<IUserManagerHelper>();
            Logger = serviceProvider.GetRequiredService<IOidcLogger<UserManager>>();

            Version = GetType().InformationalVersion();
            if (Settings.MonitorSession) Monitor.OnSessionChanged += SessionChanged;
        }

        public Task InitAsync(bool skipInitialStateValidation = false)
        {
            return HandleErrors(nameof(InitAsync), async () =>
            {
                if (Initialized) return;

                await Helper.Init(this);

                if (!skipInitialStateValidation)
                {
                    var userState = await Helper.UserState();
                    if (userState != null)
                    {
                        if (Settings.MonitorSession)
                        {
                            var sessionState = await Monitor.CheckSession(userState);
                            if (sessionState.IsValid) await UpdateUserState(userState, false, false);
                        }
                        else
                        {
                            await UpdateUserState(userState, false, false);
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
                var browserRequest = OidcClient.CreateBrowserRequest(request);

                await Helper.StartFlow(browserRequest);
            });
        }

        public Task CompleteAuthenticationAsync(string url, InteractionType interactionType)
        {
            return HandleErrors(nameof(CompleteAuthenticationAsync), async () =>
            {
                await InitAsync(true); //Needed for redirect callback

                var userState = await OidcClient.ParseResponse(url);

                await UpdateUserState(userState, true, true);

                if (interactionType == InteractionType.Popup)
                {
                    if (Settings.PostAuthenticationPopup != null)
                    {
                        await Settings.PostAuthenticationPopup(userState.User, userState.OidcRequestState.Data, ServiceProvider);
                    }
                }
                else
                {
                    if (Settings.PostAuthenticationRedirect != null)
                    {
                        await Settings.PostAuthenticationRedirect(userState.User, userState.OidcRequestState.Data, ServiceProvider);
                    }
                }
            });
        }

        public Task BeginLogoutAsync(Action<LogoutParameters> configureParameters = null)
        {
            return HandleErrors(nameof(BeginLogoutAsync), async () =>
            {
                if (!SessionIsValid) await UpdateUserState(null, true, true);
                if (UserState == null) return;

                await InitAsync(true); //Needed for redirect callback

                var idToken = UserState.IdToken;

                var request = await LogoutClient.CreateLogoutRequest(idToken, configureParameters);
                var browserRequest = LogoutClient.CreateBrowserRequest(request);

                await UpdateUserState(null, false, true);

                if (request.Parameters.InteractionType.IsPopup()) UserChanged?.Invoke(null);
                await Helper.StartFlow(browserRequest);
            });
        }

        public Task CompleteLogoutAsync(string url, InteractionType interactionType)
        {
            return HandleErrors(nameof(CompleteLogoutAsync), async () =>
            {
                await InitAsync(true); //Needed for redirect callback
                var requestState = await LogoutClient.ParseResponse(url);

                if (interactionType == InteractionType.Popup)
                {
                    if (Settings.PostAuthenticationPopup != null)
                    {
                        await Settings.PostLogoutPopup(User, requestState.Data, ServiceProvider);
                    }
                }
                else
                {
                    if (Settings.PostAuthenticationRedirect != null)
                    {
                        await Settings.PostLogoutRedirect(User, requestState.Data, ServiceProvider);
                    }
                }
            });
        }

        private Task HandleErrors(string methodName, Func<Task> action)
        {
            try
            {
                Logger.LogTrace($"{nameof(UserManager)}.{methodName}");
                return action.Invoke();
            }
            catch (Exception ex)
            {
                if (!(ex is OidcException oidcException) || !oidcException.Logged) Logger.LogError(ex.Message);
                OnError?.Invoke(ex.Message);
                return Task.CompletedTask;
            }
        }

        private async Task UpdateUserState(UserState userState, bool raiseEvent, bool updateStore)
        {
            UserState = userState;

            if (UserState == null)
            {
                SessionIsValid = false;
                if (Settings.MonitorSession) Monitor.Stop();
                if (updateStore) await Helper.ClearUserState();
            }
            else
            {
                SessionIsValid = true;
                if(Settings.MonitorSession) await Monitor.Start(UserState);
                if (updateStore) await Helper.SetUserState(userState);
            }

            if (raiseEvent) UserChanged?.Invoke(userState?.User);
        }

        private async void SessionChanged(CheckSessionResult sessionState)
        {
            if (sessionState.Type == CheckSessionResultType.Changed)
            {
                var args = new UserSessionExpiringArgs();
                OnUserSessionExpiring?.Invoke(args);

                await UpdateUserState(null, !args.Cancel, true);
            }
        }

        public void Dispose()
        {
            Monitor.OnSessionChanged -= SessionChanged;
            Monitor?.Dispose();
        }
    }
}