// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Text;

namespace Sotsera.Blazor.Oidc.Core.Protocol.Common.Model
{
    internal class OidcResponse
    {
        public string State { get; set; }
        public UrlParsingType UrlParsingType { get; set; }

        public string Error { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorUri { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();

        public bool HasErrors => Error.IsNotEmpty() || ErrorDescription.IsNotEmpty() || !ValidationErrors.IsEmpty();

        public string GetErrors()
        {
            if (!HasErrors) return string.Empty;

            var builder = new StringBuilder();
            if (Error.IsNotEmpty()) builder.Append($"Error: {Error};");
            if (ErrorDescription.IsNotEmpty()) builder.Append($"ErrorDescription: {ErrorDescription};");
            if (ValidationErrors.IsNotEmpty())
            {
                builder.Append($"ValidationErrors: {string.Join(" - ", ValidationErrors.ToArray())};");
            }
            return builder.ToString();
        }
    }
}