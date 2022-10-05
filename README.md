# Custom authorization or in-app authorization

This PoC contains a sample solution for handling custom authorization within a modern application.

## The problem

Many modern applications are using OAuth or OpenID Connect to authenticate and authorize their users. In some cases it is necessary to grant the differnet permissions to a user based on the resource that he is accessing in the application.

For example, there is a medical application that needs to have a different access for the users based on a medical case that they are accessing. A specific user should have a read only access for the archived case #123, read and write access for an active case #35 and no access to other cases:

![problem](docs/custom-authz-problem.png)

A token issued by identity provider can help us to authenticate a user. But it cannot help to authorize access to a medica case because identity provider can issue a token to the users only based on their role within the application but not based on the specific resource in the application that user is accessing.

In this example, in addition to the token received from an identity profider, the custom authorization shoud be implemented to help to authorize user's access to a specific resourse. 

During the implementation of the custom authorization the next requirements should be taken into account:

- **Centralization**  
  A solution should have a single place where authorization rules are configured. It will make it easier and faster to check, find and extend these rules.
- **Single responsibility**  
  A solution should use the existed framework authorization mechanisms (if possible) to avoid spreading the authorization logic everywhere in code (e.g. using conditions like ```if (<check-user-role>) then ...``` in every method). This will improve security becasue the request will not be able to even reach the code of the targeted endpoint if a user does not have access to the requested resource.
