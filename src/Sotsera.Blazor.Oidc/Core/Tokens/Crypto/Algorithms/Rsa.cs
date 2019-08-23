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
    internal class Rsa : SignatureValidator<Rsa>
    {
        public override string KeyType { get; } = "RSA";

        public Rsa(TokenHeader header, JwsAlgorithm algorithm, ILogger<Rsa> logger) 
            : base(header, algorithm, logger)
        {
        }

        public override bool IsValid(byte[] signedBytes, byte[] signature, Jwk key)
        {
            var (hasher, algorithName) = CreateHasher();

            using (hasher)
            {
                var hash = hasher.ComputeHash(signedBytes);

                using var provider = new RSACryptoServiceProvider();
                provider.ImportParameters(new RSAParameters
                {
                    Modulus = Base64Url.DeserializeBytes(key.N, "RSA key modulus"),
                    Exponent = Base64Url.DeserializeBytes(key.E, "RSA key exponent")
                });
                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(provider);
                rsaDeformatter.SetHashAlgorithm(algorithName);
                return rsaDeformatter.VerifySignature(hash, signature);
            }
        }

        public override bool IsValidKey(Jwk key)
        {
            return key.N.IsNotEmpty() && key.E.IsNotEmpty();
        }

        private (HashAlgorithm, string) CreateHasher()
        {
            switch (Algorithm)
            {
                case JwsAlgorithm.RS256: return (SHA256.Create(), "SHA256");
                case JwsAlgorithm.RS384: return (SHA384.Create(), "SHA384");
                case JwsAlgorithm.RS512: return (SHA512.Create(), "SHA512");
                default: throw Logger.Exception($"Invalid algorithm \"{Algorithm}\" for {nameof(Rsa)}");
            }
        }
    }
}