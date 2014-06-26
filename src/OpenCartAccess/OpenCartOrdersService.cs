using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Order;
using OpenCartAccess.Services;

namespace OpenCartAccess
{
	public class OpenCartOrdersService : IOpenCartOrdersService
	{
		private readonly WebRequestServices _webRequestServices;

		public OpenCartOrdersService( OpenCartConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._webRequestServices = new WebRequestServices( config );
		}

		public IEnumerable< OpenCartOrder > GetOrders( DateTime dateFrom, DateTime dateTo )
		{
			var orders = new List< OpenCartOrder >();
			var endpoint = ParamsBuilder.CreateOrdersParams( dateFrom, dateTo );

			var ordersResponse = this._webRequestServices.GetResponse<OpenCartOrdersResponse>(OpenCartCommand.GetOrders, ParamsBuilder.EmptyParams);
			return ordersResponse.Orders;
		}

		public Task< IEnumerable< OpenCartOrder > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			return null;
		}
	}
}