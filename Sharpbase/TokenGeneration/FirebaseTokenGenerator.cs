using System;
using System.Collections.Generic;

namespace Sharpbase.TokenGeneration
{
    public class FirebaseTokenGenerator
    {
        private const int TokenVersion = 0;

        private readonly string secret;

        public FirebaseTokenGenerator(string secret)
        {
            this.secret = secret;
        }

        /// <summary>
        /// Creates an authentication token containing arbitrary auth data.
        /// </summary>
        /// <param name="data">Arbitrary data that will be passed to the Firebase Rules API, once a client authenticates.  Must be able to be serialized to JSON with <see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>.</param>
        /// <returns>The auth token.</returns>
        public string CreateToken(IDictionary<string, object> data)
        {
            return CreateToken(data, new TokenOptions());
        }

        /// <summary>
        /// Creates an authentication token containing arbitrary auth data and the specified options.
        /// </summary>
        /// <param name="data">Arbitrary data that will be passed to the Firebase Rules API, once a client authenticates.  Must be able to be serialized to JSON with <see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>.</param>
        /// <param name="options">A set of custom options for the token.</param>
        /// <returns>The auth token.</returns>
        public string CreateToken(IDictionary<string, object> data, TokenOptions options)
        {
            bool dataEmpty = (data == null || data.Count == 0);
            if (dataEmpty && (options == TokenOptions.Empty || (!options.Admin && !options.Debug)))
                throw new Exception(
                    "data is empty and no options are set.  This token will have no effect on Firebase.");

            var claims = new Dictionary<string, object>
            {
                ["v"] = TokenVersion,
                ["iat"] = SecondsSinceEpoch(DateTime.Now)
            };

            bool isAdminToken = (options != TokenOptions.Empty && options.Admin);
            ValidateToken(data, isAdminToken);

            if (!dataEmpty)
                claims["d"] = data;

            // Handle options.
            if (options != TokenOptions.Empty)
            {
                if (options.Expires.HasValue)
                    claims["exp"] = SecondsSinceEpoch(options.Expires.Value);
                if (options.NotBefore.HasValue)
                    claims["nbf"] = SecondsSinceEpoch(options.NotBefore.Value);
                if (options.Admin)
                    claims["admin"] = true;
                if (options.Debug)
                    claims["debug"] = true;
            }

            string token = ComputeToken(claims);
            if (token.Length > 1024)
                throw new Exception("Generated token is too long. The token cannot be longer than 1024 bytes.");

            return token;
        }

        private string ComputeToken(IDictionary<string, object> claims)
        {
            return JWT.JsonWebToken.Encode(claims, secret, JWT.JwtHashAlgorithm.HS256);
        }

        private static long SecondsSinceEpoch(DateTime dt)
        {
            TimeSpan t = dt.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (long)t.TotalSeconds;
        }

        private static void ValidateToken(IDictionary<string, object> data, Boolean isAdminToken)
        {
            bool containsUid = (data != null && data.ContainsKey("uid"));
            if ((!containsUid && !isAdminToken) || (containsUid && !(data["uid"] is string)))
                throw new Exception("Data payload must contain a \"uid\" key that must not be a string.");

            if (containsUid && data["uid"].ToString().Length > 256)
                throw new Exception(
                    "Data payload must contain a \"uid\" key that must not be longer than 256 characters.");
        }
    }
}
