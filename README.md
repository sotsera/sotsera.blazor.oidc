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

The minimal configuration for connecting to an Authorization server using the default callback URIs requires:

- The issuer (Authorization server) URI
- The application base URI used to construct the default html callback URIs
- The client ID
- The response Type for **implicit flow** or **code+pkce**
- The scopes for the access token

The current application base URI is passed along to the settings object for configuring the default callback URIs but can also be used for setting other environment sensitive values.

As an example, the configuration for the code+pkce flow to the Identity server demo instance is:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddOidc(new Uri("https://demo.identityserver.io"), (settings, siteUri) =>
    {
        settings.UseDefaultCallbackUris(siteUri);
        settings.ClientId = "spa";
        settings.ResponseType = "code";
        settings.Scope = "openid profile email api";
    });
}
```

### Javascript inclusion
Add the following reference to the oidc javascript in the __index.html__ file paying attention to match the library version:

```c#
<script src="_content/Sotsera.Blazor.Oidc/sotsera.blazor.oidc-1.0.0-alpha-4.js"></script>
```

### Blazor Authorization 
The default **router** in the **App.razor** has to be updated:

```html
@using Sotsera.Blazor.Oidc
@inject IUserManager UserManager

<Router AppAssembly="@typeof(Program).Assembly" AdditionalAssemblies="new[] { typeof(IUserManager).Assembly }">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
            <NotAuthorized>
                <p>You're not authorized to reach this page.</p>
            </NotAuthorized>
            <Authorizing>
                <h3>Authentication in progress</h3>
            </Authorizing>
        </AuthorizeRouteView>
    </Found>
    <NotFound>
        <CascadingAuthenticationState>
            <LayoutView Layout="@typeof(MainLayout)">
                <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </CascadingAuthenticationState>
    </NotFound>
</Router>
```

### Authorization server callbacks
The interaction with the authorization server can be made through a popup window or performing a redirect. 

The library already contains the **default** html pages used by the popup window callbacks and the callback page components for the redirect interaction type and their default URIs are automatically configured by the `UseDefaultCallbackUris`. 

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
