// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using Microsoft.Extensions.DependencyInjection;
using Sotsera.Blazor.Oidc.Configuration.Model;
using Sotsera.Blazor.Oidc.Core.Common;

namespace Sotsera.Blazor.Oidc.Configuration
{
    internal class OidcSettingsValidator
    {
        public void EnsureValidSettings(IServiceCollection services, OidcSettings settings)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var validationResult = Validate(settings);

            if (!validationResult.HasErrors) return;

            validationResult.PrintErrors();
            throw new OidcException("Oidc configuration aborted", true);
        }

        public OidcSettingsValidationResult Validate(OidcSettings settings)
        {
            var result = new OidcSettingsValidationResult();

            if (settings == null)
            {
                result.AddError("Missing Oidc settings");
                return result;
            }

            if (settings.Issuer == null) result.AddError("Missing issuer");
            if (settings.ClientId.IsEmpty()) result.AddError("Missing client id");
            if (settings.ResponseType.IsEmpty()) result.AddError("Missing response type");
            if (settings.Scope.IsEmpty()) result.AddError("Missing scope");

            if (settings.StorageType.IsMemory() && settings.InteractionType.IsRedirect())
            {
                result.AddError("Interaction type cannot be redirect using memory storage");
            }

            if (!settings.StorageType.IsMemory() && settings.StoragePrefix.IsEmpty())
            {
                result.AddError("Missing storage prefix");
            }

            return result;
        }
    }
}