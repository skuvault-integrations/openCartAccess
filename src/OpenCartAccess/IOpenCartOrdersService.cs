using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Order;

namespace OpenCartAccess
{
	public interface IOpenCartOrdersService
	{
		IEnumerable< OpenCartOrder > GetOrders( DateTime dateFrom, DateTime dateTo );
		Task< IEnumerable< OpenCartOrder > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo );

		OpenCartDateTimeUtcOffset GetDateTimeOffset();
		Task< OpenCartDateTimeUtcOffset > GetDateTimeOffsetAsync();
	}
}