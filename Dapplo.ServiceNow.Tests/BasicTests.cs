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
using System.Threading.Tasks;
using Dapplo.Log.Facade;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.ServiceNow.Tests
{
	public class BasicTests
	{
		public BasicTests(ITestOutputHelper testOutputHelper)
		{
			LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
			_serviceNowApi = new ServiceNowApi(new Uri("https://demo006.service-now.com"));
			var username = "itil";
			var password = "itil";
			if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
			{
				_serviceNowApi.SetBasicAuthentication(username, password);
			}
		}

		private readonly ServiceNowApi _serviceNowApi;

		[Fact]
		public async Task TestGetIncidentAsync()
		{
			var incident = await _serviceNowApi.GetIncidentAsync("INC0014709");
			Assert.NotNull(incident);
		}
	}
}