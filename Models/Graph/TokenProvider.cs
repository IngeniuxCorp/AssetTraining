using Ingeniux.CMS.Models;
using Microsoft.Identity.Client;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Host.SystemWeb;
using IdentityModel.OidcClient;
using Microsoft.Owin.Security;
using System.Net.Http;
using Newtonsoft.Json;
using IdentityModel;

namespace v10CustomTabQuickStart.Models.Graph
{
	public class TokenProvider
	{
		private string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
		private string appId = ConfigurationManager.AppSettings["ida:AppId"];
		private string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];
		private string scopes = ConfigurationManager.AppSettings["ida:GraphScopes"];
		private HttpContextBase _Context;
		private const string MS_AUTHENDPOINT = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
		private const string MS_TOKENENDPOINT = "https://login.microsoftonline.com/common/oauth2/v2.0/token";

		public CMS_Common Common { get; private set; }

		public TokenProvider(CMS_Common common, HttpContextBase context)
		{
			Common = common;
			_Context = context;
		}

		public bool SaveUserAccessCode()
		{
			string userId = Common.CurrentUser.UserId;
			SessionTokenCache sessionCache = new SessionTokenCache(userId, _Context);
			string stateString = sessionCache.ReadUserStateValue();

			var request = new AuthorizeRequest(MS_AUTHENDPOINT);
			var url = request.CreateAuthorizeUrl(
					clientId: appId,
					responseType: OidcConstants.ResponseTypes.Code,
					responseMode: OidcConstants.ResponseModes.FormPost,
					scope: scopes,
					redirectUri: redirectUri,
					state: $"{stateString}{userId}");

			var response = new AuthorizeResponse(url);

			string code = response.Code;
			string responseState = response.State;


			if (!string.IsNullOrWhiteSpace(responseState) && responseState == $"{stateString}{userId}")
			{
				sessionCache.SaveUserCodeValue(code);
			}else if(!string.IsNullOrWhiteSpace(responseState))
			{
				throw new Exception("Invalid state response");
			}
			return true;
		}

		public async Task<string> GetUserAccessTokenAsync()
		{
			string userId = Common.CurrentUser.UserId;
			SessionTokenCache sessionCache = new SessionTokenCache(userId, _Context);
			string token = sessionCache.ReadUserTokenValue();
			if (!string.IsNullOrWhiteSpace(token))
			{
				return token;
			}
			try
			{
				Dictionary<string, string> values;
				

				//using (var client = new HttpClient())
				//{
					var tclient = new TokenClient(
						MS_TOKENENDPOINT,
						appId,
						appSecret);

					string refreshToken = sessionCache.ReadUserRefreshValue();
					string code = sessionCache.ReadUserCodeValue();
					TokenResponse tokenResp;
					if (!string.IsNullOrWhiteSpace(refreshToken))
					{
						//values = new Dictionary<string, string>
						//{
						//	{ "client_id", appId},
						//	{ "refresh_token", refreshToken},
						//	{ "scope", scopes},
						//	{ "redirect_uri", redirectUri},
						//	{ "grant_type", "refresh_token"},
						//	{ "client_secret", appSecret}
						//};
						tokenResp = await tclient.RequestRefreshTokenAsync(refreshToken, extra: new
						{
							scopes = scopes,
							redirect_uri = redirectUri
						});
					}
					else
					{
						tokenResp = await tclient.RequestAuthorizationCodeAsync(code, redirectUri, extra: new
						{
							scopes = scopes
						});
						//values = new Dictionary<string, string>
						//{
						//	{ "client_id", appId},
						//	{ "code", code},
						//	{ "scope", scopes},
						//	{ "redirect_uri", redirectUri},
						//	{ "grant_type", "authorization_code"},
						//	{ "client_secret", appSecret}
						//};
					}




					//var content = new FormUrlEncodedContent(values);

					//var response = await client.PostAsync(MS_TOKENENDPOINT, content);

					//if (!response.IsSuccessStatusCode)
					//{
					//	throw new Exception("Token request failed");
					//}

					//var tokenString = await response.Content.ReadAsStringAsync();

					//var obj = JsonConvert.DeserializeObject(tokenString);

					if (tokenResp.IsError)
					{
						throw new Exception("Token request failed");
					}

					return tokenResp.AccessToken;
				//}
			}
			catch (Exception e)
			{
				var state = Guid.NewGuid().ToString("N");
				sessionCache.SaveUserStateValue(state);

				var request = new AuthorizeRequest(MS_AUTHENDPOINT);
				var url = request.CreateAuthorizeUrl(
					clientId: appId,
					responseType: OidcConstants.ResponseTypes.Code,
					responseMode: OidcConstants.ResponseModes.FormPost,
					scope: scopes,
					redirectUri: redirectUri,
					state: $"{state}{userId}");
				_Context.Response.Redirect(url);
				throw new Exception("Unable to obtain token.");
			}
		}
	}
}