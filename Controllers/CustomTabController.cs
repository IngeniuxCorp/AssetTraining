using Ingeniux.CMS;
using Ingeniux.CMS.Models.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using v10CustomTabBase.Models;

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
            return View();
        }
    }
}