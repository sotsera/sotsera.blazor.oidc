// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Text;
using Sotsera.Blazor.Oidc.Core.Common;

namespace Sotsera.Blazor.Oidc.Utilities
{
internal static class Base64Url
    {
        public static string Serialize(string value, string subject)
        {
            return SerializeBytes(Encoding.UTF8.GetBytes(value), subject);
        }

        public static string SerializeBytes(byte[] bytes, string subject)
        {
            try
            {
                if (bytes == null || bytes.Length == 0) return string.Empty;
                return Convert.ToBase64String(bytes)
                    .Replace("=", "")
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
            catch (Exception ex)
            {
                throw new OidcException($"Error serializing the {subject} to Base64Url from byte[]: {ex.Message}");
            }
        }

        public static string Deserialize(string value, string subject)
        {
            try
            {
                return value.IsEmpty() 
                    ? string.Empty 
                    : Encoding.UTF8.GetString(DeserializeBytes(value, subject));
            }
            catch (Exception ex)
            {
                throw new OidcException($"Error deserializing the Base64 {subject}: {ex.Message}");
            }
        }

        public static byte[] DeserializeBytes(string value, string subject)
        {
            try
            {
                if (value.IsEmpty()) return new byte[0];
                var base64 = value
                    .Replace('-', '+')
                    .Replace('_', '/')
                    .AddPadding();

                return Convert.FromBase64String(base64);
            }
            catch (Exception ex)
            {
                throw new OidcException($"Error deserializing the Base64Url {subject} to byte[]: {ex.Message}");
            }
        }

        public static T Deserialize<T>(string value, string subject)
        {
            try
            {
                if (value.IsEmpty()) return default;
                var json = Deserialize(value, subject);
                return Json.Deserialize<T>(json, subject);
            }
            catch (Exception ex)
            {
                throw new OidcException($"Error deserializing the Base64 {subject}: {ex.Message}");
            }
        }

        public static JsonData DeserializeData(string value, string subject)
        {
            try
            {
                if (value.IsEmpty()) return default;
                var json = Deserialize(value, subject);
                return Json.DeserializeData(json, subject);
            }
            catch (Exception ex)
            {
                throw new OidcException($"Error deserializing the Base64 {subject}: {ex.Message}");
            }
        }

        private static string AddPadding(this string value)
        {
            switch (value.Length % 4)
            {
                case 0: return value;
                case 2: return $"{value}==";
                case 3: return $"{value}=";
                default:
                    throw new OidcException("Invalid base64url string length");
            }
        }
    }
}