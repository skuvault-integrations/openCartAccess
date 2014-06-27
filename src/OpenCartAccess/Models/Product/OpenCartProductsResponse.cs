using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Product
{
	[ DataContract ]
	internal sealed class OpenCartProductsResponse
	{
		[ DataMember( Name = "data" ) ]
		public IList< OpenCartProduct > Products { get; set; }

		public OpenCartProductsResponse()
		{
			this.Products = new List< OpenCartProduct >();
		}
	}
}