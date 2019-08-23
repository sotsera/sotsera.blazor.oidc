// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect
{
    internal interface IAuthRequestBuilder
    {
        Task<AuthParameters> CreateAuthParameters(Action<AuthParameters> configureParameters);
        AuthRequest CreateAuthRequest(AuthParameters parameters);
        OidcRequest CreatePopupRequest(AuthRequest request);
        Task<TokenRequest> CreateCodeRequest(AuthState state, AuthResponse code);
        Task<UserInfoRequest> CreateUserInfoRequest();
    }

    internal class AuthRequestBuilder: ThrowsErrors<AuthRequestBuilder>, IAuthRequestBuilder
    {
        private OidcSettings Settings { get; }
        private IMetadataService Metadata { get; }
        protected override IOidcLogger<AuthRequestBuilder> Logger { get; }

        public AuthRequestBuilder(OidcSettings settings, IMetadataService metadata, IOidcLogger<AuthRequestBuilder> logger)
        {
            Settings = settings;
            Metadata = metadata;
            Logger = logger;
        }

        public Task<AuthParameters> CreateAuthParameters(Action<AuthParameters> configureParameters)
        {
            return HandleErrors(nameof(CreateAuthParameters), async () =>
            {
                var parameters = await BuildParameters();
                configureParameters?.Invoke(parameters);
                EnsureValidParameters(parameters);
                return parameters;
            });
        }
        
        public void EnsureValidParameters(AuthParameters parameters)
        {
            HandleErrors(nameof(EnsureValidParameters), () =>
            {
                var validResopnseTypes = Consts.Oidc.ValidResponseTypes;

                if (parameters.AuthorizationEndpoint.IsEmpty()) throw Logger.Exception("No Authorization Endpoint passed");
                if (parameters.ClientId.IsEmpty()) throw Logger.Exception("No ClientId passed");
                if (parameters.RedirectUri.IsEmpty()) throw Logger.Exception("No RedirectUri passed");
                if (parameters.ResponseType.IsEmpty()) throw Logger.Exception("No ResponseType passed");
                if (parameters.Scope.IsEmpty()) throw Logger.Exception("No Scope passed");
                if (parameters.Issuer.IsEmpty()) throw Logger.Exception("No Issuer passed");

                if (!validResopnseTypes.Any(x => x.Equals(parameters.ResponseType)))
                {
                    throw Logger.Exception("Response type must be " + string.Join(" OR ", validResopnseTypes));
                }

                if (Settings.StorageType.IsMemory() && parameters.InteractionType.IsRedirect())
                {
                    throw Logger.Exception("Interaction type cannot be redirect using memory storage");
                }
            });
        }

        public AuthRequest CreateAuthRequest(AuthParameters parameters)
        {
            return HandleErrors(nameof(CreateAuthRequest), () =>
            {
                var state = BuildRequestState(parameters);
                var url = BuildAuthenticationUrl(parameters, state);

                return new AuthRequest {Url = url, State = state, Parameters = parameters};
            });
        }

        public OidcRequest CreatePopupRequest(AuthRequest request)
        {
            return HandleErrors(nameof(CreatePopupRequest), () => new OidcRequest
            {
                Url = request.Url,
                Timeout = request.Parameters.OpenPopupTimeout.Milliseconds,
                WindowName = request.Parameters.PopupWindowName,
                WindowFeatures = request.Parameters.PopupWindowFeatures
            });
        }

        public async Task<TokenRequest> CreateCodeRequest(AuthState state, AuthResponse response)
        {
            return new TokenRequest
            {
                Url = await Metadata.TokenEndpoint(),
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["client_id"] = state.ClientId,
                    //["client_secret"] = Parameters.Client.ClientSecret,// TODO: should this be in the AthenticationParameters?
                    ["code_verifier"] = state.CodeVerifier,
                    ["code"] = response.Code,
                    ["redirect_uri"] = state.RedirectUri
                })
            };
        }

        public async Task<UserInfoRequest> CreateUserInfoRequest()
        {
            return new UserInfoRequest
            {
                Url = await Metadata.UserinfoEndpoint()
            };
        }

        private string BuildAuthenticationUrl(AuthParameters parameters, AuthState state)
        {
            return new UrlBuilder(parameters.AuthorizationEndpoint)
                .Add("scope", parameters.Scope)
                .Add("response_type", parameters.ResponseType)
                .Add("client_id", parameters.ClientId)
                .Add("redirect_uri", parameters.RedirectUri)
                .Add("state", state.State)
                .Add("response_mode", parameters.ResponseMode)
                .Add("display", parameters.Display.Format())
                .Add("prompt", parameters.Prompt.Format())
                .Add("max_age", parameters.MaxAge.HasValue ? parameters.MaxAge.ToString() : null)
                .Add("ui_locales", parameters.UiLocales)
                .Add("id_token_hint", parameters.IdTokenHint)
                .Add("login_hint", parameters.LoginHint)
                .Add("resource", parameters.Resource)
                .Add("request", parameters.Request)
                .Add("request_uri", parameters.RequestUri)
                .Add("nonce", state.Nonce, parameters.RequiresNonce)
                .Add("code_challenge", state.CodeChallenge, parameters.RequiresCodeVerifier)
                .Add("code_challenge_method", "S256", parameters.RequiresCodeVerifier)
                .Add(parameters.AdditionalParameters)
                .ToString();
        }

        private AuthState BuildRequestState(AuthParameters parameters)
        {
            var crypto = new Crypto();
            var state = new AuthState
            {
                Issuer = parameters.Issuer,
                ClientId = parameters.ClientId,
                ResponseType = parameters.ResponseType,
                State = crypto.CreateUniqueHexadecimal(32),
                RedirectUri = parameters.RedirectUri,
                ResponseMode = parameters.ResponseMode
            };

            if (parameters.RequiresNonce)
            {
                state.Nonce = crypto.CreateUniqueHexadecimal(32);
            }

            if (parameters.RequiresCodeVerifier)
            {
                state.CodeVerifier = Base64Url.SerializeBytes(crypto.CreateUniqueBytes(96), "code verifier");
                
                var challenge = crypto.ToSha256(Encoding.ASCII.GetBytes(state.CodeVerifier));
                state.CodeChallenge = Base64Url.SerializeBytes(challenge, "code challenge");
            }

            return state;
        }

        private async Task<AuthParameters> BuildParameters()
        {
            var authorizationEndpoint = await Metadata.AuthorizationEndpoint();
            
            return new AuthParameters
            {
                Issuer = await Metadata.Issuer(),
                ClientId = Settings.ClientId,
                ResponseType = Settings.ResponseType,
                ResponseTypes = ParseResponseTypes(Settings.ResponseType),
                ResponseMode = Settings.ResponseMode,

                AuthorizationEndpoint = authorizationEndpoint,

                Scope = Settings.Scope,
                Prompt = Settings.Prompt,
                Display = Settings.Display,
                MaxAge = Settings.MaxAge,
                UiLocales = Settings.UiLocales,
                AcrValues = Settings.AcrValues,
                AdditionalParameters = Settings.AdditionalParameters ?? new NameValueCollection(),

                PopupWindowName = Settings.PopupWindowName,
                PopupWindowFeatures = Settings.PopupWindowFeatures,
                OpenPopupTimeout = Settings.OpenPopupTimeout,
                InteractionType = Settings.InteractionType,

                RedirectCallbackUri = Settings.AuthenticationRedirectCallbackUri,
                PopupCallbackUri = Settings.AuthenticationPopupCallbackUri
            };
        }

        private string[] ParseResponseTypes(string responseType)
        {
            return responseType.IsEmpty() 
                ? new string[0] 
                : responseType.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }
    }
}