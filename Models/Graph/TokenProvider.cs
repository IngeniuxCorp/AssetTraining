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
			try
			{
				string userId = Common.CurrentUser.UserId;
				SessionTokenCache sessionCache = new SessionTokenCache(userId, _Context);
				string stateString = sessionCache.ReadUserStateValue();

				var code = _Context.Request.Form["code"];
				var responseState = _Context.Request.Form["state"];

				if (!string.IsNullOrWhiteSpace(responseState) && responseState == $"{stateString}{userId}")
				{
					sessionCache.SaveUserCodeValue(code);
				}
				else if (!string.IsNullOrWhiteSpace(responseState))
				{
					throw new Exception("Invalid state response");
				}
				return true;
			}catch(Exception e)
			{
				return false;
			}
		}

		public async Task<string> GetUserAccessTokenAsync()
		{
			string userId = Common.CurrentUser.UserId;
			SessionTokenCache sessionCache = new SessionTokenCache(userId, _Context);
			string token = sessionCache.ReadUserTokenValue();
			DateTime expiredTime = sessionCache.ReadTokenExpirationValue();
			if (!string.IsNullOrWhiteSpace(token) && DateTime.Now < expiredTime)
			{
				return token;
			}

			try
			{				
				var tclient = new TokenClient(
					MS_TOKENENDPOINT,
					appId,
					appSecret,
					AuthenticationStyle.PostValues);

				string refreshToken = sessionCache.ReadUserRefreshValue();
				TokenResponse tokenResp;
				if (!string.IsNullOrWhiteSpace(refreshToken))
				{
					tokenResp = await tclient.RequestRefreshTokenAsync(refreshToken, extra: new
					{
						scopes = scopes,
						redirect_uri = redirectUri
					});
				}
				else
				{
					string code = sessionCache.ReadUserCodeValue();
					tokenResp = await tclient.RequestAuthorizationCodeAsync(code, redirectUri, extra: new
					{
						scopes = scopes,
					});
				}

				if (tokenResp.IsError)
				{
					throw new Exception("Token request failed");
				}else
				{
					sessionCache.SaveUserTokenValue(tokenResp.AccessToken, DateTime.Now.AddSeconds(tokenResp.ExpiresIn));
					sessionCache.SaveUserRefreshValue(tokenResp.RefreshToken);
				}

				return tokenResp.AccessToken;
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