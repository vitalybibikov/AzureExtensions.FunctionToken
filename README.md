# AzureExtensions.FunctionToken
Extension Attribute to Azure Functions v3, that allows to obrain ClaimsPrincipal on every request. Currently supports key load from Azure B2C by jwks_uri and simple JsonWebKey. Pluggable into Azure Function Startup

The extension allows you to use custom tokens in Azure Functions v3.

Step 1.
1. Add the nuget *AzureExtensions.FunctionToken*
2. Add to Startup file the following code.  Currently, accepts simple JWK tokens or tokens loaded out of Azure B2C

```

           builder.AddAzureFunctionsToken(new TokenSinginingKeyOptions()
            {
                SigningKey = new JsonWebKey("your key"),
                Audience = "your audience",
                Issuer = "your issuer"
            });

```

OR B2C

```
            builder.AddAzureFunctionsToken(new TokenAzureB2COptions()
            {
                //AzureB2CSingingKeyUri = new Uri("https://yourapp.b2clogin.com/yourapp.onmicrosoft.com/discovery/v2.0/keys?p=yourapp-policy"),
                Audience = "your audience",
                Issuer = "your issuer"
            });

```

OR  Firebase
```
            builder.AddAzureFunctionsToken(new FireBaseOptions()
            {
                GoogleServiceAccountJsonUri = new Uri("%uri-to-storage-with-secret-json-from-google")
            });

```

3. Add it to Azure Function:

```
    public class Example
    {
        [FunctionName("Example")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [FunctionToken] FunctionTokenResult token,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return (ActionResult) new OkObjectResult($"Hello, {token}");
        }
    }
```

4. By, default AuthLevel.Authorized level is used, but you can also specify AuthLevel.AllowAnonymous


```
        [FunctionName("Example")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req,
            [FunctionToken(AuthLevel.AllowAnonymous)] FunctionTokenResult token,
            ILogger log)
        {
                log.LogInformation("C# HTTP trigger function processed a request.");
                return new OkObjectResult($"Hello, {token}");
        }}
```

5. Currently, AF 2.0 does not support invocation to Short Circuit, so in order to return proper 401 code when UnAuthorized,
   the function should be wrapped in Handler: Wrap/WrapAsync.
   This one will return 401 if token is invalid:
   
   
```
        [FunctionName("Example")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req,
            [FunctionToken(AuthLevel.Authorized)] FunctionTokenResult token,
            ILogger log)
        {
            return await Handler.WrapAsync(token,async () =>
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                return new OkObjectResult($"Hello, {token}");
            });
        }
```

6. Also, roles as a set of strings are supported:
   In order the role to be validated, role ClaimTypes.Role of System.Security should be presented in a token
   It is also mapped to type:  http://schemas.microsoft.com/ws/2008/06/identity/claims/role

        [FunctionName("Example")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req,
            [FunctionToken("Manager", "Worker")] FunctionTokenResult token,
            ClaimsPrincipal principal,
            ILogger log)
        {
            var identity = token.Principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
            return await Handler.WrapAsync(token,async () =>
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                return new OkObjectResult($"Hello, {token}");
            });
        }

ClaimsPrincipal can be injected, when 

           builder.Services.AddHttpContextAccessor();

attached via:

           var injectedPrincipal = req.HttpContext.User;


7.  That's it



