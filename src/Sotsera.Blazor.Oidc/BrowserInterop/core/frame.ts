// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

export interface ISessionFrameSettings {
    url: string;
    origin: string;
    timeout: number;
}

export class Frame {
    private readonly frame: HTMLIFrameElement;
    private readonly origin: string;
    private waitingForResponse: boolean = false;

    get contentWindow(): Window {
        return this.frame.contentWindow as Window;
    }

    constructor(settings: ISessionFrameSettings) {
        this.frame = window.document.createElement("iframe") as HTMLIFrameElement;
        this.frame.src = settings.url;
        this.frame.style.display = "none";
        this.origin = settings.origin;
    }

    load(): Promise<Frame> {
        return new Promise<Frame>((resolve) => {
            this.frame.onload = () => resolve(this);
            window.document.body.appendChild(this.frame);
        });
    }

    cleanUp(): void {
        window.document.body.removeChild(this.frame);
    }

    postToFrame(message: string): Promise<string> {
        if (this.waitingForResponse) return Promise.reject("Already waiting for a previous message response");
        this.waitingForResponse = true;

        return new Promise((resolve, reject) => {
            const messageHandler = (evt: MessageEvent): void => {
                window.removeEventListener("message", messageHandler);
                this.waitingForResponse = false;

                if (this.origin !== evt.origin) reject(`invalid origin: ${evt.origin}`);
                else if (this.contentWindow !== (evt.source as Window)) reject("invalid source window");
                else resolve(evt.data);
            }
                
            window.addEventListener("message", messageHandler);
            this.contentWindow.postMessage(message, this.origin);
        });
    }
}