// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

import { Popup, IOpenPopupRequest } from "./popup";
import timeout from "../utils/timeout";

export interface IDotNetReference {
    invokeMethod<T>(methodIdentifier: string, ...args: any[]): T;
    invokeMethodAsync<T>(methodIdentifier: string, ...args: any[]): Promise<T>;
}

export default class Oidc {
    private popup: Popup | undefined;
    private interop: IDotNetReference | undefined;

    init = (interop: IDotNetReference): Promise<void> => {
        this.interop = interop;
        return Promise.resolve();
    }

    openPopup = (request: IOpenPopupRequest): Promise<void> => {
        if (this.popup === undefined) this.popup = new Popup();
        return timeout(request.timeout, this.popup.open(request));
    }

    authenticationCallback = async (): Promise<void> => {
        return this.popupCallback("CompleteAuthenticationAsync");
    }

    logoutCallback = (): Promise<void> => {
        return this.popupCallback("CompleteLogoutAsync");
    }

    silentRenewCallback = (): Promise<void> => {
        return new Promise<void>((resolve, reject) => {
            reject("Not implemented yet");
        });
    }

    private popupCallback = async (callbackName: string): Promise<void> => {
        const popup = this.popup;

        if (this.interop === undefined) return Promise.reject("Oidc Interop service not initialized");
        if (popup === undefined || popup === null) return Promise.reject("Invalid popup handler");

        return await this.interop
            .invokeMethodAsync<void>(callbackName, popup.url)
            .then(() => popup.close());
    }
}