using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using OpenCartAccess.Misc;
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
			var newOrdersEndpoint = ParamsBuilder.CreateNewOrdersParams( dateFrom, dateTo );
			var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( dateFrom, dateTo );

			ActionPolicies.OpenCartGetPolicy.Do( () =>
			{
				var newOrdersResponse = this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, newOrdersEndpoint );
				var modifiedOrdersResponse = this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint );
				orders = newOrdersResponse.Orders.Concat( modifiedOrdersResponse.Orders ).ToList();
			} );

			return orders;
		}

		public async Task< IEnumerable< OpenCartOrder > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			var orders = new List< OpenCartOrder >();
			var newOrdersEndpoint = ParamsBuilder.CreateNewOrdersParams( dateFrom, dateTo );
			var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( dateFrom, dateTo );

			await ActionPolicies.OpenCartGetPolicyAsync.Do( async () =>
			{
				var newOrdersResponse = await this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, newOrdersEndpoint );
				var modifiedOrdersResponse = await this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint );
				orders = newOrdersResponse.Orders.Concat( modifiedOrdersResponse.Orders ).ToList();
			} );

			return orders;
		}
	}
}