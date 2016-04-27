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
		public async Task TestGetIncidentAsync()
		{
			var incident = await _serviceNowApi.GetIncidentAsync("INC0014709");
			Assert.NotNull(incident);
		}
	}
}
