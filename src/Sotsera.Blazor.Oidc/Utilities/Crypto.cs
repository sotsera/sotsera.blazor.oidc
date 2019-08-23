// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Security.Cryptography;
using System.Text;

namespace Sotsera.Blazor.Oidc.Utilities
{
    internal class Crypto
    {
        public string CreateUniqueHexadecimal(int length)
        {
            var bytes = CreateUniqueBytes(length / 2);
            var hexBuilder = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes) hexBuilder.AppendFormat("{0:x2}", b);
            return hexBuilder.ToString();
        }

        public byte[] CreateUniqueBytes(int length)
        {
            using var generator = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            generator.GetBytes(bytes);
            return bytes;
        }

        public byte[] ToSha256(byte[] input)
        {
            if (input == null) return new byte[0];

            using var sha = SHA256.Create();
            return sha.ComputeHash(input);
        }

        public string ToSha256(string input) => ToSha256(input, Encoding.UTF8);

        public string ToSha256(string input, Encoding encoding)
        {
            if (input.IsEmpty()) return string.Empty;

            var bytes = encoding.GetBytes(input);
            var hash = ToSha256(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}