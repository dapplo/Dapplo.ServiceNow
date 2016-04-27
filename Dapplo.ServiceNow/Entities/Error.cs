using System.Runtime.Serialization;

namespace Dapplo.ServiceNow.Entities
{
	/// <summary>
	///     Container for the Error
	/// </summary>
	[DataContract]
	public class Error
	{
		/// <summary>
		/// The status for the error
		/// </summary>
		[DataMember(Name = "status")]
		public string Status { get; set; }

		/// <summary>
		/// The status for the error
		/// </summary>
		[DataMember(Name = "error")]
		public ErrorDetails ErrorDetails { get; set; }
	}
}
