using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Shouldly;
using Sotsera.Blazor.Oidc.Utilities;
using Xunit;

namespace Sotsera.Blazor.Oidc.Tests
{
    public class ProtocolJwtSamples
    {
        // Example for RSASSA-PKCS-v1_5 SHA-256
        // https://tools.ietf.org/html/draft-ietf-jose-json-web-signature-41#appendix-A.2
        [Fact]
        public void CanValidateRS256()
        {
            const string token = "eyJhbGciOiJSUzI1NiJ9.eyJpc3MiOiJqb2UiLA0KICJleHAiOjEzMDA4MTkzODAsDQogImh0dHA6Ly9leGFtcGxlLmNvbS9pc19yb290Ijp0cnVlfQ.cC4hiUPoj9Eetdgtv3hF80EGrhuB__dzERat0XF9g2VtQgr9PJbu3XOiZj5RZmh7AAuHIm4Bh-0Qc_lF5YKt_O8W2Fp5jujGbds9uJdbF9CUAr7t1dnZcAcQjbKBYNX4BAynRFdiuB--f_nZLgrnbyTyWzO75vRK5h6xBArLIARNPvkSjtQBMHlb1L07Qe7K0GarZRmB_eSN9383LcOLn6_dO--xi12jzDwusC-eOkHWEsqtFZESc6BfI7noOPqvhJ1phCnvWh6IeYI2w9QOYEUipUTI8np6LbgGY9Fs98rqVt5AXLIhWkWywlVmtVrBp0igcN_IoypGlUPQGe77Rw";
            var n = Base64Url.DeserializeBytes("ofgWCuLjybRlzo0tZWJjNiuSfb4p4fAkd_wWJcyQoTbji9k0l8W26mPddxHmfHQp-Vaw-4qPCJrcS2mJPMEzP1Pt0Bm4d4QlL-yRT-SFd2lZS-pCgNMsD1W_YpRPEwOWvG6b32690r2jZ47soMZo9wGzjb_7OMg0LOL-bSf63kpaSHSXndS5z5rexMdbBYUsLA9e-KXBdQOS-UTo7WTBEMa2R2CapHg665xsmtdVMTBQY4uDZlxvb3qCo5ZwKh9kG4LT6_I5IhlJH7aGhyxXFvUK-DWNmoudF8NAco9_h9iaGNj8q2ethFkMLs91kzk2PAcDTW9gb54h4FRWyuXpoQ", "n");
            var e = Base64Url.DeserializeBytes("AQAB", "e");

            var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n });

            IdentityModelEventSource.ShowPII = true;

            var result = new JsonWebTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromDays(4000),
                ValidIssuer = "joe",
                ValidateAudience = false
            });

            result.IsValid.ShouldBeTrue();
        }

        // Example for ECDSA P-256 SHA-256
        // https://tools.ietf.org/html/draft-ietf-jose-json-web-signature-41#appendix-A.3
        [Fact]
        public void CanValidateES256()
        {
            const string token = "eyJhbGciOiJFUzI1NiJ9.eyJpc3MiOiJqb2UiLA0KICJleHAiOjEzMDA4MTkzODAsDQogImh0dHA6Ly9leGFtcGxlLmNvbS9pc19yb290Ijp0cnVlfQ.DtEhU3ljbEg8L38VWAfUAqOyKAM6-Xx-F4GawxaepmXFCgfTjDxw5djxLa8ISlSApmWQxfKTUJqPP3-Kg6NU1Q";
            var x = Base64Url.DeserializeBytes("f83OJ3D2xF1Bg8vub9tLe1gHMzV76e8Tus9uPHvRVEU", "ECDSA key X value");
            var y = Base64Url.DeserializeBytes("x_FEzRu9m36HLN_tue659LNpXW6pCyStikYjKIWI5a0", "ECDSA key Y value");

            var key = new ECDsaSecurityKey(ECDsa.Create(new ECParameters
            {
                Q = new ECPoint
                {
                    X = x,
                    Y = y
                },
                Curve = ECCurve.NamedCurves.nistP384
            }));

            IdentityModelEventSource.ShowPII = true;

            var result = new JsonWebTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromDays(4000),
                ValidIssuer = "joe",
                ValidateAudience = false
            });

            result.IsValid.ShouldBeTrue();
        }
    }
}
