using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace v10CustomTabQuickStart.Models
{
	public enum StateTypes
	{
		Assets,
		Email
	}

	public class StateData
	{
		public string UserId { get; set; }
		public StateTypes Type { get; set; }
	}
}