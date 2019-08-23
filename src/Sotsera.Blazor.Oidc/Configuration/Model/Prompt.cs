// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;

namespace Sotsera.Blazor.Oidc.Configuration.Model
{
   /// <summary>
    /// OPTIONAL. Space delimited, case sensitive list of ASCII string values that specifies whether the Authorization Server prompts the End-OidcUser for reauthentication and consent.
    /// Multiple values can be added using the OR operator.
    /// </summary>
    [Flags]
    public enum Prompt
    {
        /// <summary>
        /// The Authorization Server MUST NOT display any authentication or consent user interface pages. An error is returned if an End-OidcUser is not already authenticated or the Client does not have pre-configured consent for the requested Claims or does not fulfill other conditions for processing the request. The error code will typically be login_required, interaction_required, or another code defined in Section 3.1.2.6. This can be used as a method to check for existing authentication and/or consent.
        /// </summary>
        None = 1,

        /// <summary>
        /// The Authorization Server SHOULD prompt the End-OidcUser for reauthentication. If it cannot reauthenticate the End-OidcUser, it MUST return an error, typically login_required.
        /// </summary>
        Login = 2,

        /// <summary>
        /// The Authorization Server SHOULD prompt the End-OidcUser for consent before returning information to the Client. If it cannot obtain consent, it MUST return an error, typically consent_required.
        /// </summary>
        Consent = 4,

        /// <summary>
        /// The Authorization Server SHOULD prompt the End-OidcUser to select a user account. This enables an End-OidcUser who has multiple accounts at the Authorization Server to select amongst the multiple accounts that they might have current sessions for. If it cannot obtain an account selection choice made by the End-OidcUser, it MUST return an error, typically account_selection_required.
        /// </summary>
        SelectAccount = 8
    }

    internal static class PromptExtensions
    {
        /// <summary>
        ///  If this parameter contains none with any other value, then it is not valid.
        /// </summary>
        /// <param name="prompt">The <see cref="Prompt"/> value to be validated</param>
        /// <returns></returns>
        public static bool IsValid(this Prompt? prompt)
        {
            if (!prompt.HasValue) return true;

            if ((prompt & Prompt.None) != Prompt.None) return true;
            if ((prompt & Prompt.Consent) == Prompt.Consent) return false;
            if ((prompt & Prompt.Login) == Prompt.Login) return false;
            if ((prompt & Prompt.SelectAccount) == Prompt.SelectAccount) return false;

            return true;
        }

        /// <summary>
        /// Space delimited, case sensitive list of ASCII string values that specifies whether the Authorization Server prompts the End-OidcUser for reauthentication and consent
        /// </summary>
        /// <param name="prompt">The <see cref="Prompt"/> value to be formatted</param>
        /// <returns>Returns the space delimited, case sensitive list of ASCII string values contained by the <param name="prompt"></param></returns>
        public static string Format(this Prompt? prompt)
        {
            if (!prompt.HasValue) return string.Empty;
            if ((prompt & Prompt.None) == Prompt.None) return "none";

            var result = "";

            if ((prompt & Prompt.Consent) == Prompt.Consent) result += "consent ";
            if ((prompt & Prompt.Login) == Prompt.Login) result += "login ";
            if ((prompt & Prompt.SelectAccount) == Prompt.SelectAccount) result += "select_account ";

            return result.TrimEnd();
        }
    }
}