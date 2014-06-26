using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Order
{
	[ DataContract ]
	public class OpenCartOrder
	{
		[ DataMember( Name = "order_id" ) ]
		public int OrderId { get; set; }

		[ DataMember( Name = "order_status_id" ) ]
		public string StatusId { get; set; }
	}
}