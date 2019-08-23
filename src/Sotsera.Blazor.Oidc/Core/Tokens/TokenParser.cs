// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Tokens.Crypto.Algorithms;
using Sotsera.Blazor.Oidc.Core.Tokens.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Tokens
{
    internal interface ITokenParser
    {
        Task<Token> Parse(string token, string issuer, string audience, bool validateSignature);
    }

    internal class TokenParser : ITokenParser
    {
        private OidcSettings Settings { get; }
        private IJwtValidator Validator { get; }
        private ILogger<TokenParser> Logger { get; }

        public TokenParser(OidcSettings settings, IJwtValidator validator, ILogger<TokenParser> logger)
        {
            Settings = settings;
            Validator = validator;
            Logger = logger;
        }

        public async Task<Token> Parse(string token, string issuer, string audience, bool validateSignature)
        {
            using (Logger.BeginScope(nameof(Parse)))
            {
                try
                {
                    Validator.ValidateFormat(token, Settings.MaxTokenSize);

                    var jws = ParseJws(token);

                    Validator.ValidateJoseHeader(jws);

                    var payloadClaims = Base64Url.DeserializeData(jws.Parts[1], "JWT payload");
                    var payload = new TokenPayload(payloadClaims);
                    
                    Validator.ValidatePayload(payload, issuer, audience, Settings.ClockSkew);

                    if (validateSignature) await Validator.ValidateSignature(jws);

                    return new Token
                    {
                        Algorithm = jws.Algorithm, 
                        Header = jws.Header, 
                        Payload = payload
                    };
                }
                catch (Exception ex)
                {
                    throw Logger.Exception($"Error parsing the Json Web Token: {ex.Message}");
                }
            }
        }

        private Jws ParseJws(string token)
        {
            var tokenParts = token.Split('.');
            var header = Base64Url.Deserialize<TokenHeader>(tokenParts[0], "JWT token header");
            var algorithm = ParseJwsAlgorithm(header.Alg);

            return new Jws
            {
                Parts = tokenParts,
                Header = header,
                Algorithm = algorithm
            };
        }

        private JwsAlgorithm ParseJwsAlgorithm(string algorithmName)
        {
            return Enum.TryParse(algorithmName, out JwsAlgorithm algorithm)
                ? algorithm
                : JwsAlgorithm.Unknown;
        }
    }
}