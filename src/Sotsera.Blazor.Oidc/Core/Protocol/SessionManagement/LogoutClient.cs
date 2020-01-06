// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.TokenRevocation;
using Sotsera.Blazor.Oidc.Core.Storage;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement
{
    internal interface ILogoutClient
    {
        Task<LogoutRequest> CreateLogoutRequest(string idToken, Action<LogoutParameters> configureParameters);
        OidcRequest CreateBrowserRequest(LogoutRequest request);
        Task<OidcRequestState> ParseResponse(string url);
    }

    internal class LogoutClient: ThrowsErrors<LogoutClient>, ILogoutClient
    {
        private ILogoutRequestBuilder RequestBuilder { get; }
        private ILogoutResponseParser ResponseParser { get; }
        private IStore Store { get; }
        private ITokenRevocationClient RevocationClient { get; }
        private OidcHttpClient HttpClient { get; }
        protected override IOidcLogger<LogoutClient> Logger { get; }

        public LogoutClient(ILogoutRequestBuilder requestBuilder, ILogoutResponseParser responseParser, 
            IStore store, ITokenRevocationClient revocationClient, OidcHttpClient httpClient, IOidcLogger<LogoutClient> logger)
        {
            RequestBuilder = requestBuilder;
            ResponseParser = responseParser;
            Store = store;
            RevocationClient = revocationClient;
            HttpClient = httpClient;
            Logger = logger;
        }

        public Task<LogoutRequest> CreateLogoutRequest(string idToken, Action<LogoutParameters> configureParameters)
        {
            return HandleErrors(nameof(CreateLogoutRequest), async () =>
            {
                var parameters = await RequestBuilder.CreateLogoutParameters(idToken, configureParameters);
                var request = RequestBuilder.CreateLogoutRequest(parameters);

                if (request.Parameters.RevokeAccessTokenOnSignout)
                {
                    await RevocationClient.RevokeToken();
                }

                await Store.SetLogoutState(request.State);

                return request;
            });
        }
        
        public OidcRequest CreateBrowserRequest(LogoutRequest request)
        {
            return HandleErrors(nameof(CreateBrowserRequest), () => RequestBuilder.CreateBrowserRequest(request));
        }

        public Task<OidcRequestState> ParseResponse(string url)
        {
            return HandleErrors(nameof(ParseResponse), async () =>
            {
                var response = ResponseParser.ParseLogoutUrl(url);
                var state = await Store.GetLogoutState();
                
                ResponseParser.EnsureValidResponse(response, state);
                ResponseParser.EnsureValidState(response, state);
                
                await Store.RemoveLogoutState();

                return state.OidcRequestState;
            });
        }
    }
}