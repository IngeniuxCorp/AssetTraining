using Ingeniux.CMS;
using Ingeniux.CMS.Models.Messaging;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using v10CustomTabQuickStart.Models;
using v10CustomTabQuickStart.Models.Graph;
using v10CustomTabQuickStart.Models.Helpers;

namespace v10CustomTabBase.Controllers
{
	//TIP: delete 'fcnMode="Disabled"' from the site's web config
	//this will make it so you can make view changes without recycling app pool
	//only do this when developing, do not remove it for production
	[Export(typeof(CMSControllerBase))]
	//This value is always the name of the controller.
	[ExportMetadata("controller", "CustomTabController")]
	[PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
	public class CustomTabController : CustomTabApplicationController
	{
		//A unique value that allows the front end to filter custom messages.
		public const string HEADER_VALUE = "CustomTabHeader";
		public IUserWriteSession UserSession;
		public CustomMessengerClient Messenger;
		public TokenProvider TokenProvider;
		public GraphServiceClient GraphClient;

		#region Initialize and helpers

		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);
			UserSession = _Common.ContentStore.OpenWriteSession(_Common.CurrentUser);
			Messenger = new CustomMessengerClient(UserSession, HEADER_VALUE);
			TokenProvider = new TokenProvider(_Common, HttpContext);
			GraphClient = new GraphServiceClient(
				new DelegateAuthenticationProvider(
					async (requestMessage) =>
					{
						TokenProvider provider = TokenProvider;
						string accessToken = await provider.GetUserAccessTokenAsync(true);

						// Append the access token to the request.
						requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
					}));
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (UserSession != null && !UserSession.Closed)
			{
				UserSession.Dispose();
			}
		}
		#endregion

		// GET: CustomTab
		public ActionResult Index()
        {
			var model = new CustomTabModel(_Common, HttpContext);
			model.HasToken = TokenProvider.HasAccessToken();
			return View(model);
        }

		public async Task<ActionResult> SignInResponse()
		{
			return await Task.Run<ActionResult>(() =>
			{
				if (TokenProvider.HasAccessToken())
				{
					if (TokenProvider.IsTokenResponse())
					{
						Messenger.SendMessageToMe(
							new TokenMessage()
							{
								IsSuccess = true
							},
							TokenMessage.MESSAGE_HEADER
						);
					}
					return RedirectToAction("Index");
				}
				else
				{
					return RedirectToAction("SignIn");
				}
			});
		}

		public async Task<ActionResult> SignIn()
		{
			var model = new CustomTabModel(_Common, HttpContext);
			model.HasToken = TokenProvider.HasAccessToken();
			if (model.HasToken)
			{
				return View("Index", model);
			}

			try
			{
				var token = await TokenProvider.GetUserAccessTokenAsync();
			}
			catch
			{
				model.Message = "Please Login";
				return View();
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> CreateEmailSubscription(string path)
		{
			var subscriptions = await GraphClient.Subscriptions.Request().GetAsync();
			subscriptions.Add(new Subscription()
			{
				ChangeType = "created,updated",
				Resource = "/me/mailfolders('inbox')/messages",
				ExpirationDateTime = DateTime.Now.AddDays(3),
				NotificationUrl = _Common.ServerUrl,
				
				ClientState = JWTHelper.GetJwtToken("graph.microsoft.com", "GraphDemo", new StateData() {
					Type = StateTypes.Email,
					UserId = _Common.CurrentUser.UserId
				}
				, DateTime.Now)
			});
			return Content(JsonConvert.SerializeObject(true), "application/json");
		}

		[HttpPost]
		public async Task<ActionResult> GetDriveItems(string path)
		{
			IDriveItemChildrenCollectionPage driveChildren;
			if (string.IsNullOrWhiteSpace(path))
			{
				driveChildren = await GraphClient.Me.Drive.Root.Children.Request().GetAsync();
			}
			else
			{
				driveChildren = await GraphClient.Me.Drive.Root.ItemWithPath(path).Children.Request().GetAsync();
			}
			return Content(JsonConvert.SerializeObject(driveChildren), "application/json");
		}

		const string ASSETFOLDER_ID = "";

		[HttpPost]
		public async Task<ActionResult> ImportDriveItem(string id, string schemaId)
		{
			//TODO: Implement this.
			DriveItem driveItem = await GraphClient.Me.Drive.Items[id].Request().GetAsync();
			Stream fileStream = driveItem.Content;



			IAsset asset = new Asset();
			return Content(JsonConvert.SerializeObject(asset), "application/json");
		}

		[HttpPost]
		public ActionResult GetAssetSchemas(string fileExtention)
		{
			// TODO: Implement this.
			IEnumerable<IAssetSchema> assetSchemas = new IAssetSchema[0];
			return Content(JsonConvert.SerializeObject(assetSchemas), "application/json");
		}
	}
}