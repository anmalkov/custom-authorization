# Custom authorization or in-app authorization

This PoC contains a sample solution for handling custom authorization within a modern application.

### The problem

Most of the modern applications are using OAuth or OpenID Connect to authenticate and authorize their users. However, in some cases it is necessary to grant  the differnet permissions to a user based on the resource that he is accessing in the application.

![problem](docs/custom-authz-problem.png)

In this case a token issued by identity provider can't help because identity provider can issue a token to a user only based on his role within an application but not based on the resource in application that user is accessing. 
