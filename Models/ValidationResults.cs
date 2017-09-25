using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace v10CustomTabQuickStart.Models
{
	public class ValidationResult : IHttpActionResult
	{
		string _token;
		HttpRequestMessage _request;

		public ValidationResult(string token, HttpRequestMessage request)
		{
			_token = token;
			_request = request;
		}

		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			var response = new HttpResponseMessage()
			{
				Content = new StringContent(_token),
				RequestMessage = _request
			};
			return Task.FromResult(response);
		}
	}
}