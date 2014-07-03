using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Checksum
{
	[ DataContract ]
	public class OpenCartChecksum
	{
		[ DataMember( Name = "checksum" ) ]
		public string Value { get; set; }
	}
}