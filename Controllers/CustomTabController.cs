using Ingeniux.CMS;
using Ingeniux.CMS.Models.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using v10CustomTabBase.Models;

namespace v10CustomTabBase.Controllers
{
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


		#region Constructors and helpers
		public CustomTabController():base()
		{
			UserSession = _Common.ContentStore.OpenWriteSession(_Common.CurrentUser);
			Messenger = new CustomMessenger(UserSession, HEADER_VALUE);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if(UserSession != null && !UserSession.Closed)
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