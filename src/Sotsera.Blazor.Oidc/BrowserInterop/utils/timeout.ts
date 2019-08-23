// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

export default function timeout<T>(milliseconds: number, promise: Promise<T>): Promise<T> {
    return new Promise<T>((resolve, reject) => {
        var timer = setTimeout(() => { reject(`Promise timed out after ${milliseconds}ms.`); }, milliseconds);

        promise
            .then(result => { clearTimeout(timer); resolve(result); })
            .catch(promiseError => { clearTimeout(timer); reject(promiseError); });
    });
};
