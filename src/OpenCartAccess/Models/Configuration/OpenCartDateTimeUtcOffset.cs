using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Configuration
{
	[ DataContract ]
	public class OpenCartDateTimeUtcOffset
	{
		[ DataMember( Name = "offset" ) ]
		public long Offset{ get; set; }
	}
}