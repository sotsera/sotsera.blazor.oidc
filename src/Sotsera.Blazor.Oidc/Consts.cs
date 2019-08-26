// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc
{
    internal class Consts
    {
        internal class Oidc
        {
            internal static readonly string[] ValidResponseTypes = {"id_token token", "id_token", "code"};

            internal class Response
            {
                public const string Error = "error";
                public const string ErrorDescription = "error_description";
                public const string ErrorUri = "error_uri";
                public const string State = "state";

                internal class Authentication
                {
                    public const string AccessToken = "access_token";
                    public const string Code = "code";
                    public const string ExpiresIn = "expires_in";
                    public const string IdentityToken = "id_token";
                    public const string Scope = "scope";
                    public const string SessionState = "session_state";
                    public const string TokenType = "token_type";
                }
            }
        }
        
        internal class Interop
        {
            private const string Namespace = "sotsera.blazor.oidc";
            private const string Prefix = ".storage";

            public const string Init = Namespace + ".init";
            public const string PostToSessionFrame = Namespace + ".postToSessionFrame";
            public const string OpenPopup = Namespace + ".openPopup";
            public const string GetItem = Namespace + Prefix + ".getItem";
            public const string SetItem = Namespace + Prefix + ".setItem";
            public const string RemoveItem = Namespace + Prefix + ".removeItem";
        }

        /// <summary>
        /// https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
        /// </summary>
        internal class ClaimNames
        {
            // profile
            public const string Sub = "sub";
            public const string Name = "name";
            public const string GivenName = "given_name";
            public const string FamilyName = "family_name";
            public const string MiddleName = "middle_name";
            public const string Nickname = "nickname";
            public const string PreferredUsername = "preferred_username";
            public const string Profile = "profile";
            public const string Picture = "picture";
            public const string Website = "website";
            public const string Email = "email";
            public const string EmailVerified = "email_verified";
            public const string Gender = "gender";
            public const string Birthdate = "birthdate";
            public const string Zoneinfo = "zoneinfo";
            public const string Locale = "locale";
            public const string PhoneNumber = "phone_number";
            public const string PhoneNumberVerified = "phone_number_verified";
            public const string Address = "address";
            public const string UpdatedAt = "updated_at";

            // additional 
            public const string Role = "role";

            // protocol
            public const string Nonce = "nonce";
            public const string AtHash = "at_hash";
            public const string Iat = "iat";
            public const string Nbf = "nbf";
            public const string Exp = "exp";
            public const string Aud = "aud";
            public const string Iss = "iss";
            public const string CHash = "c_hash";

            public static readonly string[] ProtocolClaims =
            {
                Nonce, AtHash, Iat, Nbf, Exp, Aud, Iss, CHash
            };
        }
    }
}