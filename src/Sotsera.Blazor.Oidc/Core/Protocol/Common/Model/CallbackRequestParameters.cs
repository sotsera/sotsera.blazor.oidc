// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using Sotsera.Blazor.Oidc.Configuration.Model;

namespace Sotsera.Blazor.Oidc.Core.Protocol.Common.Model
{
    public abstract class CallbackRequestParameters
    {
        /// <summary>
        /// Specifies how the Authorization Server displays the authentication and consent user interface pages to the End-OidcUser.
        /// </summary>
        public Display? Display { get; set; }
        /// <summary>
        /// Specifies if the Authorization Server is engaged with a redirect or through a popup
        /// </summary>
        public InteractionType InteractionType { get; internal set; }
        public string PopupWindowName { get; set; }
        public string PopupWindowFeatures { get; set; }

        internal string RedirectCallbackUri { get; set; }
        internal string PopupCallbackUri { get; set; }
        internal string RedirectUri => InteractionType.IsRedirect() ? RedirectCallbackUri: PopupCallbackUri;
        
        public void WithRedirect(string redirectCallbackUrl = null)
        {
            InteractionType = InteractionType.Redirect;
            if (redirectCallbackUrl.IsNotEmpty()) RedirectCallbackUri = redirectCallbackUrl;
        }

        public void WithPopup(string popupCallbackUrl = null)
        {
            InteractionType = InteractionType.Popup;
            Display = Configuration.Model.Display.Popup;
            if (popupCallbackUrl.IsNotEmpty()) PopupCallbackUri = popupCallbackUrl;
        }
    }
}