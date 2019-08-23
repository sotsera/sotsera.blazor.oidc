// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Core.Protocol.Common;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement
{
    internal interface ILogoutResponseParser
    {
        LogoutResponse ParseLogoutUrl(string url);
        void EnsureValidResponse(LogoutResponse response, LogoutState state);
        void EnsureValidState(LogoutResponse response, LogoutState state);
    }
    
    internal class LogoutResponseParser: ResponseParser<LogoutResponseParser, LogoutResponse>, ILogoutResponseParser
    {
        protected override IOidcLogger<LogoutResponseParser> Logger { get; }

        public LogoutResponseParser(IOidcLogger<LogoutResponseParser> logger)
        {
            Logger = logger;
        }

        public LogoutResponse ParseLogoutUrl(string url) => ParseUrl(url);
        
        public void EnsureValidResponse(LogoutResponse response, LogoutState state)
        {
            if (response.UrlParsingType != UrlParsingType.Query)
            {
                var partName = Enum.GetName(typeof(UrlParsingType), UrlParsingType.Query);
                throw Logger.Exception($"The Logout response was expected in the \"{partName}\" url part");
            }

            EnsureNoErrorsPresent(response);
        }

        public void EnsureValidState(LogoutResponse response, LogoutState state)
        {
            if (response.State.IsEmpty())
            {
                Logger.LogDebug("No logout state in response");
                return;
            }
            if (state == null) throw Logger.Exception("Storage does not contain the logout state");
            if (state.State != response.State) throw Logger.Exception("Logout state does not match with response");

            Logger.LogDebug("Logout state is valid");
        }
    }
}