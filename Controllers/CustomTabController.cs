using Ingeniux.CMS;
using Ingeniux.CMS.Models.Messaging;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using v10CustomTabBase.Models;
using v10CustomTabQuickStart.Models;
using v10CustomTabQuickStart.Models.Graph;

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
		public CustomMessenger Messenger;

		#region Initialize and helpers

		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);
			UserSession = _Common.ContentStore.OpenWriteSession(_Common.CurrentUser);
			Messenger = new CustomMessenger(UserSession, HEADER_VALUE);
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
			return View(model);
        }

		public async Task<ActionResult> Test()
		{
			var model = new CustomTabModel(_Common, HttpContext);
			GraphServiceClient graphClient = null;
			try
			{
				graphClient = new GraphServiceClient(
					new DelegateAuthenticationProvider(
						async (requestMessage) =>
						{
							TokenProvider provider = new TokenProvider(_Common, HttpContext);
							string accessToken = await provider.GetUserAccessTokenAsync();

							// Append the access token to the request.
							requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
						}));
				Microsoft.Graph.User me = await graphClient.Me.Request().Select("mail,userPrincipalName").GetAsync();
			}
			catch
			{
				model.Message = "Please Login";
				return View(model);
			}

			if(graphClient == null)
			{
				model.Message = "Graph is null";
				return View(model);
			}

			var driveChildren = await graphClient.Me.Drive.Root.Children.Request().GetAsync();
			model.Message = driveChildren.Select(c => c.Name).Aggregate((cur, next) => $"{cur},{next}");
			return View(model);
		}
    }
}