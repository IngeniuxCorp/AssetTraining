using Ingeniux.CMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace v10CustomTabQuickStart.Models
{
	public class CustomTabModel
	{
		public bool IsDebug = false;
		public CMS_Common Common { get; private set; }

		public CustomTabModel(CMS_Common common)
		{
			Common = common;
		}
	}
}