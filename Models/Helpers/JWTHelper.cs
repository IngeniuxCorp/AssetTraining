using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace v10CustomTabQuickStart.Models.Helpers
{
	public class JWTHelper
	{
		private static string SECURITY_KEY = ConfigurationManager.AppSettings["ida:SecurityKey"];
		public static string GetJwtToken(string audience, string issuer, StateData details, DateTime timeStamp)
		{

			string plainTextSecurityKey = SECURITY_KEY;
			var signingKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(plainTextSecurityKey));
			var signingCredentials = new SigningCredentials(signingKey,
				SecurityAlgorithms.HmacSha256Signature);
			var claimsIdentity = new ClaimsIdentity(new List<Claim>()
				{
					new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(details), ClaimValueTypes.String),
				}, "Custom");
			var securityTokenDescriptor = new SecurityTokenDescriptor()
			{
				Audience = audience,
				Issuer = issuer,
				Subject = claimsIdentity,
				Expires = timeStamp.AddDays(200),
				SigningCredentials = signingCredentials,
				IssuedAt = timeStamp
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
			var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);
			return signedAndEncodedToken;
		}

		public static StateData DecodeJwtToken(string token, IEnumerable<string> audiences, string issuer)
		{
			var plainTextSecurityKey = SECURITY_KEY;
			var signingKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(plainTextSecurityKey));
			var signingCredentials = new SigningCredentials(signingKey,
				SecurityAlgorithms.HmacSha256Signature);

			var tokenValidationParameters = new TokenValidationParameters()
			{
				ValidAudiences = audiences,
				ValidIssuers = new string[]
					{
						issuer
					},
				IssuerSigningKey = signingKey
			};

			var tokenHandler = new JwtSecurityTokenHandler();

			SecurityToken validatedToken;

			var claims = tokenHandler.ValidateToken(token,
				tokenValidationParameters, out validatedToken);

			string userData = claims.FindFirst(c => c.Type == ClaimTypes.UserData).Value;

			var data = JsonConvert.DeserializeObject<StateData>(userData);

			return data;
		}
	}
}