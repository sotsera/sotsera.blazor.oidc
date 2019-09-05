// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sotsera.Blazor.Oidc.Utilities;

namespace Sotsera.Blazor.Oidc.Core.Common
{
    internal abstract class ThrowsErrors<T>
    {
        private string ClassName { get; }
        protected abstract IOidcLogger<T> Logger { get; }

        protected ThrowsErrors()
        {
            ClassName = typeof(T).Name;
        }

        protected void HandleErrors(string methodName, Action func)
        {
            Logger.LogTrace($"{ClassName}.{methodName}");

            if(func == null) throw new ArgumentNullException(nameof(func));
            
            try
            {
                func();
            }
            catch (Exception ex)
            {
                throw LoggedException(methodName, ex);
            }
        }

        protected TResult HandleErrors<TResult>(string methodName, Func<TResult> func)
        {
            Logger.LogTrace($"{ClassName}.{methodName}");

            if(func == null) throw new ArgumentNullException(nameof(func));
            
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                throw LoggedException(methodName, ex);
            }
        }

        protected Task HandleErrors(string methodName, Func<Task> func)
        {
            Logger.LogTrace($"{ClassName}.{methodName}");

            if(func == null) throw new ArgumentNullException(nameof(func));
            
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                throw LoggedException(methodName, ex);
            }
        }

        protected Task<TResult> HandleErrors<TResult>(string methodName, Func<Task<TResult>> func)
        {
            Logger.LogTrace($"{ClassName}.{methodName}");

            if(func == null) throw new ArgumentNullException(nameof(func));

            try
            {
                return func();
            }
            catch (Exception ex)
            {
                throw LoggedException(methodName, ex);
            }
        }

        private OidcException LoggedException(string methodName, Exception ex)
        {
            if (ex is OidcException oidcException && oidcException.Logged) return oidcException;
            return Logger.Exception(ex.Message);
        }
    }
}