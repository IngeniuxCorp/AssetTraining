using Ingeniux.CMS;
using Ingeniux.CMS.WebAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using v10CustomTabQuickStart.Models;
using Microsoft.Graph;
using v10CustomTabQuickStart.Models.Helpers;
using System.Web.Http.Controllers;

namespace v10CustomTabQuickStart.Controllers
{
	[Export(typeof(CMSWebAPIControllerBase))]
	[ExportMetadata("controller", "WebHooksController")]
	[PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
	public class WebHooksController : AutomatedTaskWebAPIController
	{
		bool IsValid;
		Dictionary<string, Models.Notification> Notifications;
		protected override string GetCurrentUserId(HttpControllerContext controllerContext)
		{
			IsValid = false;
			try
			{
				using (var inputStream = new System.IO.StreamReader(Request.Content.ReadAsStreamAsync().Result))
				{
					JObject jsonObject = JObject.Parse(inputStream.ReadToEnd());
					Notifications = _GetNotifications(jsonObject);
					if(!Notifications.Any() || Notifications.Select(n => n.Value.UserId).Distinct().Count() > 1)
					{
						return string.Empty;
					}
					IsValid = true;
					return Notifications.FirstOrDefault().Value?.UserId ?? string.Empty;
				}
			}
			catch
			{
				Notifications = new Dictionary<string, Models.Notification>();
				return string.Empty;
			}
		}

		// GET: WebHooks
		[HttpPost]
		[Route("/Listen")]
		public IHttpActionResult Listen(JObject message, string validationToken = null)
        {
			if (validationToken != null)
			{
				return new ValidationResult(validationToken, Request);
			}

			if (IsValid)
			{
				return Content(System.Net.HttpStatusCode.Accepted, true);
			}

			return Content(System.Net.HttpStatusCode.Unauthorized, false);			
		}

		private static Dictionary<string, Models.Notification> _GetNotifications(JObject message)
		{
			Dictionary<string, Models.Notification> notifications = new Dictionary<string, Models.Notification>();
			if (message != null)
			{
				JArray value = JArray.Parse(message["value"].ToString());
				string userId = string.Empty;
				foreach (var notification in value)
				{
					Models.Notification current = JsonConvert.DeserializeObject<Models.Notification>(notification.ToString());
					if (current.Validate())
					{
						notifications[current.Resource] = current;
					}
				}
			}
			return notifications;
		}
	}
}