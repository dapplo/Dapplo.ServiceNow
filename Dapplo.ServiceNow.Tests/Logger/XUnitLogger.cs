using Dapplo.LogFacade;
using Dapplo.LogFacade.Loggers;
using System;
using System.Threading;
using Xunit.Abstractions;

namespace Dapplo.ServiceNow.Tests.Logger
{
	/// <summary>
	///     xUnit will have tests run parallel, and due to this it won't capture trace output correctly.
	///     This is where their ITestOutputHelper comes around, but Dapplo.LogFacade can only have one logger.
	///     This class solves the problem by registering the ITestOutputHelper in the CallContext.
	///     Every log statement will retrieve the ITestOutputHelper from the context and use it to log.
	/// </summary>
	public class XUnitLogger : AbstractLogger
	{
		private static readonly AsyncLocal<ITestOutputHelper> TestOutputHelperAsyncLocal = new AsyncLocal<ITestOutputHelper>();
		private static readonly AsyncLocal<LogLevel> LogLevelAsyncLocal = new AsyncLocal<LogLevel>();

		/// <summary>
		///     Prevent the constructor from being use elsewhere
		/// </summary>
		private XUnitLogger()
		{
		}

		/// <summary>
		///     LogLevel, this can give a different result pro xUnit test...
		///     It will depend on the RegisterLogger value which was used in the current xUnit test
		/// </summary>
		public override LogLevel Level
		{
			get
			{
				return LogLevelAsyncLocal.Value;
			}
			set { LogLevelAsyncLocal.Value = value; }
		}

		/// <summary>
		///     If the level is enabled, this returns true
		///     The level depends on what the xUnit test used in the RegisterLogger
		/// </summary>
		/// <param name="level">LogLevel</param>
		/// <returns>true if the level is enabled</returns>
		public override bool IsLogLevelEnabled(LogLevel level)
		{
			return level != LogLevel.None && level >= Level;
		}

		public override void Write(LogInfo logInfo, string messageTemplate, params object[] logParameters)
		{
			var testOutputHelper = TestOutputHelperAsyncLocal.Value;
			if (testOutputHelper == null)
			{
				throw new ArgumentNullException(nameof(testOutputHelper), "Couldn't find a ITestOutputHelper in the CallContext");
			}
			testOutputHelper.WriteLine($"{logInfo} - {messageTemplate}", logParameters);
		}

		public override void Write(LogInfo logInfo, Exception exception, string messageTemplate = null, params object[] logParameters)
		{
			var testOutputHelper = TestOutputHelperAsyncLocal.Value;
			if (testOutputHelper == null)
			{
				throw new ArgumentNullException(nameof(testOutputHelper), "Couldn't find a ITestOutputHelper in the CallContext");
			}
			if (messageTemplate != null)
			{
				testOutputHelper.WriteLine($"{logInfo} - {messageTemplate}", logParameters);
			}
			if (exception != null)
			{
				testOutputHelper.WriteLine(exception.ToString());
			}
		}

		/// <summary>
		///     Register the XUnitLogger,  as the global LogFacade logger
		///     This also places the ITestOutputHelper in the CallContext, so the output is mapped to the xUnit test
		/// </summary>
		/// <param name="testOutputHelper">ITestOutputHelper</param>
		/// <param name="level">LogLevel, when none is given the LogSettings.DefaultLevel is used</param>
		public static void RegisterLogger(ITestOutputHelper testOutputHelper, LogLevel level = default(LogLevel))
		{
			TestOutputHelperAsyncLocal.Value = testOutputHelper;
			LogLevelAsyncLocal.Value = level == LogLevel.None ? LogSettings.DefaultLevel : level;
			if (!(LogSettings.Logger is XUnitLogger))
			{
				LogSettings.Logger = new XUnitLogger();
			}
		}
	}

}
