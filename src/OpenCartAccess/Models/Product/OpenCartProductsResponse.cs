using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Product
{
	[ DataContract ]
	internal sealed class OpenCartProductsResponse
	{
		[ DataMember( Name = "success" ) ]
		public string Status { get; set; }

		[ DataMember( Name = "error" ) ]
		public string Error { get; set; }

		[ DataMember( Name = "data" ) ]
		public IList< OpenCartProduct > Products { get; set; }

		public OpenCartProductsResponse()
		{
			this.Products = new List< OpenCartProduct >();
		}
	}
}