using Dapplo.HttpExtensions;
using Dapplo.ServiceNow.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.ServiceNow
{
	public class ServiceNowApi
	{
		/// <summary>
		///     Store the specific HttpBehaviour, which contains a IHttpSettings and also some additional logic for making a
		///     HttpClient which works with Jira
		/// </summary>
		private readonly HttpBehaviour _behaviour;

		private string _password;

		private string _user;

		/// <summary>
		///     Create the ServiceNowApi object, here the HttpClient is configured
		/// </summary>
		/// <param name="baseUri">Base URL, e.g. https://servicenow</param>
		/// <param name="httpSettings">IHttpSettings or null for default</param>
		public ServiceNowApi(Uri baseUri, IHttpSettings httpSettings = null)
		{
			if (baseUri == null)
			{
				throw new ArgumentNullException(nameof(baseUri));
			}
			BaseUri = baseUri.AppendSegments("api", "now");

			_behaviour = new HttpBehaviour
			{
				HttpSettings = httpSettings ?? HttpExtensionsGlobals.HttpSettings,
				OnHttpRequestMessageCreated = httpMessage =>
				{
					if (!string.IsNullOrEmpty(_user) && _password != null)
					{
						httpMessage?.SetBasicAuthorization(_user, _password);
					}
					return httpMessage;
				}
			};
		}

		/// <summary>
		///     The base URI for your ServiceNow server
		/// </summary>
		public Uri BaseUri { get; }

		/// <summary>
		///     Set Basic Authentication for the current client
		/// </summary>
		/// <param name="user">username</param>
		/// <param name="password">password</param>
		public void SetBasicAuthentication(string user, string password)
		{
			_user = user;
			_password = password;
		}

		/// <summary>
		/// Get details for an incident
		/// </summary>
		/// <param name="incidentId"></param>
		/// <param name="token"></param>
		/// <returns>dynamic</returns>
		public async Task<string> GetIncident(string incidentId, CancellationToken token = default(CancellationToken))
		{
			// Only use if available
			_behaviour?.MakeCurrent();
			var incidentUri = BaseUri.AppendSegments("table", "incident").ExtendQuery("sysparm_limit", 1).ExtendQuery("sysparm_query", "number=" + incidentId);

			// Specify the fields to retrieve
			if (ServiceNowConfig.IncidentFields?.Length > 0)
			{
				incidentUri = incidentUri.ExtendQuery("sysparm_fields", string.Join(",", ServiceNowConfig.IncidentFields));
			}

			var response = await incidentUri.GetAsAsync<HttpResponse<string, Error>>(token);
			if (response.HasError)
			{
				throw new Exception(response.ErrorResponse.ErrorDetails.Message);
			}
			return response.Response;
		}
	}
}
