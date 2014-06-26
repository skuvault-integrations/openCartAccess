using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Order
{
	[ DataContract ]
	public sealed class OpenCartOrdersResponse
	{
		[ DataMember( Name = "data" ) ]
		public IList< OpenCartOrder > Orders { get; set; }
	}
}