- **Separation of concerns**  
  Solution should not mix a business logic (to define the access permissions for a user) with a platform logic (to allow or prevent the access to a resource based on the user's permissions) to make the code easier to read and maintain.
- **Extensibility and simplicity**  
  Solution should be generic and provide a simple way to inject a business logic to provide or restrict an access for a user. It will reduce the costs by speeding up implementation and simplifying integration.
- **Security review**  
  Building an authorization pattern requires careful attention to security.  Any specific implementation should be reviewed with careful attention and planning. This example pattern illustrates the basic useable service pattern but does not include fundemental security details (logging, token handling, encryption standards, session managment, etc). Identifying and addressing solutions to these details will be required for specific implementations. 

## The solution

The recommended solution for this problem, taking into account the requirements above, is to add a service (named here as AuthorizationService) to the request execution pipeline. This service is responsible for enriching the request with the roles of the current user, which are based on the business logic of your application, and then passing this information on to further processing of the request where the platform or an application can use it to allow or deny access.

![solution](docs/custom-authz-solution.png)

The authorization service can obtain the necessary information to make a business decision about the roles for the current user from various sources, including a database, cache, internal or external service.

Below, you can find the recommended implementation of this solution for .NET applications.

## The achitecture

![solution-architecture](docs/custom-authz-architecture.png)

Let's look at the flow:

1. Unauthenticated users are redirected to Identity Provider (for example - Azure AD) where they logs-in
2. Identity Provider issues a token and return it to a user's browser
3. A user uses this token to access an application
4. The standard Authentication middleware validates the presented token and initializes User.Identity object that contains all the claims provided in the token (including user's groups as well)
5. The custom middleware `InjectRolesMiddleware`, injected after the Authentication middleware and before the Authorization middleware, calls the internal service `AuthorizationService` to get the user's roles
6. The service `AuthorizationService` implements the business logic to get roles for the current user for the current request. To get the roles for the user, it can use external storage (like SQL or NoSQL databases), internal storage (like files or memory), internal or external service (available via HTTP).
7. The service returns the user's roles which will be injected by the `InjectRolesMiddleware` into the User.Identity object as the role claims
8. The standard Authorization middleware will use (by default) these injected roles to make a desision to authorize an access and continue to run until the request is dispatched to the code at the requested endpoint or to deny access and stop the request immediately.

## The code

You can find multiple projects within [src](./src) folder. These projects correspond to different types of .NET applications.

### [Api.Minimal](./src/Api.Minimal)
This project shows implementation of this solution for an ASP.NET Web API application that is built using minimal API.

- Create a new [authorization policy](https://learn.microsoft.com/aspnet/core/security/authorization/policies) in [Program.cs](./src/Api.Minimal/Program.cs) file:
  ```csharp
    options.AddPolicy("RequireAccessToSecret", policy => policy.RequireRole("AccessToSecret"));
  ```

- The `DummyAuthorizationService` is registered in `IServiceCollection` in [Program.cs](./src/Api.Minimal/Program.cs) file:
  ```csharp
    builder.Services.AddScoped<IAuthorizationService, DummyAuthorizationService>();
  ```

- The `InjectRolesMiddleware` injected into the pipeline in [Program.cs](./src/Api.Minimal/Program.cs) file:
  ```csharp
    app.UseInjectedRoles();
  ```

- To [authorize an access](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis#authorization) in minimal API, require created above authorization policy for `/weatherforecast` method in [Program.cs](./src/Api.Minimal/Program.cs) file:
  ```csharp
    .RequireAuthorization(new[] { "RequireAccessToSecret" });
  ```

### [Api.Mvc](./src/Api.Mvc)
This project shows implementation of this solution for an ASP.NET Web API application that is built using MVC.

- Create a new [authorization policy](https://learn.microsoft.com/aspnet/core/security/authorization/policies) in [Program.cs](./src/Api.Mvc/Program.cs) file:
  ```csharp
    options.AddPolicy("RequireAccessToSecret", policy => policy.RequireRole("AccessToSecret"));
  ```

- The `DummyAuthorizationService` is registered in `IServiceCollection` in [Program.cs](./src/Api.Mvc/Program.cs) file:
  ```csharp
    builder.Services.AddScoped<IAuthorizationService, DummyAuthorizationService>();
  ```

- The `InjectRolesMiddleware` injected into the pipeline in [Program.cs](./src/Api.Mvc/Program.cs) file:
  ```csharp
    app.UseInjectedRoles();
  ```

- To [authorize an access](https://learn.microsoft.com/aspnet/web-api/overview/security/authentication-and-authorization-in-aspnet-web-api#authorization) in Web API, require created above authorization policy for `Get` method in [WeatherForecastController.cs](./src/Api.Mvc/Controllers/WeatherForecastController.cs) file:
  ```csharp
    [Authorize(Policy = "RequireAccessToSecret")]
  ```

### [Shared](./src/Shared)
This project is referenced by all the other projects and contains the implementation for the custom middleware [InjectRolesMiddleware](./src/Shared/Middlewares/InjectRolesMiddleware.cs). It also declares the interface [IAuthorizationService](./src/Shared/Services/IAuthorizationService.cs) and contains an implementation for this interface called [DummyAuthorizationService](/src/Shared/Services/DummyAuthorizationService.cs). This service is just a simple and naive examle of implementation of the `IAuthorizationService` interface. You should create your own custom implementation of this interface based on your business authorization rules and register this service as a scoped service in `IServiceCollection` in your `Program.cs` file:
```csharp
  builder.Services.AddScoped<Shared.Services.IAuthorizationService, YourCustomAuthorizationService>();
```

### [Web.Mvc](./src/Web.Mvc)
This project shows implementation of this solution for an ASP.NET Web application that is built using MVC.

- Create a new [authorization policy](https://learn.microsoft.com/aspnet/core/security/authorization/policies) in [Program.cs](./src/Web.Mvc/Program.cs) file:
  ```csharp
    options.AddPolicy("RequireAccessToSecret", policy => policy.RequireRole("AccessToSecret"));
  ```

- The `DummyAuthorizationService` is registered in `IServiceCollection` in [Program.cs](./src/Web.Mvc/Program.cs) file:
  ```csharp
    builder.Services.AddScoped<Shared.Services.IAuthorizationService, DummyAuthorizationService>();
  ```

- The `InjectRolesMiddleware` injected into the pipeline in [Program.cs](./src/Web.Mvc/Program.cs) file:
  ```csharp
    app.UseInjectedRoles();
  ```

- To [authorize an access](https://learn.microsoft.com/aspnet/core/security/authorization/policies#apply-policies-to-mvc-controllers) in MVC application, require created above authorization policy for `Secret` method in [HomeController.cs](/src/Web.Mvc/Controllers/HomeController.cs) file:
  ```csharp
    [Authorize(Policy = "RequireAccessToSecret")]
  ```


### [Web.Razor](./src/Web.Razor)
This project shows implementation of this solution for an ASP.NET Web application that is built using Razor Pages.

- Create a new [authorization policy](https://learn.microsoft.com/aspnet/core/security/authorization/policies) in [Program.cs](./src/Web.Razor/Program.cs) file:
  ```csharp
    options.AddPolicy("RequireAccessToSecret", policy => policy.RequireRole("AccessToSecret"));
  ```

- The `DummyAuthorizationService` is registered in `IServiceCollection` in [Program.cs](./src/Web.Razor/Program.cs) file:
  ```csharp
    builder.Services.AddScoped<Shared.Services.IAuthorizationService, DummyAuthorizationService>();
  ```

- The `InjectRolesMiddleware` injected into the pipeline in [Program.cs](./src/Web.Razor/Program.cs) file:
  ```csharp
    app.UseInjectedRoles();
  ```

- To [authorize an access](https://learn.microsoft.com/aspnet/core/security/authorization/razor-pages-authorization) in Razor pages, require created above authorization policy for `/Secret` page in [Program.cs](./src/Web.Razor/Program.cs) file:
  ```csharp
    options.Conventions.AuthorizePage("/Secret", "RequireAccessToSecret");
  ```
