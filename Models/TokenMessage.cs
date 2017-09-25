using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace v10CustomTabQuickStart.Models
{
	public class TokenMessage
	{
		public bool IsSuccess { get; set; }
		public const string MESSAGE_HEADER = "TokenResponse";
	}
}