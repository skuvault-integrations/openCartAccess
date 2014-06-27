using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Order
{
	[ DataContract ]
	internal sealed class OpenCartOrdersResponse
	{
		[ DataMember( Name = "data" ) ]
		public IList< OpenCartOrder > Orders { get; set; }

		public OpenCartOrdersResponse()
		{
			this.Orders = new List< OpenCartOrder >();
		}
	}
}