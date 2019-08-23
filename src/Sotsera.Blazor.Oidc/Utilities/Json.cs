// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Text.Json;
using Sotsera.Blazor.Oidc.Core.Common;

namespace Sotsera.Blazor.Oidc.Utilities
{
    internal static class Json
    {
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static readonly JsonDocumentOptions DocumentOptions = new JsonDocumentOptions()
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        };

        public static string Serialize(object value, string subject)
        {
            try
            {
                return JsonSerializer.Serialize(value, DefaultOptions);
            }
            catch (Exception ex)
            {
                var message = $"Error serializing the {subject ?? "input value"} to json: {ex.Message}";
                throw new OidcException(message);
            }
        }

        public static T Deserialize<T>(string json, string subject)
        {
            try
            {
                return json.IsEmpty() 
                    ? default 
                    : JsonSerializer.Deserialize<T>(json, DefaultOptions);
            }
            catch (Exception ex)
            {
                throw DeserializationError<T>(subject, ex);
            }
        }

        //TODO: see https://github.com/dotnet/corefx/issues/37564
        //The class above throws for dictionaries of primitive types
        public static T Deserialize<T>(JsonElement element, string subject)
        {
            if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
            {
                return default;
            }

            var type = typeof(T);

            try
            {
                if (type == typeof(string)) return (T)(object)element.GetString();
                if (type == typeof(bool)) return (T)(object)element.GetBoolean();
                if (type == typeof(int)) return (T)(object)element.GetInt32();
                if (type == typeof(long)) return (T)(object)element.GetInt64();
                if (type == typeof(decimal)) return (T)(object)element.GetDecimal();
                if (type == typeof(double)) return (T)(object)element.GetDouble();
                if (type == typeof(float)) return (T)(object)element.GetSingle();
                if (type == typeof(DateTime)) return (T)(object)element.GetDateTime();

                return JsonSerializer.Deserialize<T>(element.GetRawText(), DefaultOptions);
            }
            catch (Exception ex)
            {
                throw DeserializationError<T>(subject, ex);
            }
        }

        public static JsonData DeserializeData(string json, string subject)
        {
            try
            {
                if (json.IsEmpty()) return default;
                return JsonSerializer.Deserialize<JsonData>(json, DefaultOptions);
            }
            catch (Exception ex)
            {
                throw DeserializationError<JsonData>(subject, ex);
            }
        }

        private static OidcException DeserializationError<T>(string subject, Exception ex)
        {
            var message = $"Error deserializing the {subject} json to {typeof(T).Name} : {ex.Message}";
            return new OidcException(message);
        }
    }
}