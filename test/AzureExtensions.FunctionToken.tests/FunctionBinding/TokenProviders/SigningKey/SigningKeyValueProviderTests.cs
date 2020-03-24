using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using AzureExtensions.FunctionToken.FunctionBinding.Enums;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.SigningKey;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace AzureExtensions.FunctionToken.Tests
{
    public class SigningKeyValueProviderTests
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
            TokenSinginingKeyOptions options = new TokenSinginingKeyOptions {
                SigningKey = JsonWebKey.Create(
                    @"{
                        ""alg"": ""RS256"",
                        ""kty"": ""RSA"",
                        ""use"": ""sig"",
                        ""n"": ""fasdklfjahsdfkaljsnddfnlkasudvgiuq3450897uzxcvnlksdfn---aserkfjasbvdkluy3t45r"",
                        ""e"": ""AQAB"",
                        ""kid"": ""NKJLGHLJKHGBVKBLKJAFUYKJHBFADF"",
                        ""x5t"": ""NKJLGHLJKHGBVKBLKJAFUYKJHBFADF"",
                        ""x5c"": [
                            ""hkjlgrhkljzvkzjhlgvzvdklhvzsilseujrgfoisyehfltw34to--=sdfghzksujfdhgi7tsertukjhask.fhcgfalsiyuegrht.jklw34batgfklyuasgdvkjsdrbg=""
                        ]

                    }"
                ),
                Issuer = "http://iamanissuerofauthtokens.com",
                Audience = "some audience",
            };
            FunctionTokenAttribute attribute = new FunctionTokenAttribute(
                AuthLevel.Authorized,
                requiredScope,
                authorizedRoles
            );
            SecurityToken mockSecurityToken = Mock.Of<SecurityToken>();
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

            SigningKeyValueProvider provider = new SigningKeyValueProvider(
                request.Object,
                options,
                attribute,
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
                    null,
                    new List<Claim>(),
                    TokenStatus.Valid
                };
                
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

                yield return new object[] {
                    "",
                    null,
                    new List<Claim>(),
                    TokenStatus.Valid
                };

                // Only  scope is required
                yield return new object[] {
                    "read",
                    null,
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
                    null,
                    new List<Claim> 
                    {
                        new Claim("scp", "Different Scope"),
                    },
                    TokenStatus.Error
                };
                
                yield return new object[] {
                    "read",
                    null,
                    new List<Claim> (),
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
                    null,
                    new List<Claim> 
                    {
                        new Claim("scp", "write read"),
                    },
                    TokenStatus.Valid
                };

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
