using Ingeniux.CMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace v10CustomTabQuickStart.Models
{
	public class CustomTabModel
	{
		public bool IsDebug { get {
				#if DEBUG
					return true;
				#else
					return false;
				#endif
			}
		}
		public CMS_Common Common { get; private set; }
		public string FolderName {
			get{
				return _BaseFolder?.Name ?? string.Empty;
			}
		}
		public string CurrentURL
		{
			get
			{
				return _Context.Request.Url.GetLeftPart(UriPartial.Path).ToString();
			}
		}

		public string HubsPath
		{
			get
			{
				return VirtualPathUtility.ToAbsolute("~/signalr/hubs", _Context.Request.ApplicationPath);
			}
		}

		public string AssetPath
		{
			get
			{
				return VirtualPathUtility.ToAbsolute($"~/AppAsset/{FolderName}", _Context.Request.ApplicationPath);
			}
		}

		private DirectoryInfo _BaseFolder { get; set; }
		private HttpContextBase _Context { get; set; }
		public string Message { get; set; }
		public CustomTabModel(CMS_Common common, HttpContextBase context)
		{
			
			_Context = context;

			string path = _Context.Server.MapPath(".");
			_BaseFolder = new DirectoryInfo(path);

			Common = common;
		}
	}
}