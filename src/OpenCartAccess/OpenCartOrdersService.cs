using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using OpenCartAccess.Misc;
using OpenCartAccess.Models;
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

		public bool TryGetOrders( DateTime? dateFrom = null, DateTime? dateTo = null, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			dateFrom = dateFrom ?? DateTime.Now.AddHours( -3 );
			dateTo = dateTo ?? DateTime.Now.AddHours( -2 );

			try
			{
				var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( dateFrom.Value, dateTo.Value );
				var modifiedOrdersResponse = this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint, mark );
				return true;
			}
			catch( Exception )
			{
				return false;
			}
		}

		public async Task< bool > TryGetOrdersAsync( DateTime? dateFrom = null, DateTime? dateTo = null, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			dateFrom = dateFrom ?? DateTime.Now.AddHours( -3 );
			dateTo = dateTo ?? DateTime.Now.AddHours( -2 );

			try
			{
				var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( dateFrom.Value, dateTo.Value );
				var modifiedOrdersResponse = await this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint, mark );
				return true;
			}
			catch( Exception )
			{
				return false;
			}
		}

		public IEnumerable< OpenCartOrder > GetOrders( DateTime dateFrom, DateTime dateTo, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var modifiedOrders = new List< OpenCartOrder >();

			var dateTimeOffset = this.GetDateTimeOffset();
			dateFrom = this.ApplyDateTimeOffset( dateFrom, dateTimeOffset );
			dateTo = this.ApplyDateTimeOffset( dateTo, dateTimeOffset );

			var currentStartDate = dateFrom;
			while( currentStartDate < dateTo )
			{
				var currentEndDate = currentStartDate.AddHours( 4 );
				if( currentEndDate > dateTo )
					currentEndDate = dateTo;

				//var newOrdersEndpoint = ParamsBuilder.CreateNewOrdersParams( currentStartDate, currentEndDate );

				var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( currentStartDate, currentEndDate );
				var modifiedOrdersResponse = ActionPolicies.GetPolicy( mark ).Get( () =>
						this._webRequestServices.GetResponse< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint, mark ) );

				modifiedOrders.AddRange( modifiedOrdersResponse.Orders );
				currentStartDate = currentEndDate;
			}

			ConvertOrdersTimeToUtc( modifiedOrders, dateTimeOffset );
			return modifiedOrders;
		}

		public async Task< IEnumerable< OpenCartOrder > > GetOrdersAsync( DateTime dateFrom, DateTime dateTo, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var modifiedOrders = new List< OpenCartOrder >();

			var dateTimeOffset = await this.GetDateTimeOffsetAsync();
			dateFrom = this.ApplyDateTimeOffset( dateFrom, dateTimeOffset );
			dateTo = this.ApplyDateTimeOffset( dateTo, dateTimeOffset );

			var currentStartDate = dateFrom;
			while( currentStartDate < dateTo )
			{
				var currentEndDate = currentStartDate.AddHours( 4 );
				if( currentEndDate > dateTo )
					currentEndDate = dateTo;

				//var newOrdersEndpoint = ParamsBuilder.CreateNewOrdersParams( currentStartDate, currentEndDate );

				var modifiedOrdersEndpoint = ParamsBuilder.CreateModifiedOrdersParams( currentStartDate, currentEndDate );
				var modifiedOrdersResponse = await ActionPolicies.GetPolicyAsync( mark ).Get( async () =>
						await this._webRequestServices.GetResponseAsync< OpenCartOrdersResponse >( OpenCartCommand.GetOrders, modifiedOrdersEndpoint, mark ) );
				modifiedOrders.AddRange( modifiedOrdersResponse.Orders );
				currentStartDate = currentEndDate;
			}

			ConvertOrdersTimeToUtc( modifiedOrders, dateTimeOffset );
			return modifiedOrders;
		}

		public OpenCartDateTimeUtcOffset GetDateTimeOffset( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var response = ActionPolicies.GetPolicy( mark ).Get( () =>
					this._webRequestServices.GetResponse< OpenCartDateTimeUtcOffsetResponse >( OpenCartCommand.GetUtcOffset, ParamsBuilder.EmptyParams, mark ) );
			return response.Offset;
		}

		public async Task< OpenCartDateTimeUtcOffset > GetDateTimeOffsetAsync( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var response = await ActionPolicies.GetPolicyAsync( mark ).Get( async () =>
					await this._webRequestServices.GetResponseAsync< OpenCartDateTimeUtcOffsetResponse >( OpenCartCommand.GetUtcOffset, ParamsBuilder.EmptyParams, mark ) );
			return response.Offset;
		}

		private DateTime ApplyDateTimeOffset( DateTime baseDateTime, OpenCartDateTimeUtcOffset offset )
		{
			return baseDateTime.AddSeconds( offset.Offset );
		}

		private static void ConvertOrdersTimeToUtc( IEnumerable< OpenCartOrder > modifiedOrders, OpenCartDateTimeUtcOffset dateTimeOffset )
		{
			foreach( var order in modifiedOrders )
			{
				order.CreatedDate = order.CreatedDate.AddSeconds( -1 * dateTimeOffset.Offset );
				order.UpdatedDate = order.UpdatedDate.AddSeconds( -1 * dateTimeOffset.Offset );
			}
		}

	}
}