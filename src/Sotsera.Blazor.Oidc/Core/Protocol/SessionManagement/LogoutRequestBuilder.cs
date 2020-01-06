// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement
{
    internal interface ILogoutRequestBuilder
    {
        Task<LogoutParameters> CreateLogoutParameters(string idToken, Action<LogoutParameters> configureParameters);
        LogoutRequest CreateLogoutRequest(LogoutParameters parameters);
        OidcRequest CreateBrowserRequest(LogoutRequest request);
    }

    internal class LogoutRequestBuilder: ThrowsErrors<LogoutRequestBuilder>,  ILogoutRequestBuilder
    {
        private OidcSettings Settings { get; }
        private IServiceProvider ServiceProvider { get; }
        private IMetadataService Metadata { get; }
        protected override IOidcLogger<LogoutRequestBuilder> Logger { get; }

        public LogoutRequestBuilder(OidcSettings settings, IServiceProvider serviceProvider, IOidcLogger<LogoutRequestBuilder> logger)
        {
            Settings = settings;
            ServiceProvider = serviceProvider;
            Metadata = serviceProvider.GetRequiredService<IMetadataService>();
            Logger = logger;
        }

        public Task<LogoutParameters> CreateLogoutParameters(string idToken, Action<LogoutParameters> configureParameters)
        {
            return HandleErrors(nameof(CreateLogoutParameters), async () =>
            {
                var parameters = await BuildParameters(idToken);
                Settings.PreLogout?.Invoke(parameters, ServiceProvider);
                configureParameters?.Invoke(parameters);
                EnsureValidParameters(parameters);
                return parameters;
            });
        }

        public LogoutRequest CreateLogoutRequest(LogoutParameters parameters)
        {
            return HandleErrors(nameof(CreateLogoutRequest), () =>
            {
                var requestState = CreateOidcRequestState(new Crypto(), parameters.StateData);
                var state = new LogoutState
                {
                    State = Base64Url.Serialize(requestState, "oidc logout request state")
                };
                var url = BuildLogoutUrl(parameters, state);

                return new LogoutRequest {Url = url, State = state, Parameters = parameters};
            });
        }

        public OidcRequest CreateBrowserRequest(LogoutRequest request)
        {
            return HandleErrors(nameof(CreateBrowserRequest), () => new OidcRequest
            {
                InteractionType = request.Parameters.InteractionType,
                Url = request.Url,
                Timeout = request.Parameters.OpenPopupTimeout.TotalMilliseconds,
                WindowName = request.Parameters.PopupWindowName,
                WindowFeatures = request.Parameters.PopupWindowFeatures
            });
        }

        private OidcRequestState CreateOidcRequestState(Crypto crypto, Dictionary<string, string> stateData)
        {
            return new OidcRequestState
            {
                Id = crypto.CreateUniqueHexadecimal(32), 
                Data = stateData.Keys.Count > 0 ? stateData : null
            };
        }

        private async Task<LogoutParameters> BuildParameters(string idToken)
        {
            var endSessionEndpoint = await Metadata.EndSessionEndpoint();
            
            return new LogoutParameters
            {
                EndSessionEndpoint = endSessionEndpoint,
                IdTokenHint = idToken,

                RevokeAccessTokenOnSignout = Settings.RevokeAccessTokenOnSignout,
                
                PopupWindowName = Settings.PopupWindowName,
                PopupWindowFeatures = Settings.PopupWindowFeatures,
                OpenPopupTimeout = Settings.OpenPopupTimeout,
                InteractionType = Settings.InteractionType,

                RedirectCallbackUri = Settings.LogoutRedirectCallbackUri,
                PopupCallbackUri = Settings.LogoutPopupCallbackUri,
                AdditionalParameters = Settings.AdditionalParameters ?? new NameValueCollection(),
                StateData = Settings.LogoutStateData ?? new Dictionary<string, string>()
            };
        }

        private void EnsureValidParameters(LogoutParameters parameters)
        {
            HandleErrors(nameof(EnsureValidParameters), () =>
            {
                if (parameters.EndSessionEndpoint.IsEmpty()) throw Logger.Exception("No End Session Endpoint passed");
            });
        }

        private string BuildLogoutUrl(LogoutParameters parameters, LogoutState state)
        {
            return new UrlBuilder(parameters.EndSessionEndpoint)
                .Add("post_logout_redirect_uri", parameters.RedirectUri)
                .Add("id_token_hint", parameters.IdTokenHint, parameters.RedirectUri.IsNotEmpty())
                .Add("state", state.State)
                .Add(parameters.AdditionalParameters)
                .ToString();
        }
    }
}