# Sotsera.Blazor.Oidc

OpenID Connect client for Blazor client-side or hosted projects.

Server side projects, and probably hosted projects too, having a server side environment should relay
on cookies which are currently considered more secure: 
[Using OAuth and OIDC with Blazor](https://brockallen.com/2019/01/11/using-oauth-and-oidc-with-blazor/).

This is just a learning exercise built with three constraints:
- Minimal amount of javascript possible
- Use the Blazor shipped Json framework
- Integrate with Blazor authentication and authorization framework

These constraints basically mean that everything must be written from scratch (JWT validation included).

A demo project has been published [here](https://blazor-oidc.sotsera.com/).

## Installation

Add a reference to the library from [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Sotsera.Blazor.Oidc.svg)](https://www.nuget.org/packages/Sotsera.Blazor.Oidc/)

### Dependency injection configuration
The library with its configuration settings must be added to the `Startup.cs` class.

The minimal configuration for connecting through a popup window to an Authorization server using the default callback URIs requires:

- The issuer (Authorization server) URI
- The applcation base URI used to construct the default html callback URIs
- The client ID
- The response Type for **implicit flow** or **code+pkce**
- The scopes for the access token

As an example, the configuration for the code+pkce flow to the Identity server demo instance is:

```c#
public void ConfigureServices(IServiceCollection services)
{
    var issuerUri = new Uri("https://demo.identityserver.io");
    var baseUri = new Uri(WebAssemblyUriHelper.Instance.GetBaseUri());

    services.AddOidc(new OidcSettings(issuerUri, baseUri)
    {
        ClientId = "spa",
        ResponseType = "code",
        Scope = "openid profile email api"
    });
}
```

### Javascript inclusion
Add the following reference to the oidc javascript in the __index.html__ file paying attention to match the library version:

```c#
<script src="_content/Sotsera.Blazor.Oidc/sotsera.blazor.oidc-1.0.0-alpha-1.js"></script>
```

### Blazor Authorization 
Surround the default **router** in the **App.razor** component with a `CascadingAuthenticationState` component:

```html
<CascadingAuthenticationState>
    <Router AppAssembly="typeof(Program).Assembly">
        <NotFoundContent>
            <p>Sorry, there's nothing at this address.</p>
        </NotFoundContent>
        <NotAuthorizedContent>
            <h1>Sorry</h1>
            <p>You're not authorized to reach this page.</p>
            <p>You may need to log in as a different user.</p>
        </NotAuthorizedContent>
        <AuthorizingContent>
            <h1>Authentication in progress</h1>
            <p>Only visible while authentication is in progress.</p>
        </AuthorizingContent>
    </Router>
</CascadingAuthenticationState>
```

The example above specifies also the content that will be injected by the router in the `@body` part of a page that requires athorization through the `@attribute [Authorize]` when there is a pending authentication or the user is not authorized.

### Authorization server callbacks
The interaction with the authorization server can be made through a popup window or performing a redirect. 

The library already contains the **default** html pages used by the popup window callbacks and the callback page components for the redirect interaction type and their default URIs are automatically configured specifying the `baseUri` in the dependency injection. 

For `preview8` the router can't load page components from a razor class library so the following components must be added to the application in order to use the **redirect** interaction type:

```c#
@page "/oidc/callbacks/authentication-redirect"
@using Sotsera.Blazor.Oidc.CallbackPages

<AuthenticationRedirect />
```

```c#
@page "/oidc/callbacks/logout-redirect"
@using Sotsera.Blazor.Oidc.CallbackPages

<LogoutRedirect />
```

## Usage

The `IUserManager` interface provides the access the the `OidcUser` class and its profile claims and exposes the methods `BeginAuthenticationAsync` and `BeginLogoutAsync` for initiating a flow to the server using the interaction type specified during the initial configuration. Both methods configuration parameters can be overridden during the call.

This is an example Login/Logout component:

```c#
@inject Sotsera.Blazor.Oidc.IUserManager UserManager
<AuthorizeView>
    <Authorized>
        <span class="mr-3">
            Hello, @context.User.Identity.Name!
        </span>
        <button type="button" class="btn btn-primary mr-1" @onclick="LogoutPopup">
            Log out (popup)
        </button>
        <button type="button" class="btn btn-primary" @onclick="LogoutRedirect">
            Log out (redirect)
        </button>
    </Authorized>
    <NotAuthorized>
        <button type="button" class="btn btn-primary mr-1" @onclick="LoginPopup">
            Log in (popup)
        </button>
        <button type="button" class="btn btn-primary" @onclick="LoginRedirect">
            Log in (redirect)
        </button>
    </NotAuthorized>
</AuthorizeView>

@code
{
    public async void LoginPopup() => await UserManager.BeginAuthenticationAsync();
    public async void LoginRedirect() => await UserManager.BeginAuthenticationAsync(p => p.WithRedirect());
    
    public async void LogoutPopup() => await UserManager.BeginLogoutAsync();
    public async void LogoutRedirect() => await UserManager.BeginLogoutAsync(p => p.WithRedirect());
}
```

## Consuming an api

Inject in a component an instance of `OidcHttpClient` that has a **Bearer** authorization header already configured and use it for connecting to the APIs requiring the access token.

## Configuration

The list of the configuration options will be documented here soon.

## Credits

This library is based on [oidc-client-js](https://github.com/IdentityModel/oidc-client-js) by 
Brock Allen & Dominick Baier licensed under the 
[Apache License, Version 2.0](https://github.com/IdentityModel/oidc-client-js/blob/1.8.2/README.md).

## License

Sotsera.Blazor.Oidc is licensed under 
[Apache License, Version 2.0](https://github.com/sotsera/sotsera.blazor.oidc/blob/master/LICENSE.txt).
