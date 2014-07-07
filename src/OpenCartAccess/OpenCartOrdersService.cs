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

			var dateTimeOffset = this.GetDateTimeOffset();
			dateFrom = this.ApplyDateTimeOffset( dateFrom, dateTimeOffset );
			dateTo = this.ApplyDateTimeOffset( dateTo, dateTimeOffset );

			var newOrdersEndpoint = ParamsBuilder.CreateNewOrdersParams( dateFrom, dateTo );
			var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( dateFrom, dateTo );

			ActionPolicies.OpenCartGetPolicy.Do( () =>
			{
				var newOrdersResponse = this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, newOrdersEndpoint ) ?? new OpenCartOrdersResponse();
				var modifiedOrdersResponse = this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint ) ?? new OpenCartOrdersResponse();
				orders = newOrdersResponse.Orders.Union( modifiedOrdersResponse.Orders ).ToList();
			} );

			return orders;
		}

		public async Task< IEnumerable< OpenCartOrder > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			var orders = new List< OpenCartOrder >();

			var dateTimeOffset = await this.GetDateTimeOffsetAsync();
			dateFrom = this.ApplyDateTimeOffset( dateFrom, dateTimeOffset );
			dateTo = this.ApplyDateTimeOffset( dateTo, dateTimeOffset );

			var newOrdersEndpoint = ParamsBuilder.CreateNewOrdersParams( dateFrom, dateTo );
			var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( dateFrom, dateTo );

			await ActionPolicies.OpenCartGetPolicyAsync.Do( async () =>
			{
				var newOrdersResponse = ( await this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, newOrdersEndpoint ) ) ?? new OpenCartOrdersResponse();
				var modifiedOrdersResponse = ( await this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint ) ) ?? new OpenCartOrdersResponse();
				orders = newOrdersResponse.Orders.Union( modifiedOrdersResponse.Orders ).ToList();
			} );

			return orders;
		}

		public OpenCartDateTimeUtcOffset GetDateTimeOffset()
		{
			OpenCartDateTimeUtcOffset offset = null;
			ActionPolicies.OpenCartGetPolicy.Do( () =>
			{
				var response = this._webRequestServices.GetResponse< OpenCartDateTimeUtcOffsetResponse >( OpenCartCommand.GetUtcOffset, ParamsBuilder.EmptyParams ) ?? new OpenCartDateTimeUtcOffsetResponse();
				offset = response.Offset;
			} );
			return offset;
		}

		public async Task< OpenCartDateTimeUtcOffset > GetDateTimeOffsetAsync()
		{
			OpenCartDateTimeUtcOffset offset = null;
			await ActionPolicies.OpenCartGetPolicyAsync.Do( async () =>
			{
				var response = ( await this._webRequestServices.GetResponseAsync< OpenCartDateTimeUtcOffsetResponse >( OpenCartCommand.GetUtcOffset, ParamsBuilder.EmptyParams ) ) ?? new OpenCartDateTimeUtcOffsetResponse();
				offset = response.Offset;
			} );
			return offset;
		}

		private DateTime ApplyDateTimeOffset( DateTime baseDateTime, OpenCartDateTimeUtcOffset offset )
		{
			return baseDateTime.AddSeconds( offset.Offset );
		}
	}
}