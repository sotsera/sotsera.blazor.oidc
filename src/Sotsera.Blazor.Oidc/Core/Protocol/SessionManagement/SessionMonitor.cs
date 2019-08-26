// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.BrowserInterop;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement
{
    public interface ISessionMonitor: IDisposable
    {
        Task Start(UserState state);
        void Stop();
        Task<CheckSessionResult> CheckSession(UserState state);
        event Action<CheckSessionResult> OnSessionChanged;
    }

    internal class SessionMonitor: ThrowsErrors<SessionMonitor>, ISessionMonitor
    {
        private OidcSettings Settings { get; }
        private Interop Interop { get; }
        private IMetadataService Metadata { get; }
        protected override IOidcLogger<SessionMonitor> Logger { get; }

        private bool Initialized { get; set; }
        private UserState State { get; set; }
        private Timer Timer { get; }
        
        public event Action<CheckSessionResult> OnSessionChanged;

        public SessionMonitor(OidcSettings settings, Interop interop, IMetadataService metadata, IOidcLogger<SessionMonitor> logger)
        {
            Settings = settings;
            Interop = interop;
            Metadata = metadata;
            Logger = logger;
            Timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        public Task Start(UserState state)
        {
            return HandleErrors(nameof(Start), async () =>
            {
                await Init(state);

                Timer.Change(Settings.CheckSessionInterval, Settings.CheckSessionInterval);
            });
        }

        public void Stop()
        {
            HandleErrors(nameof(Stop), () =>
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
            });
        }

        public async Task<CheckSessionResult> CheckSession(UserState state)
        {
            await Init(state);
            return await PostMessage();
        }

        private async Task<CheckSessionResult> PostMessage()
        {
            var sessionState = await Interop.PostToSessionFrame($"{Settings.ClientId} {State.SessionState}");

            switch (sessionState)
            {
                case "error":
                    return new CheckSessionResult(CheckSessionResultType.Error, sessionState);
                case "changed":
                    return new CheckSessionResult(CheckSessionResultType.Changed, sessionState);
                default:
                case "unchanged":
                    return new CheckSessionResult(CheckSessionResultType.Valid, sessionState);
            }
        }

        private async Task Init(UserState state)
        {               
            Stop();
            Logger.ThrowIf(state?.SessionState == null, "Session monitor called with an empty session state");
            State = state;

            if (Initialized) return;

            var url = await Metadata.CheckSessionIframe();

            var settings = new FrameSettings
            {
                Url = await Metadata.CheckSessionIframe(),
                Origin = new Uri(url).GetLeftPart(UriPartial.Authority),
                Timeout = Settings.CheckSessionTimeout.TotalMilliseconds
            };

            await Interop.InitSessionFrame(settings);

            Initialized = true;
        }

        private async void TimerCallback(object timerState)
        {
            var result = await PostMessage();

            switch (result.Type)
            {
                case CheckSessionResultType.Error:
                    if (Settings.CheckSessionStopOnError) Stop();
                    Logger.LogError(result.Message);
                    OnSessionChanged?.Invoke(result);
                    break;
                case CheckSessionResultType.Changed:
                    Stop();
                    Logger.LogInformation(result.Message);
                    OnSessionChanged?.Invoke(result);
                    break;
                case CheckSessionResultType.Valid:
                    Logger.LogDebug(result.Message);
                    break;
            }
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
