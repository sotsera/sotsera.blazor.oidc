// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Sotsera.Blazor.Oidc.Core.Common;

namespace Sotsera.Blazor.Oidc
{
    public class OidcUser
    {
        public JsonData Data { get; set; }
        public string Issuer { get; set; }
        public string NameClaimKey { get; set; }
        public string RolesClaimKey { get; set; }

        public string Sub => Data.Value<string>(Consts.ClaimNames.Sub);
        public string Name => Data.Value<string>(Consts.ClaimNames.Name);
        public string Email => Data.Value<string>(Consts.ClaimNames.Email);
        public string[] Roles => Data.ValueAsArray<string>(RolesClaimKey);

        public OidcUser()
        {
        }

        public OidcUser(JsonData data, string issuer, string nameClaimKey, string rolesClaimKey)
        {
            Data = data ?? new JsonData();
            Issuer = issuer;
            NameClaimKey = nameClaimKey.IsEmpty() ? Consts.ClaimNames.Name : nameClaimKey;
            RolesClaimKey = rolesClaimKey.IsEmpty() ? Consts.ClaimNames.Role : rolesClaimKey;
        }

        public T Value<T>(string key) => Data.Value<T>(key);

        public T[] ValueAsArray<T>(string key) => Data.ValueAsArray<T>(key);
    }

    internal static class UserExtensions
    {
        public static AuthenticationState ToAuthenticationState(this OidcUser user)
        {
            var principal = new ClaimsPrincipal(user.ToIdentity());
            return new AuthenticationState(principal);
        }

        public static IIdentity ToIdentity(this OidcUser user)
        {
            var identity = new ClaimsIdentity();

            if (user == null || user.Data.IsEmpty()) return identity;

            var claims = new List<Claim>
            {
                CreateClaim(user, user.NameClaimKey, user.Value<string>(user.NameClaimKey), true)
            };

            var roles = user.Data.ValueAsArray<string>(user.RolesClaimKey);

            if(roles.IsNotEmpty())
            {
                foreach (var role in roles)
                {
                    claims.AddIfNotEmpty(CreateClaim(user, user.RolesClaimKey, role, false));
                }
            }

            return new ClaimsIdentity(claims, "OpenIdConnect", user.NameClaimKey, user.RolesClaimKey);
        }

        private static Claim CreateClaim(OidcUser user, string name, string value, bool required)
        {
            if (value.IsEmpty())
            {
                if (!required) return null;
                throw new OidcException($"OidcUser profile does not contain the claim \"{name}\"");
            }

            return new Claim(name, value, ClaimValueTypes.String, user.Issuer);
        }
    }
}