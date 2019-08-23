// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0
namespace Sotsera.Blazor.Oidc.Core.Tokens.Model
{
    /// <summary>
    /// JOSE (JSON Object Signing and Encryption) Header
    /// <remarks>https://tools.ietf.org/html/rfc7515#section-4</remarks>
    /// </summary>
    //TODO: use the annotations on preview 6
    internal class TokenHeader
    {
        public string Alg { get; set; }
        public object Crit { get; set; }
        public object Enc { get; set; }
        public string Kid { get; set; }
        public string Typ { get; set; }
        //public string Algorithm => Value<string>("alg");
        //public object Crit => Value<object>("crit");
        //public string Kid => Value<string>("kid");
        //public string Typ => Value<string>("typ");
    }
}