# Custom authorization or in-app authorization

This PoC contains a sample solution for handling custom authorization within a modern application.

## The problem

Most of the modern applications are using OAuth or OpenID Connect to authenticate and authorize their users. However, in some cases it is necessary to grant  the differnet permissions to a user based on the resource that he is accessing in the application.

![problem](docs/custom-authz-problem.png)

A token issued by identity provider can help us to authenticate a user. But it can't help to authorize access because identity provider can issue a token to a user only based on his role within the application but not based on the specific resource in the application that user is accessing.

In this case, in addition to the token, the custom authorization can help to authorize access. However, during the implementation the next requirements should be taken into account:

- There should be the only one place where authorization rules are configured
- The framework authorization mechanisms should be used to avoid inserting everywhere conditions like ```if (<check-user-role>) then ...```
- Solution should be generic and provide a simple way to inject a business logic to provide or restrict an access for a user

## The solution

There are multiple solution to solve this problem. But taking into account the requirements above, we recommend to add a service (named here as AuthorizationService) to the request execution pipeline. This service is responsible for enriching the request with the roles of the current user, based on the business logic of your application, and then passing this information on to further processing of the request.

![problem](docs/custom-authz-solution.png)

You can find the recommended implementation for the .NET applications below.

## The achitecture

![solution](docs/custom-authz-architecture.png)

Let's look at the flow:

1. Unauthenticated user is redirected to Identity Provider (for example - Azure AD) where he logs-in
2. Identity Provider issues a token and return it to a user's browser
3. the browser uses this token to access an application
4. Authentication middleware validates the presented token and initializes User.Identity object that contains all the inforamtion about a user including groups
5. 

## The code


