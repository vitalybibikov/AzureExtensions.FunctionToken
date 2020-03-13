using System;
using System.Collections;
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
        [Theory]
        [ClassData(typeof(UnitsTestData))]
        public void GetValueAsyncWorksForScope(string requiredScope, string[] authorizedRoles, List<Claim> claims, TokenStatus tokenStatus)
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
                requiredScope,
                authorizedRoles
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
                                claims, 
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
                .Be(tokenStatus);
        }

        public class UnitsTestData: IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // No Scopes or Roles Required on Function
                yield return new object[] {
                    null,
                    new string[] {},
                    new List<Claim>(),
                    TokenStatus.Valid
                };

                yield return new object[] {
                    "",
                    new string[] {},
                    new List<Claim>(),
                    TokenStatus.Valid
                };

                // Only  scope is required
                yield return new object[] {
                    "read",
                    new string[] {},
                    new List<Claim> 
                    {
                        new Claim("scp", "Read"),
                    },
                    TokenStatus.Valid
                };

                yield return new object[] {
                    "read",
                    new string[] {},
                    new List<Claim> 
                    {
                        new Claim("scp", "Different Scope"),
                    },
                    TokenStatus.Error
                };
                
                yield return new object[] {
                    "read",
                    new string[] {},
                    new List<Claim> (),
                    TokenStatus.Error
                };

                // Handle multiple scopes in returned claim
                yield return new object[] {
                    "read",
                    new string[] {},
                    new List<Claim> 
                    {
                        new Claim("scp", "write read"),
                    },
                    TokenStatus.Valid
                };
                
                // Scope and Role Required by function
                yield return new object[] {
                    "read",
                    new string[] {"user"},
                    new List<Claim> 
                    {
                        new Claim("scp", "read"),
                    },
                    TokenStatus.Error
                };

                yield return new object[] {
                    "read",
                    new string[] {"user"},
                    new List<Claim> 
                    {
                        new Claim("scp", "write read"),
                    },
                    TokenStatus.Error
                };
                
                yield return new object[] {
                    "read",
                    new string[] {"user"},
                    new List<Claim> 
                    {
                        new Claim("scp", "read"),
                        new Claim(ClaimTypes.Role, "nonuser")
                    },
                    TokenStatus.Error
                };
                
                yield return new object[] {
                    "read",
                    new string[] {"user"},
                    new List<Claim> 
                    {
                        new Claim("scp", "read"),
                        new Claim(ClaimTypes.Role, "user")
                    },
                    TokenStatus.Valid
                };

                yield return new object[] {
                    "read",
                    new string[] {"user"},
                    new List<Claim> 
                    {
                        new Claim("scp", "write read"),
                        new Claim(ClaimTypes.Role, "user")
                    },
                    TokenStatus.Valid
                };
                
                yield return new object[] {
                    "read",
                    new string[] {"admin", "user"},
                    new List<Claim> 
                    {
                        new Claim("scp", "read"),
                        new Claim(ClaimTypes.Role, "user")
                    },
                    TokenStatus.Valid
                };
            }
            IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) GetEnumerator();
        }
    }
}
