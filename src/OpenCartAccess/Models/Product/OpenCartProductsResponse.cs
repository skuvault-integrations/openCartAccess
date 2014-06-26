using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Product
{
	[ DataContract ]
	public sealed class OpenCartProductsResponse
	{
		[ DataMember( Name = "data" ) ]
		public IList< OpenCartProduct > Products { get; set; }
	}
}