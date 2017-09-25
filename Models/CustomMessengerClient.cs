using Ingeniux.CMS;
using Ingeniux.CMS.Models.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace v10CustomTabQuickStart.Models
{
	public class CustomMessengerClient
	{
		private IUserWriteSession _Session;
		private PushMessenger<Messenger> _Messenger;
		private string _Header;
		private JsonSerializer _serializer = new JsonSerializer();

		public bool EnableDebugLog = false;


		public CustomMessengerClient(IUserWriteSession session, string messageHeader)
		{
			_Session = session;
			_Messenger = new PushMessenger<Messenger>(_Session);
			_Header = messageHeader ?? string.Empty;
		}
		public void SendMessageToAll(object message, string header = null)
		{
			if (string.IsNullOrWhiteSpace(header))
			{
				header = _Header;
			}

			_Messenger.SendToAll(new CustomMessage()
			{
				Header = header,
				Message = message
			});

			if (EnableDebugLog)
			{
				_WriteDebugLog(message);
			}
		}

		public void SendMessageToUsers(IEnumerable<string> userIds, object message, string header = null)
		{
			if (string.IsNullOrWhiteSpace(header))
			{
				header = _Header;
			}

			_Messenger.SendToUsers(new CustomMessage()
			{
				Header = header,
				Message = message
			}, userIds.ToArray());

			if (EnableDebugLog)
			{
				_WriteDebugLog(message);
			}
		}

		public void SendMessageToMe(object message, string header = null)
		{
			if (string.IsNullOrWhiteSpace(header))
			{
				header = _Header;
			}
			_Messenger.SendToMe(new CustomMessage()
			{
				Header = header,
				Message = message
			});

			if (EnableDebugLog)
			{
				_WriteDebugLog(message);
			}
		}

		private void _WriteDebugLog(object message)
		{
			using (StringWriter sw = new StringWriter())
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;
				_serializer.Serialize(writer, message);
				_Session.Debug(sw.ToString());
			}
		}
	}
}