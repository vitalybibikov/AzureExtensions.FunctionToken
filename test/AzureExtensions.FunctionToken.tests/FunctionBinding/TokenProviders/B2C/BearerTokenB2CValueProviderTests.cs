using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.FunctionBinding.Enums;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.B2C;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace AzureExtensions.FunctionToken.Tests
{
    public class BearerTokenB2CValueProviderTests
    {
        [Fact]
        public void GetValueAsyncWorksForScope()
        {
            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request
                .SetupGet(r => r.Headers)
                .Returns(new HeaderDictionary { { "Authorization", "Bearer abc123" } });
            request
                .SetupGet(r => r.HttpContext)
                .Returns(Mock.Of<HttpContext>());
            TokenAzureB2COptions options = new TokenAzureB2COptions {
                AzureB2CSingingKeyUri = new Uri("http://localhost:7001")
            };
            FunctionTokenAttribute attribute = new FunctionTokenAttribute(
                AuthLevel.Authorized,
                "z",
                new string[] {"user"}
            );
            SecurityToken mockSecurityToken = Mock.Of<SecurityToken>();
            Mock<IAzureB2CTokensLoader> mockLoader = new Mock<IAzureB2CTokensLoader>();
            mockLoader
                .Setup(l => l.Load(It.IsAny<Uri>()))
                .Returns(Task<List<JsonWebKey>>.Factory.StartNew( o => new List<JsonWebKey>(), null));
            mockLoader
                .Setup(l => l.Reload(It.IsAny<Uri>()))
                .Returns(Task<List<JsonWebKey>>.Factory.StartNew( o => new List<JsonWebKey>(), null));
            Mock<ISecurityTokenValidator> mockSecurityTokenValidator = new Mock<ISecurityTokenValidator>();
            mockSecurityTokenValidator
                .Setup(v => v.ValidateToken(
                        It.IsAny<string>(),
                        It.IsAny<TokenValidationParameters>(),
                        out mockSecurityToken
                    )
                )
                .Returns(
                    new ClaimsPrincipal(
                        new List<ClaimsIdentity> {
                            new ClaimsIdentity(
                                new List<Claim> 
                                {
                                    new Claim("scp", "Read"),
                                    new Claim(ClaimTypes.Role, "user")
                                }, 
                                "Bearer"
                            )
                    })
                );

            BearerTokenB2CValueProvider provider = new BearerTokenB2CValueProvider(
                request.Object,
                options,
                attribute,
                mockLoader.Object,
                mockSecurityTokenValidator.Object
            );
            
            ((FunctionTokenResult) (provider
                .GetValueAsync()
                .GetAwaiter()
                .GetResult()))
                .Status
                .Should()
                .Be(TokenStatus.Valid);
        }
    }
}
