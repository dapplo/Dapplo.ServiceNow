using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dapplo.ServiceNow.Entities
{
	/// <summary>
	///     Details for the Error
	/// </summary>
	[DataContract]
	public class ErrorDetails
	{
		/// <summary>
		/// The message
		/// </summary>
		[DataMember(Name = "message")]
		public string Message { get; set; }

		/// <summary>
		/// The detail
		/// </summary>
		[DataMember(Name = "detail")]
		public string Detail { get; set; }
	}
}
