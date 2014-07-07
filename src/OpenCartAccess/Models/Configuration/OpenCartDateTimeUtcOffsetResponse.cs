using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Configuration
{
	[ DataContract ]
	internal class OpenCartDateTimeUtcOffsetResponse
	{
		[ DataMember( Name = "data" ) ]
		public OpenCartDateTimeUtcOffset Offset { get; set; }
	}
}