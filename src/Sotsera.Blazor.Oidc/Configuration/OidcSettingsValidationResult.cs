// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Sotsera.Blazor.Oidc.Configuration
{
    internal class OidcSettingsValidationResult
    {
        public IList<string> Errors { get; } = new List<string>();

        public bool HasErrors => Errors.Count > 0;

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void PrintErrors()
        {
            const string methodName = nameof(IServiceCollectionExtensions.AddOidc);
            var errorMessage = $"Please fix the error(s) in the \"{methodName}\" method call";

            var line = new string('-', errorMessage.Length + 10);
            Console.WriteLine(line);
            Console.Error.WriteLine("Sotsera.Blazor.Oidc configuration errors:");
            foreach (var error in Errors) Console.Error.WriteLine($"- {error}");
            Console.Error.WriteLine(errorMessage);
            Console.WriteLine(line);
        }
    }
}