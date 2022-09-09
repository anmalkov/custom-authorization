# Custom authorization or in-app authorization

This PoC contains a sample solution for handling custom authorization within a modern application.

## The problem

Most of the modern applications are using OAuth or OpenID Connect to authenticate and authorize their users. However, in some cases it is necessary to grant  the differnet permissions to a user based on the resource that he is accessing in the application.

![problem](docs/custom-authz-problem.png)

A token issued by identity provider can't help in this scenario because identity provider can issue a token to a user only based on his role within an application but not based on the resource in application that user is accessing.

In this case the custom authorization can help. However, during the implementation the next requirements should be taken into account:

- There should be the only one place where authorization rules are configured
- The framework authorization mechanisms should be used to avoid inserting everywhere conditions like ```if (<check-user-role>) then ...```
- Solution should be generic and provide a simple way to inject a business logic to provide or restrict an access for a user

## The solution
