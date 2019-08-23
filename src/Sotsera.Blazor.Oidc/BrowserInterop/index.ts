// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0

import Oidc from "./core/oidc";

const sotsera = (window["sotsera"] || {}) as any;
sotsera.blazor = sotsera.blazor || {};
sotsera.blazor.oidc = new Oidc();

window["sotsera"] = sotsera;