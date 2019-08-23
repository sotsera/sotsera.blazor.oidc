// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery;
using Sotsera.Blazor.Oidc.Core.Tokens.Crypto.Algorithms;
using Sotsera.Blazor.Oidc.Core.Tokens.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Tokens
{
    internal interface IJwtValidator
    {
        void ValidateFormat(string token, int maxTokenSize);
        void ValidateJoseHeader(Jws jws);
        Task ValidateSignature(Jws jws);
        void ValidatePayload(TokenPayload payload, string issuer, string audience, TimeSpan clockSkew);
    }

    internal class JwtValidator : IJwtValidator
    {
        private const string JwsFormatRegex = @"^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]*$";
        private static readonly Regex JwsRegex = new Regex(JwsFormatRegex, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));

        private ISignatureValidatorFactory ValidatorFactory { get; }
        private IMetadataService Metadata { get; }
        private ILogger<JwtValidator> Logger { get; }

        public JwtValidator(ISignatureValidatorFactory validatorFactory, IMetadataService metadata,
            ILogger<JwtValidator> logger)
        {
            ValidatorFactory = validatorFactory;
            Metadata = metadata;
            Logger = logger;
        }

        public void ValidateFormat(string token, int maxTokenSize)
        {
            using (Logger.BeginScope("ValidateFormat"))
            {
                Logger.ThrowIf(token.IsEmpty(), "Missing token");

                const string maxLengthError = "The token lenght ({0} bytes) exceeds the max allowed length: {1} bytes";
                Logger.ThrowIf(token.IsEmpty(), maxLengthError, token.Length, maxTokenSize);

                Logger.ThrowIf(!JwsRegex.IsMatch(token), "The token contains invalid chars or does not contain 2 parts separed by dots");
            }
        }

        // TODO: double check https://tools.ietf.org/html/rfc7515#section-5.2 point 5
        public void ValidateJoseHeader(Jws jws)
        {
            // The JWS Signature value is not valid if the "alg" value does not represent
            // a supported algorithm (https://tools.ietf.org/html/rfc7515#section-4.1.1)
            Logger.ThrowIf(jws.Header.Alg.IsEmpty(), "Missing JWS Algorithm");
            Logger.ThrowIf(jws.Algorithm == JwsAlgorithm.Unknown, "Unknown JWS Algorithm: {0}", jws.Header.Alg);

            // TODO: During this step, verify that the resulting JOSE Header does not contain duplicate Header Parameter names.
            // https://tools.ietf.org/html/rfc7515#section-5.2 - Point 4
            // With the current json implementation identifying these duplications is not possible

            Logger.ThrowIf(jws.Header.Crit != null, "JWS \"crit\" extensions are not supported yet");

            // https://tools.ietf.org/html/rfc7516#section-9
            Logger.ThrowIf(jws.Header.Enc != null, "The JWS header contains \"enc\" key which is used by JWE tokens");
        }

        public async Task ValidateSignature(Jws jws)
        {
            if (jws.Algorithm == JwsAlgorithm.none) return;

            var signedBytes = GetBytes(jws.RawSignedPart);
            var signature = Base64Url.DeserializeBytes(jws.RawSignature, "Token signature");

            var validator = ValidatorFactory.Create(jws.Header, jws.Algorithm);

            var keys = (await Metadata.JsonWebKeys()).Keys;
            validator.Validate(signedBytes, signature, keys);
        }

        public void ValidatePayload(TokenPayload payload, string issuer, string audience, TimeSpan clockSkew)
        {
            using (Logger.BeginScope(nameof(ValidatePayload)))
            {
                if (payload == null) throw Logger.Exception("Payload not provided");

                var tokenIssuer = payload.Issuer;
                if (tokenIssuer.IsEmpty()) throw Logger.Exception("Issuer (iss) was not provided");
                if (tokenIssuer != issuer) throw Logger.Exception($"Invalid issuer in token: {tokenIssuer}");

                var tokenAudience = payload.Audience;
                if (tokenAudience.IsEmpty()) throw Logger.Exception("Audience (aud) was not provided");
                if (!tokenAudience.Contains(audience)) throw Logger.Exception($"Invalid audience in token: [{string.Join(", ", tokenAudience)}]");

                var azp = payload.AuthorizedParty;
                if (azp.IsNotEmpty() && azp != audience) throw Logger.Exception($"Invalid azp in token: {azp}");

                var now = DateTimeOffset.Now;
                var lowerBoundary = now.Subtract(clockSkew);
                var upperBoundary = now.Add(clockSkew);

                var iat = payload.IssuedAt;
                if(!iat.HasValue) throw Logger.Exception("IssuedAt (iat) was not provided");
                if (iat.Value > upperBoundary) throw Logger.Exception("IssuedAt (iat) is in the future: " + iat);

                var nbf = payload.NotBefore;
                if (nbf.HasValue && nbf.Value > upperBoundary) throw Logger.Exception("NotBefore (nbf) is in the future: " + nbf);

                var exp = payload.ExpirationTime;
                if(!exp.HasValue) throw Logger.Exception("ExpirationTime (exp) was not provided");
                if (exp.Value < lowerBoundary) throw Logger.Exception("ExpirationTime (exp) is in the past: " + iat);

                //---->
                // Riparti da if (state.nonce && state.nonce !== payload.nonce) {
                // Dove lo metto il resto delle validazioni??
            }
        }

        private byte[] GetBytes(string value)
        {
            try
            {
                return Encoding.UTF8.GetBytes(value);
            }
            catch (Exception ex)
            {
                const string template = "Error converting the token signed part (header.payload) to bytes: {0}";
                throw Logger.Exception(template, ex.Message);
            }
        }
    }
}