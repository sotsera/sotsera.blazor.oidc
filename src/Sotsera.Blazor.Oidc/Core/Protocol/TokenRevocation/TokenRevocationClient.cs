// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Threading.Tasks;

namespace Sotsera.Blazor.Oidc.Core.Protocol.TokenRevocation
{
    internal interface ITokenRevocationClient
    {
        Task RevokeToken();
    }

    // It informs the token service that a token will not be used anymore.
    // It doesn't matter on client side because jwt tokens cannot be revoked so they can just be deleted on the client.
    internal class TokenRevocationClient: ITokenRevocationClient
    {
        public Task RevokeToken()
        {
            // Should revoke the access token
            // should not throw but just log an error so the logout can continue.
            return Task.CompletedTask;
        }
    }
}