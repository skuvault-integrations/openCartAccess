using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Product
{
	[ DataContract ]
	public sealed class OpenCartProduct
	{
		[ DataMember( Name = "id" ) ]
		public int Id { get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku { get; set; }

		[ DataMember( Name = "quantity" ) ]
		public int Quantity { get; set; }
	}
}