// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

export interface IOpenPopupRequest {
    url: string;
    timeout: number;
    target?: string;
    features?: string;
    replace?: boolean;
}

export class Popup {
    private popup: Window | null = null;

    get url(): any | null {
        return this.popup === null ? null : this.popup.location.href;
    }

    open(request: IOpenPopupRequest): Promise<void> {
        this.close();

        const req = request;
        this.popup = window.open(req.url, req.target, req.features, req.replace);

        if (this.popup) {
            this.popup.window.focus();
            return Promise.resolve();
        }

        return Promise.reject("Unable to open the popup");
    }

    close(): void {
        if (this.popup !== null) this.popup.close();
        this.popup = null;
    }
}