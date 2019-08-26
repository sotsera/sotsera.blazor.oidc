// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Sotsera.Blazor.Oidc.Core;
using Sotsera.Blazor.Oidc.Core.Protocol.Common.Model;
using Sotsera.Blazor.Oidc.Core.Protocol.SessionManagement.Model;

namespace Sotsera.Blazor.Oidc.BrowserInterop
{
    public class Interop
    {
        private UserManager UserManager { get; set; }
        private IJSRuntime Runtime { get; }
        
        public Interop(IJSRuntime runtime)
        {
            Runtime = runtime;
        }

        internal Task Init(UserManager userManager)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            return Runtime.InvokeAsync<object>(Consts.Interop.Init, DotNetObjectRef.Create(this));
        }

        public Task<string> InitSessionFrame(FrameSettings settings)
        {
            return Runtime.InvokeAsync<string>(Consts.Interop.InitSessionFrame, settings);
        }

        public Task<string> PostToSessionFrame(string message)
        {
            return Runtime.InvokeAsync<string>(Consts.Interop.PostToSessionFrame, message);
        }

        public Task OpenPopup(OidcRequest request)
        {
            return Runtime.InvokeAsync<object>(Consts.Interop.OpenPopup, request);
        }

        public async Task<string> GetAsync<T>(string storageType, string key)
        {
            return await Runtime.InvokeAsync<string>($"{storageType}.getItem", key);
        }

        public Task SetAsync(string storageType, string key, string value)
        {
            return Runtime.InvokeAsync<object>($"{storageType}.setItem", key, value);
        }

        public Task RemoveAsync(string storageType, string key)
        {
            return Runtime.InvokeAsync<object>($"{storageType}.removeItem", key);
        }

        [JSInvokable]
        public Task CompleteAuthenticationAsync(string url) => UserManager.CompleteAuthenticationAsync(url);

        [JSInvokable]
        public Task CompleteLogoutAsync(string url) => UserManager.CompleteLogoutAsync(url);
    }
}