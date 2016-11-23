using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenCartAccess.Models;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Order;

namespace OpenCartAccess
{
	public interface IOpenCartOrdersService
	{
		bool TryGetOrders( DateTime? dateFrom = null, DateTime? dateTo = null, Mark mark = null );
		Task< bool > TryGetOrdersAsync( DateTime? dateFrom = null, DateTime? dateTo = null, Mark mark = null );

		IEnumerable< OpenCartOrder > GetOrders( DateTime dateFrom, DateTime dateTo, Mark mark = null );
		Task< IEnumerable< OpenCartOrder > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo, Mark mark = null );

		OpenCartDateTimeUtcOffset GetDateTimeOffset( Mark mark = null );
		Task< OpenCartDateTimeUtcOffset > GetDateTimeOffsetAsync( Mark mark = null );
	}
}