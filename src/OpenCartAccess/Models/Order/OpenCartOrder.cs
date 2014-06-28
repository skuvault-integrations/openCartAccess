using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Order
{
	[ DataContract ]
	public sealed class OpenCartOrder
	{
		[ DataMember( Name = "order_id" ) ]
		public int OrderId { get; set; }

		[ DataMember( Name = "date_added" ) ]
		public DateTime CreatedDate { get; set; }

		[ DataMember( Name = "date_modified" ) ]
		public DateTime UpdatedDate { get; set; }

		[ DataMember( Name = "order_status_id" ) ]
		public string StatusId { get; set; }

		[ DataMember( Name = "total" ) ]
		public decimal OrderTotal { get; set; }

		[ DataMember( Name = "email" ) ]
		public string Email { get; set; }

		[ DataMember( Name = "telephone" ) ]
		public string Phone { get; set; }

		[ DataMember( Name = "payment_firstname" ) ]
		public string PaymentFirstName { get; set; }

		[ DataMember( Name = "payment_lastname" ) ]
		public string PaymentLastName { get; set; }

		[ DataMember( Name = "payment_company" ) ]
		public string PaymentCompany { get; set; }

		[ DataMember( Name = "shipping_address_1" ) ]
		public string ShippingAddress1 { get; set; }

		[ DataMember( Name = "shipping_address_2" ) ]
		public string ShippingAddress2 { get; set; }

		[ DataMember( Name = "shipping_postcode" ) ]
		public string ShippingPostCode { get; set; }

		[ DataMember( Name = "shipping_city" ) ]
		public string ShippingCity { get; set; }

		[ DataMember( Name = "shipping_country" ) ]
		public string ShippingCountry { get; set; }

		[ DataMember( Name = "shipping_zone" ) ]
		public string ShippingZone { get; set; }

		[ DataMember( Name = "shipping_method" ) ]
		public string ShippingMethod { get; set; }

		[ DataMember( Name = "products" ) ]
		public IList< OpenCartOrderProduct > Products { get; set; }
	}

	public enum OpenCartOrderStatusEnum
	{
		MissingOrder,
		Canceled,
		CanceledReversal,
		Chargeback,
		Complete,
		Denied,
		Expired,
		Failed,
		Pending,
		Processed,
		Processing,
		Refunded,
		Reversed,
		Shipped,
		Voided
	}
}