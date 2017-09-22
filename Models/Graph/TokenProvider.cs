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
using Microsoft.Graph;

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
		private SessionTokenCache _SessionCache;
		private string _UserId;
		public CMS_Common Common { get; private set; }

		public TokenProvider(CMS_Common common, HttpContextBase context)
		{
			Common = common;
			_Context = context;
			_UserId = Common.CurrentUser.UserId;
			_SessionCache = new SessionTokenCache(_UserId, _Context);
			_SaveUserAccessCode();
		}

		private bool _SaveUserAccessCode()
		{
			try
			{
				string stateString = _SessionCache.ReadUserStateValue();

				var code = _Context.Request.Form["code"];
				var responseState = _Context.Request.Form["state"];

				if (!string.IsNullOrWhiteSpace(responseState) && responseState == $"{stateString}{_UserId}")
				{
					_SessionCache.SaveUserCodeValue(code);
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

		public async Task<string> GetUserAccessTokenAsync(bool isXHR = false)
		{
			string token = _SessionCache.ReadUserTokenValue();
			DateTime expiredTime = _SessionCache.ReadTokenExpirationValue();
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

				string refreshToken = _SessionCache.ReadUserRefreshValue();
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
					string code = _SessionCache.ReadUserCodeValue();
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
					_SessionCache.SaveUserTokenValue(tokenResp.AccessToken, DateTime.Now.AddSeconds(tokenResp.ExpiresIn));
					_SessionCache.SaveUserRefreshValue(tokenResp.RefreshToken);
				}

				return tokenResp.AccessToken;
			}
			catch (Exception e)
			{
				var state = Guid.NewGuid().ToString("N");
				_SessionCache.SaveUserStateValue(state);
				var test = _SessionCache.ReadUserStateValue();

				var request = new AuthorizeRequest(MS_AUTHENDPOINT);
				var url = request.CreateAuthorizeUrl(
					clientId: appId,
					responseType: OidcConstants.ResponseTypes.Code,
					responseMode: OidcConstants.ResponseModes.FormPost,
					scope: scopes,
					redirectUri: redirectUri,
					state: $"{state}{_UserId}");

				if (!isXHR)
				{
					_Context.Response.Redirect(url, false);
				}
				throw new ServiceException(
					new Error
					{
						Code = GraphErrorCode.AuthenticationFailure.ToString(),
						Message = "Caller needs to authenticate.",
					});
			}
		}

		public bool HasAccessToken()
		{
			return !string.IsNullOrWhiteSpace(_SessionCache.ReadUserCodeValue());
		}
	}
}