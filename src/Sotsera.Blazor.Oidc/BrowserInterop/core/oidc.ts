// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

import { Frame, ISessionFrameSettings } from "./frame";
import { Popup, IOpenPopupRequest } from "./popup";
import timeout from "../utils/timeout";

export interface IDotNetReference {
    invokeMethod<T>(methodIdentifier: string, ...args: any[]): T;
    invokeMethodAsync<T>(methodIdentifier: string, ...args: any[]): Promise<T>;
}

export default class Oidc {
    private sessionFrame: Frame | undefined;
    private sessionFrameTimeout: number = 5000;
    private popup: Popup | undefined;
    private interop: IDotNetReference | undefined;

    init = (interop: IDotNetReference): Promise<void> => {
        this.interop = interop;
        return Promise.resolve();
    }

    async initSessionFrame(settings: ISessionFrameSettings): Promise<void> {
        if (this.sessionFrame !== undefined) return Promise.resolve();
        this.sessionFrame = await new Frame(settings).load();
        this.sessionFrameTimeout = settings.timeout;
        return Promise.resolve();
    }

    postToSessionFrame(message: string): Promise<string> {
        if (this.sessionFrame === undefined) return Promise.reject("Check session service not initialized");
        return timeout(this.sessionFrameTimeout, this.sessionFrame.postToFrame(message));
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