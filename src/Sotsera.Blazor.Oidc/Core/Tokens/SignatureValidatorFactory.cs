// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Tokens.Crypto;
using Sotsera.Blazor.Oidc.Core.Tokens.Crypto.Algorithms;
using Sotsera.Blazor.Oidc.Core.Tokens.Model;

namespace Sotsera.Blazor.Oidc.Core.Tokens
{
    internal interface ISignatureValidatorFactory
    {
        ISignatureValidator Create(TokenHeader header, JwsAlgorithm algorithm);
    }

    internal class SignatureValidatorFactory : ISignatureValidatorFactory
    {
        public IServiceProvider ServiceProvider { get; }
        public ILogger<SignatureValidatorFactory> Logger { get; }

        public SignatureValidatorFactory(IServiceProvider serviceProvider, ILogger<SignatureValidatorFactory> logger)
        {
            ServiceProvider = serviceProvider;
            Logger = logger;
        }

        public ISignatureValidator Create(TokenHeader header, JwsAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case JwsAlgorithm.RS256:
                case JwsAlgorithm.RS384:
                case JwsAlgorithm.RS512:
                    return new Rsa(header, algorithm, GetLogger<Rsa>());
                case JwsAlgorithm.HS256:
                case JwsAlgorithm.HS384:
                case JwsAlgorithm.HS512:
                    return new Hmac(header, algorithm, GetLogger<Hmac>());
                case JwsAlgorithm.ES256:
                case JwsAlgorithm.ES384:
                case JwsAlgorithm.ES512:
                    return new Ecdsa(header, algorithm, GetLogger<Ecdsa>());
                case JwsAlgorithm.PS256:
                case JwsAlgorithm.PS384:
                case JwsAlgorithm.PS512:
                    throw Logger.Exception($"The JWS signature algorithm {algorithm} is not supported yet");
                default:
                    throw Logger.Exception("Invalid JWS Algorithm");
            }
        }

        private ILogger<T> GetLogger<T>()
        {
            return ServiceProvider.GetRequiredService<ILogger<T>>();
        }
    }
}