// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Core.Tokens.Crypto.Algorithms
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc7518#section-3.1
    /// Set of "alg" (algorithm) Header Parameter values defined for use with JWS
    /// </summary>
    internal enum JwsAlgorithm
    {
        /// <summary>
        /// HMAC using SHA-256
        /// </summary>
        HS256,

        /// <summary>
        ///  HMAC using SHA-384
        /// </summary>
        HS384,

        /// <summary>
        /// HMAC using SHA-512
        /// </summary>
        HS512,

        /// <summary>
        /// RSASSA-PKCS1-v1_5 using SHA-256
        /// </summary>
        RS256,

        /// <summary>
        /// RSASSA-PKCS1-v1_5 using SHA-384
        /// </summary>
        RS384,

        /// <summary>
        /// RSASSA-PKCS1-v1_5 using SHA-512
        /// </summary>
        RS512,

        /// <summary>
        /// ECDSA using P-256 and SHA-256
        /// </summary>
        ES256,

        /// <summary>
        /// ECDSA using P-384 and SHA-384
        /// </summary>
        ES384,

        /// <summary>
        /// ECDSA using P-521 and SHA-512
        /// </summary>
        ES512,

        /// <summary>
        /// RSASSA-PSS using SHA-256 and MGF1 with SHA-256
        /// </summary>
        PS256,

        /// <summary>
        /// RSASSA-PSS using SHA-384 and MGF1 with SHA-384
        /// </summary>
        PS384,

        /// <summary>
        /// RSASSA-PSS using SHA-512 and MGF1 with SHA-512
        /// </summary>
        PS512,

        /// <summary>
        /// No digital signature or MAC performed
        /// </summary>
        none,

        /// <summary>
        /// Fake type used for validation in case of an empty or unknown algorithm type
        /// </summary>
        Unknown,
    }
}