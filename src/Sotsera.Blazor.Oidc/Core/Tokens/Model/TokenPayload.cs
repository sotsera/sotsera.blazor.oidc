// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Tokens.Model
{
    //https://tools.ietf.org/html/rfc7519
    internal class TokenPayload
    {
        public JsonData Data { get; }

        public string Issuer { get; }
        public string[] Audience { get; }
        public string AuthorizedParty { get; }
        public string[] Amr { get; }
        public string Sub { get; }


        /// <summary>
        /// The NumericDate at which the JWT was issued
        /// </summary>
        public DateTimeOffset? IssuedAt { get; }

        /// <summary>
        /// The NumericDate before which the JWT MUST NOT be accepted for processing
        /// </summary>
        public DateTimeOffset? NotBefore  { get; }

        /// <summary>
        /// The NumericDate on or after which the JWT MUST NOT be accepted for processing
        /// </summary>
        public DateTimeOffset? ExpirationTime { get; }

        public TokenPayload(JsonData data)
        {
            Data = data ?? new JsonData();

            Issuer = Data.Value<string>("iss");
            Audience = Data.ValueAsArray<string>("aud");
            AuthorizedParty = Data.Value<string>("azp");
            IssuedAt = Time.FromNumericDate(Data.Value<int?>("iat"));
            NotBefore = Time.FromNumericDate(Data.Value<int?>("nbf"));
            ExpirationTime = Time.FromNumericDate(Data.Value<int?>("exp"));
            Amr = Data.ValueAsArray<string>("amr");
            Sub = Data.Value<string>("sub");
        }

        //public string at_hash { get; set; }
        //public string aud { get; set; }
        //public long auth_time { get; set; }
        //public string idp { get; set; }
        //public string nonce { get; set; }
        //public string sid { get; set; }
    }
}