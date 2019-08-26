// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Core.Storage;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect
{
    internal interface IOidcClient
    {
        Task<AuthRequest> CreateAuthenticationRequest(Action<AuthParameters> configureParameters);
        OidcRequest CreatePopupRequest(AuthRequest request);
        Task<UserState> ParseResponse(string url);
    }

    internal class OidcClient: ThrowsErrors<OidcClient>, IOidcClient
    {
        private IAuthRequestBuilder RequestBuilder { get; }
        private IAuthResponseParser ResponseParser { get; }
        private IStore Store { get; }
        private HttpClient HttpClient { get; }
        protected override IOidcLogger<OidcClient> Logger { get; }

        public OidcClient(IAuthRequestBuilder requestBuilder, IAuthResponseParser responseParser, 
            IStore store, HttpClient httpClient, IOidcLogger<OidcClient> logger)
        {
            RequestBuilder = requestBuilder;
            ResponseParser = responseParser;
            Store = store;
            HttpClient = httpClient;
            Logger = logger;
        }

        public Task<AuthRequest> CreateAuthenticationRequest(Action<AuthParameters> configureParameters)
        {
            return HandleErrors(nameof(CreateAuthenticationRequest), async () =>
            {
                var parameters = await RequestBuilder.CreateAuthParameters(configureParameters);
                var request = RequestBuilder.CreateAuthRequest(parameters);

                await Store.SetAuthState(request.State);

                return request;
            });
        }

        public OidcRequest CreatePopupRequest(AuthRequest request)
        {
            return HandleErrors(nameof(CreatePopupRequest), () => RequestBuilder.CreatePopupRequest(request));
        }

        public Task<UserState> ParseResponse(string url)
        {
            return HandleErrors(nameof(ParseResponse), async () =>
            {
                var response = ResponseParser.ParseAuthenticationUrl(url);
                var state = await Store.GetAuthState();
                
                ResponseParser.EnsureValidResponse(response, state);
                ResponseParser.EnsureValidState(response, state);
                ResponseParser.EnsureTokenOrCodePresence(response, state);

                await Store.RemoveAuthState();

                if (state.IsCodeFlow)
                {
                    response.Merge(await RequestToken(response, state));
                }

                var token = await ResponseParser.ParseResponse(response, state);

                var claims = ResponseParser.ParsePayloadClaims(token);
                if(response.LoadUserInfo) claims.Merge(await LoadUserInfo(response.AccessToken));

                var userState = ResponseParser.ParseUserState(response, token, claims);

                return userState;
            });
        }

        public Task<JsonData> LoadUserInfo(string accessToken)
        {
            return HandleErrors(nameof(LoadUserInfo), async () =>
            {
                // TODO: check the errors --> https://openid.net/specs/openid-connect-core-1_0.html#UserInfoError
                // TODO: check the signature
                var request = await RequestBuilder.CreateUserInfoRequest();

                try
                {
                    var header = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpClient.DefaultRequestHeaders.Authorization = header;
                    return await HttpClient.GetJsonAsync<JsonData>(request.Url);
                }
                finally
                {
                    HttpClient.DefaultRequestHeaders.Authorization = null;
                }
            });
        }

        private Task<CodeResponse> RequestToken(AuthResponse response, AuthState state)
        {
            // TODO: review the specs
            return HandleErrors(nameof(RequestToken), async () =>
            {
                var request = await RequestBuilder.CreateCodeRequest(state, response);
                var httpResponse = await HttpClient.PostAsync(request.Url, request.Content);
                var content = await httpResponse.Content.ReadAsStringAsync();

                return httpResponse.IsSuccessStatusCode
                    ? Json.Deserialize<CodeResponse>(content, "code token response")
                    : throw Logger.Exception(FormatCodeErrors(content, httpResponse));
            });
        }

        private string FormatCodeErrors(string responseText, HttpResponseMessage response)
        {
            var error = Json.Deserialize<CodeTokenError>(responseText, "code token error");
            var description = error.Description.IsNotEmpty()
                ? error.Description
                : $"status code {(int) response.StatusCode} ({response.ReasonPhrase})";

            return $"Error retrieving the token: {description}";
        }
    }
}