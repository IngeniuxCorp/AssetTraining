using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace v10CustomTabBase.Models
{
	public class CustomMessage
	{
		private const string _SENDER = "CustomTabMessage"; 
		public object Message;
		public string Header = string.Empty;
		public string Sender { get
			{
				return _SENDER;
			}
		}
	}
}