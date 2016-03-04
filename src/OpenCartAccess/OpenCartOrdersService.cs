using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Netco.Extensions;
using OpenCartAccess.Misc;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Order;
using OpenCartAccess.Services;

namespace OpenCartAccess
{
	public class OpenCartOrdersService: IOpenCartOrdersService
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

			var currentStartDate = dateFrom;
			while( currentStartDate < dateTo )
			{
				var currentEndDate = currentStartDate.AddDays( 1 );
				if( currentEndDate > dateTo )
					currentEndDate = dateTo;

				var newOrdersEndpoint = ParamsBuilder.CreateNewOrdersParams( currentStartDate, currentEndDate );
				var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( currentStartDate, currentEndDate );

				ActionPolicies.OpenCartGetPolicy.Do( () =>
				{
					var newOrdersResponse = this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, newOrdersEndpoint ) ?? new OpenCartOrdersResponse();
					var modifiedOrdersResponse = this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint ) ?? new OpenCartOrdersResponse();
					orders.AddRange( newOrdersResponse.Orders );
					orders.AddRange( modifiedOrdersResponse.Orders );
				} );

				currentStartDate = currentEndDate;
			}
			return orders;
		}

		public async Task< IEnumerable< OpenCartOrder > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			var orders = new List< OpenCartOrder >();

			var dateTimeOffset = await this.GetDateTimeOffsetAsync();
			dateFrom = this.ApplyDateTimeOffset( dateFrom, dateTimeOffset );
			dateTo = this.ApplyDateTimeOffset( dateTo, dateTimeOffset );

			var currentStartDate = dateFrom;
			while( currentStartDate < dateTo )
			{
				var currentEndDate = currentStartDate.AddDays( 1 );
				if( currentEndDate > dateTo )
					currentEndDate = dateTo;

				var newOrdersEndpoint = ParamsBuilder.CreateNewOrdersParams( currentStartDate, currentEndDate );
				var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( currentStartDate, currentEndDate );

				await ActionPolicies.OpenCartGetPolicyAsync.Do( async () =>
				{
					var newOrdersResponse = this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, newOrdersEndpoint );
					var modifiedOrdersResponse = this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint );
					await TaskHelper.WhenAll( newOrdersResponse, modifiedOrdersResponse );

					if( newOrdersResponse.Result != null )
						orders.AddRange( newOrdersResponse.Result.Orders );
					if( modifiedOrdersResponse.Result != null )
						orders.AddRange( modifiedOrdersResponse.Result.Orders );
				} );

				currentStartDate = currentEndDate;
			}
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