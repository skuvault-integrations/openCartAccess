using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Order
{
	[ DataContract ]
	public class OpenCartOrderProduct
	{
		[ DataMember( Name = "sku" ) ]
		public string Sku { get; set; }

		[ DataMember( Name = "quantity" ) ]
		public int Quantity { get; set; }

		[ DataMember( Name = "price" ) ]
		public string Price { get; set; }
	}
}