// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model;
using Sotsera.Blazor.Oidc.Core.Tokens.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Tokens.Crypto.Algorithms
{
    internal class Hmac : SignatureValidator<Hmac>
    {
        public override string KeyType { get; } = "oct";

        public Hmac(TokenHeader header, JwsAlgorithm algorithm, ILogger<Hmac> logger) 
            : base(header, algorithm, logger)
        {
        }

        public override bool IsValid(byte[] signedBytes, byte[] signature, Jwk key)
        {
            using var hasher = CreateHasher(key);
            var hash = hasher.ComputeHash(signedBytes);
            return signature.SequenceEqual(hash);
        }

        public override bool IsValidKey(Jwk key)
        {
            return key.K.IsNotEmpty();
        }

        private HMAC CreateHasher(Jwk key)
        {
            var keyBytes = Base64Url.DeserializeBytes(key.K, "HMAC signature key");
            switch (Algorithm)
            {
                case JwsAlgorithm.HS256: return new HMACSHA256(keyBytes);
                case JwsAlgorithm.HS384: return new HMACSHA384(keyBytes);
                case JwsAlgorithm.HS512: return new HMACSHA512(keyBytes);
                default: throw Logger.Exception($"Invalid algorithm \"{Algorithm}\" for {nameof(Hmac)}");
            }
        }
    }
}