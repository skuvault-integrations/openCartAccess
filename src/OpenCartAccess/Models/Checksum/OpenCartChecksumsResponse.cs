using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Checksum
{
	[ DataContract ]
	public class OpenCartChecksumsResponse
	{
		[ DataMember( Name = "data" ) ]
		public IList< OpenCartChecksum > Checksums{ get; set; }

		public OpenCartChecksumsResponse()
		{
			this.Checksums = new List< OpenCartChecksum >();
		}
	}
}