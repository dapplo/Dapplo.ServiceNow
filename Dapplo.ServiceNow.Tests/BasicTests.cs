using Dapplo.LogFacade;
using Dapplo.ServiceNow.Tests.Logger;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.ServiceNow.Tests
{
	public class BasicTests
	{
		private ServiceNowApi _serviceNowApi;
		public BasicTests(ITestOutputHelper testOutputHelper)
		{
			XUnitLogger.RegisterLogger(testOutputHelper, LogLevel.Verbose);
			_serviceNowApi = new ServiceNowApi(new System.Uri("https://demo006.service-now.com"));
			var username = "itil";
			var password = "itil";
			if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
			{
				_serviceNowApi.SetBasicAuthentication(username, password);
			}
		}

		[Fact]
		public async Task GetIncidentAsync()
		{
			var incident = await _serviceNowApi.GetIncident("INC0014709");
			Assert.NotNull(incident);
		}

		/// <summary>
		/// See: http://www.john-james-andersen.com/blog/service-now/generate-attachments-in-servicenow-via-rest.html
		/// Or: https://docs.servicenow.com/bundle/geneva-servicenow-platform/page/integrate/inbound_rest/concept/c_AttachmentAPI.html
		/// Especially here: https://docs.servicenow.com/bundle/geneva-servicenow-platform/page/integrate/inbound_rest/reference/r_AttachmentAPI-POST.html
		/// </summary>
		/// <returns></returns>
		public async Task AttachAsync()
		{

		}
	}
}
