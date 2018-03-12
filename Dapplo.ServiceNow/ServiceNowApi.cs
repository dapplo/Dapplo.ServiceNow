#region Dapplo 2016 - GNU Lesser General Public License

// Dapplo - building blocks for .NET applications
// Copyright (C) 2016 Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Dapplo.ServiceNow
// 
// Dapplo.ServiceNow is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Dapplo.ServiceNow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Dapplo.ServiceNow. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#endregion

#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions;
using Dapplo.ServiceNow.Entities;

#endregion

namespace Dapplo.ServiceNow
{
	/// <summary>
	///     A ServiceNow client
	///     Might need to switch to
	///     <a href="http://wiki.servicenow.com/index.php?title=Generating_OAuth_Tokens#gsc.tab=0">OAuth</a>
	///     Although there is not much improvement if the API still needs the username and password!
	///     http://stackoverflow.com/questions/31137312/servicenow-oauth2-0-authorization-url-and-token-url
	///     http://stackoverflow.com/questions/35643629/how-to-get-clientid-and-clientsecret-for-oauth-2-0-authentication-in-servicenow
	/// </summary>
	public class ServiceNowApi
	{
		/// <summary>
		///     Store the specific HttpBehaviour, which contains a IHttpSettings and also some additional logic for making a
		///     HttpClient which works with Jira
		/// </summary>
		private readonly HttpBehaviour _behaviour;

		private string _password;

		private string _user;

		private string _userToken;

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
					if (!string.IsNullOrEmpty(_userToken))
					{
						httpMessage?.AddRequestHeader("X-UserToken", _userToken);
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

		public void SetUserToken(string userToken)
		{
			_userToken = userToken;
		}

		/// <summary>
		///     Get details for an incident
		/// </summary>
		/// <param name="incidentId"></param>
		/// <param name="token"></param>
		/// <returns>dynamic</returns>
		public async Task<string> GetIncidentAsync(string incidentId, CancellationToken token = default)
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

		/// <summary>
		///     See: http://www.john-james-andersen.com/blog/service-now/generate-attachments-in-servicenow-via-rest.html
		///     Or:
		///     https://docs.servicenow.com/bundle/geneva-servicenow-platform/page/integrate/inbound_rest/concept/c_AttachmentAPI.html
		///     Especially here:
		///     https://docs.servicenow.com/bundle/geneva-servicenow-platform/page/integrate/inbound_rest/reference/r_AttachmentAPI-POST.html
		/// </summary>
		/// <returns></returns>
		public async Task AttachAsync()
		{
			await Task.Delay(10);
		}

		/// <summary>
		///     See: https://service-now.com/api/now/table/sys_user?sysparm_query=user_name=username
		/// </summary>
		/// <returns></returns>
		public async Task GetUserAsync(string username, CancellationToken token = default)
		{
			await Task.Delay(10);
		}
	}
}