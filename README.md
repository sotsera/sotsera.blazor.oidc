# Sotsera.Blazor.Oidc

OpenID Connect client for Blazor client-side projects.

Server side projects, and probably hosted projects too, having a server side environment should relay
on cookies which are currently considered more secure: 
[Using OAuth and OIDC with Blazor](https://brockallen.com/2019/01/11/using-oauth-and-oidc-with-blazor/).

This is just a learning exercise built with two constraints:
- Minimal amount of javascript possible
- Use the Blazor shipped Json framework

These constraints basically mean that everything must be written from scratch (JWT validation included).


- add the reference Install-Package Sotsera.Blazor.Oidc -Pre

## Credits

This library is based on [oidc-client-js](https://github.com/IdentityModel/oidc-client-js) by 
Brock Allen & Dominick Baier licensed under the 
[Apache License, Version 2.0](https://github.com/IdentityModel/oidc-client-js/blob/1.8.2/README.md)

## License

Sotsera.Blazor.Oidc is licensed under 
[Apache License, Version 2.0](https://github.com/sotsera/sotsera.blazor.oidc/blob/master/LICENSE.txt).
