// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model;
using Sotsera.Blazor.Oidc.Core.Tokens.Crypto.Algorithms;
using Sotsera.Blazor.Oidc.Core.Tokens.Model;

namespace Sotsera.Blazor.Oidc.Core.Tokens.Crypto
{
internal interface ISignatureValidator
    {
        void Validate(byte[] signedBytes, byte[] signature, IList<Jwk> keys);
    }

    internal abstract class SignatureValidator<T> : ISignatureValidator
    {
        public TokenHeader Header { get; }
        public JwsAlgorithm Algorithm { get; }
        public ILogger<T> Logger { get; }

        protected SignatureValidator(TokenHeader header, JwsAlgorithm algorithm, ILogger<T> logger)
        {
            Header = header;
            Algorithm = algorithm;
            Logger = logger;
        }

        public abstract string KeyType { get; }
        public abstract bool IsValid(byte[] signedBytes, byte[] signature, Jwk key);
        public abstract bool IsValidKey(Jwk key);

        // The JWS Signature value is not valid if there is not a key for use with that
        // algorithm associated with the party that digitally signed or MACed the
        // content (https://tools.ietf.org/html/rfc7515#section-4.1.1)
        public void Validate(byte[] signedBytes, byte[] signature, IList<Jwk> keys)
        {
            foreach (var key in SelectKeys(keys))
            {
                if (!IsValidKey(key)) continue;
                if (IsValid(signedBytes, signature, key))
                {
                    Logger.LogDebug($"Token signature validated with key ID: {key.Kid}");
                    return;
                }
            }

            throw Logger.Exception("The token signature is invalid");
        }

        private IEnumerable<Jwk> SelectKeys(IEnumerable<Jwk> keys)
        {
            var keyId = Header.Kid;

            if (keyId.IsNotEmpty())
            {
                //TODO: refresh the keys list when a kid is not present (https://openid.net/specs/openid-connect-core-1_0.html#RotateSigKeys)
                var key = keys.FirstOrDefault(x => x.Kid == keyId);

                if (key == null)
                {
                    var message = $"The token header requires the key id \"{keyId}\" but it is no present";
                    throw Logger.Exception(message);
                }

                if (key.Kty != KeyType)
                    throw Logger.Exception($"The key with id \"{keyId}\" doesn't have the valid \"{KeyType}\" kty");

                return new List<Jwk> {key};
            }

            var validKeys = keys?.Where(key => key.Kty == KeyType).ToList() ?? new List<Jwk>();
            if (validKeys.Count == 0)
            {
                const string noValidKeyPresent = "The token header requires the alg \"{0}\" but no key with kty \"{1}\" has been found";
                throw Logger.Exception(string.Format(noValidKeyPresent, Algorithm, KeyType));
            }

            return validKeys;
        }
    }
}