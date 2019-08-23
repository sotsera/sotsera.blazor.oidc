// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Protocol.Discovery.Model;
using Sotsera.Blazor.Oidc.Core.Tokens.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Tokens.Crypto.Algorithms
{
    internal class Ecdsa : SignatureValidator<Ecdsa>
    {
        public override string KeyType { get; } = "EC";

        public Ecdsa(TokenHeader header, JwsAlgorithm algorithm, ILogger<Ecdsa> logger) 
            : base(header, algorithm, logger)
        {
        }

        public override bool IsValid(byte[] signedBytes, byte[] signature, Jwk key)
        {
            var (hasher, algorithmName) = CreateHasher(key);

            using (hasher)
            {
                return hasher.VerifyData(signedBytes, signature, algorithmName);
            }
        }

        public override bool IsValidKey(Jwk key)
        {
            return key.X.IsNotEmpty() && key.Y.IsNotEmpty() && key.Crv.IsNotEmpty();
        }


        private (ECDsa, HashAlgorithmName) CreateHasher(Jwk key)
        {
            var (algorithmName, curve) = HasherParameters;

            var parameters = new ECParameters
            {
                Q = new ECPoint
                {
                    X = Base64Url.DeserializeBytes(key.X, "ECDSA key X value"),
                    Y = Base64Url.DeserializeBytes(key.Y, "ECDSA key Y value")
                },
                Curve = curve
            };

            return (ECDsa.Create(parameters), algorithmName);
        }

        private (HashAlgorithmName, ECCurve) HasherParameters
        {
            get
            {
                switch (Algorithm)
                {
                    case JwsAlgorithm.ES256: return (HashAlgorithmName.SHA256, ECCurve.NamedCurves.nistP256);
                    case JwsAlgorithm.ES384: return (HashAlgorithmName.SHA384, ECCurve.NamedCurves.nistP384);
                    case JwsAlgorithm.ES512: return (HashAlgorithmName.SHA512, ECCurve.NamedCurves.nistP521);
                    default: throw Logger.Exception($"Invalid algorithm \"{Algorithm}\" for {nameof(Ecdsa)}");
                }
            }
        }
    }
}