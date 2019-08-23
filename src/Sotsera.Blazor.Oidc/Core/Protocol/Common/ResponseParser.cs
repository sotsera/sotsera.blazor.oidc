// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.Common
{
    internal delegate void UrlParser<in T>(T type, Dictionary<string, string> data);

    internal abstract class ResponseParser<T, TResponse>: ThrowsErrors<T> where TResponse : OidcResponse, new()
    {
        protected TResponse ParseUrl(string url, UrlParser<TResponse> parser = null)
        {
            var parts = new UrlParser().Parse(url);

            return ParseParts(parts, UrlParsingType.Fragment, parser)
                   ?? ParseParts(parts, UrlParsingType.Query, parser);
        }

        protected TResponse ParseParts( ResponseUri parts,  UrlParsingType type, UrlParser<TResponse> parser)
        {
            var data = type == UrlParsingType.Query ? parts.Query : parts.Fragment;

            if (data.IsEmpty()) return null;

            var response = new TResponse
            {
                UrlParsingType = type,
                Error = data.Get(Consts.Oidc.Response.Error),
                ErrorDescription = data.Get(Consts.Oidc.Response.ErrorDescription),
                ErrorUri = data.Get(Consts.Oidc.Response.ErrorUri),
                State = data.Get(Consts.Oidc.Response.State)
            };

            parser?.Invoke(response, data);
            return response;
        }

        public void EnsureNoErrorsPresent(OidcResponse response)
        {
            if (!response.HasErrors) return;
            Logger.LogError(response.GetErrors());
            throw Logger.Exception("The response contains errors");
        }
    }
}