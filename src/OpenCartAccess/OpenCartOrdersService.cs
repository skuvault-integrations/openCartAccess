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
			var newOrders = new List< OpenCartOrder >();
			var modifiedOrders = new List< OpenCartOrder >();

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
					// TODO: Remove code for getting new orders if all are ok. New orders should be included into modified collection
					var newOrdersResponse = new OpenCartOrdersResponse();//this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, newOrdersEndpoint ) ?? new OpenCartOrdersResponse();
					var modifiedOrdersResponse = this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint ) ?? new OpenCartOrdersResponse();
					newOrders.AddRange( newOrdersResponse.Orders );
					modifiedOrders.AddRange( modifiedOrdersResponse.Orders );
				} );

				currentStartDate = currentEndDate;
			}

			foreach( var newOrder in newOrders )
			{
				if( modifiedOrders.All( x => x.OrderId != newOrder.OrderId ) )
					modifiedOrders.Add( newOrder );
			}
			return modifiedOrders;
		}

		public async Task< IEnumerable< OpenCartOrder > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			var newOrders = new List< OpenCartOrder >();
			var modifiedOrders = new List< OpenCartOrder >();

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
					// TODO: Remove code for getting new orders if all are ok. New orders should be included into modified collection
					var newOrdersResponse = Task.FromResult( new OpenCartOrdersResponse() );// this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, newOrdersEndpoint );
					var modifiedOrdersResponse = this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint );
					await TaskHelper.WhenAll( newOrdersResponse, modifiedOrdersResponse );

					if( newOrdersResponse.Result != null )
						newOrders.AddRange( newOrdersResponse.Result.Orders );
					if( modifiedOrdersResponse.Result != null )
						modifiedOrders.AddRange( modifiedOrdersResponse.Result.Orders );
				} );

				currentStartDate = currentEndDate;
			}

			foreach( var newOrder in newOrders )
			{
				if( modifiedOrders.All( x => x.OrderId != newOrder.OrderId ) )
					modifiedOrders.Add( newOrder );
			}
			return modifiedOrders;
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