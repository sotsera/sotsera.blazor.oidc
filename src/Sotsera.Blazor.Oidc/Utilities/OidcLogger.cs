// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using Microsoft.Extensions.Logging;

namespace Sotsera.Blazor.Oidc.Utilities
{
    internal interface IOidcLogger<T> : ILogger<T>
    {

    }

    internal class OidcLogger<T>: IOidcLogger<T>
    {
        private OidcSettings Settings { get; }
        private ILogger<T> RealLogger { get; }

        public OidcLogger(OidcSettings settings, ILoggerFactory loggerFactory)
        {
            Settings = settings;
            RealLogger = loggerFactory.CreateLogger<T>();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            RealLogger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= Settings.MinimumLogeLevel;
        public IDisposable BeginScope<TState>(TState state) => RealLogger.BeginScope(state);
    }
}