// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect.Model;
using Sotsera.Blazor.Oidc.Core.Tokens;
using Sotsera.Blazor.Oidc.Core.Tokens.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.OpenIdConnect
{
    internal interface IAuthResponseParser
    {
        AuthResponse ParseAuthenticationUrl(string url);
        void EnsureValidResponse(AuthResponse response, AuthState state);
        void EnsureValidState(AuthResponse response, AuthState state);
        void EnsureTokenOrCodePresence(AuthResponse response, AuthState state);
        Task<Token> ParseResponse(AuthResponse response, AuthState state);
        UserState ParseUserState(AuthResponse response, Token token, JsonData claims);
        JsonData ParsePayloadClaims(Token token);

    }

    internal class AuthResponseParser : ResponseParser<AuthResponseParser, AuthResponse>, IAuthResponseParser
    {
        private static readonly string[] ProtocolClaims = Consts.ClaimNames.ProtocolClaims;
        private OidcSettings Settings { get; }
        private ITokenParser TokenParser { get; }
        protected override IOidcLogger<AuthResponseParser> Logger { get; }

        public AuthResponseParser(OidcSettings settings, ITokenParser tokenParser, IOidcLogger<AuthResponseParser> logger)
        {
            Settings = settings;
            TokenParser = tokenParser;
            Logger = logger;
        }

        public AuthResponse ParseAuthenticationUrl(string url)
        {
            return ParseUrl(url, (response, data) =>
            {
                response.AccessToken = data.Get(Consts.Oidc.Response.Authentication.AccessToken);
                response.Code = data.Get(Consts.Oidc.Response.Authentication.Code);
                response.ExpiresAt = Time.ParseExpiration(data.Get(Consts.Oidc.Response.Authentication.ExpiresIn));
                response.IdToken = data.Get(Consts.Oidc.Response.Authentication.IdentityToken);
                response.Scope = data.Get(Consts.Oidc.Response.Authentication.Scope);
                response.SessionState = data.Get(Consts.Oidc.Response.Authentication.SessionState);
                response.TokenType = data.Get(Consts.Oidc.Response.Authentication.TokenType);
                response.LoadUserInfo = Settings.LoadUserInfo;
            });
        }

        public void EnsureValidResponse(AuthResponse response, AuthState state)
        {
            if (state.RequiredResponseMode != response.UrlParsingType)
            {
                var partName = Enum.GetName(typeof(UrlParsingType), state.RequiredResponseMode);
                throw Logger.Exception($"The Auth response was expected in the \"{partName}\" url part");
            }

            EnsureNoErrorsPresent(response);
        }

        public void EnsureValidState(AuthResponse response, AuthState state)
        {
            if (response.State.IsEmpty()) throw Logger.Exception("No authentication state in response");
            if (state == null) throw Logger.Exception("Storage does not contain the authentication state");
            if (state.State != response.State) throw Logger.Exception("Authentication state does not match with response");

            Logger.LogDebug("Authentication state is valid");
        }

        public void EnsureTokenOrCodePresence(AuthResponse response, AuthState state)
        {
            if (state.IsImplicitFlow && response.IdToken.IsEmpty())
                throw Logger.Exception("Response does not contain the identity token");

            if (!state.IsImplicitFlow && response.IdToken.IsNotEmpty())
                throw Logger.Exception("Unexpected identity token for a non Implicit flow request");

            if (state.IsCodeFlow && response.Code.IsEmpty())
                throw Logger.Exception("Response does not contain the code");

            if (!state.IsCodeFlow && response.Code.IsNotEmpty())
                throw Logger.Exception("Unexpected identity token for a non Code flow request");
        }

        public Task<Token> ParseResponse(AuthResponse response, AuthState state)
        {
            var validateSignature = !state.IsCodeFlow; // TODO: verify this
            return TokenParser.Parse(response.IdToken, state.Issuer, state.ClientId, validateSignature);
        }

        public UserState ParseUserState(AuthResponse response, Token token, JsonData claims)
        {
            return new UserState
            {
                AccessToken = response.AccessToken,
                ExpiresAt = response.ExpiresAt,
                IdToken = response.IdToken,
                Scopes = response.Scopes,
                SessionState = response.SessionState,
                State = response.State,
                TokenType = response.TokenType,
                User = new OidcUser(claims, token.Payload.Issuer, Settings.NameClaimKey, Settings.RoleClaimKey)
            };
        }

        public JsonData ParsePayloadClaims(Token token)
        {
            var claims = new JsonData();

            foreach (var claim in token.Payload.Data)
            {
                if (!Settings.FilterProtocolClaims || !ProtocolClaims.Contains(claim.Key))
                {
                    claims.Add(claim.Key, claim.Value);
                }
            }

            return claims;
        }
    }
}