// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

namespace Sotsera.Blazor.Oidc.Configuration.Model
{
    /// <summary>
    /// Specifies if the Authorization Server is engaged with a redirect or through a popup
    /// </summary>
    public enum InteractionType
    {
        Redirect,
        Popup
    }

    internal static class InteractionTypeExtensions {
        public static bool IsRedirect(this InteractionType type) => type == InteractionType.Redirect;
        public static bool IsPopup(this InteractionType type) => type == InteractionType.Popup;
    }
}