// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading;
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
        void Start(UserState state);
        void Stop();
    }

    internal class SessionMonitor: ThrowsErrors<SessionMonitor>, ISessionMonitor
    {
        private OidcSettings Settings { get; }
        private Interop Interop { get; }
        private IMetadataService Metadata { get; }
        protected override IOidcLogger<SessionMonitor> Logger { get; }

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

        public void Start(UserState state)
        {
            HandleErrors(nameof(Start), () =>
            {
                Stop();
                
                Logger.ThrowIf(state?.SessionState == null, "Session monitor called with an empty session state");

                State = state;
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

        private async void TimerCallback(object timerState)
        {
            var url = await Metadata.CheckSessionIframe();

            var request = new OidcFrameRequest
            {
                Url = await Metadata.CheckSessionIframe(),
                Origin = new Uri(url).GetLeftPart(UriPartial.Authority),
                Message = $"{Settings.ClientId} {State.SessionState}",
                Timeout = Settings.CheckSessionTimeout.TotalMilliseconds
            };

            var result = await Interop.PostToSessionFrame(request);

            switch (result)
            {
                case "error":
                    if (Settings.CheckSessionStopOnError) Stop();
                    LogAndRaiseSessionEvent(CheckSessionResultType.Error, result);
                    break;
                case "changed":
                    Stop();
                    LogAndRaiseSessionEvent(CheckSessionResultType.Changed, result);
                    break;
                default:
                case "unchanged":
                    LogAndRaiseSessionEvent(CheckSessionResultType.Valid, result);
                    break;
            }
        }

        private void LogAndRaiseSessionEvent(CheckSessionResultType type, string message)
        {
            var result = new CheckSessionResult(type, message);

            switch (type)
            {
                case CheckSessionResultType.Error:
                    Logger.LogError(result.Message);
                    break;
                case CheckSessionResultType.Changed:
                    Logger.LogInformation(result.Message);
                    break;
                case CheckSessionResultType.Valid:
                    Logger.LogDebug(result.Message);
                    break;
            }

            OnSessionChanged?.Invoke(result);
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
