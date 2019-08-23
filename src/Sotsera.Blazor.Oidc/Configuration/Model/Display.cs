// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;

namespace Sotsera.Blazor.Oidc.Configuration.Model
{
    /// <summary>
    /// Specifies how the Authorization Server displays the authentication and consent user interface pages to the End-OidcUser.
    /// </summary>
    public enum Display
    {
        /// <summary>
        /// The Authorization Server SHOULD display the authentication and consent UI consistent with a full OidcUser Agent page view. If the display parameter is not specified, this is the default display mode.
        /// </summary>
        Page,

        /// <summary>
        /// The Authorization Server SHOULD display the authentication and consent UI consistent with a popup OidcUser Agent window. The popup OidcUser Agent window should be of an appropriate size for a login-focused dialog and should not obscure the entire window that it is popping up over.
        /// </summary>
        Popup,

        /// <summary>
        /// The Authorization Server SHOULD display the authentication and consent UI consistent with a device that leverages a touch interface.
        /// </summary>
        Touch,

        /// <summary>
        /// The Authorization Server SHOULD display the authentication and consent UI consistent with a "feature phone" type display.
        /// </summary>
        Wap
    }

    internal static class DisplayExtensions
    {
        public static string Format(this Display? display)
        {
            return display == null ? null : Enum.GetName(typeof(Display), display);
        }
    }
}