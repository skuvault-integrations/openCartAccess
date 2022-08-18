using System;

namespace OpenCartAccess.Services
{
	internal class ParamsBuilder
	{
		internal const int RequestMaxLimit = 250;
		public static readonly string EmptyParams = string.Empty;

		public static string CreateNewOrdersParams( DateTime dateFrom, DateTime dateTo )
		{
			return string.Format( "added_from/{0}/added_to/{1}", dateFrom.ToString( "s" ), dateTo.ToString( "s" ) );
		}

		public static string CreateModifiedOrdersParams( DateTime dateFrom, DateTime dateTo )
		{
			return string.Format( "modified_from/{0}/modified_to/{1}", dateFrom.ToString( "s" ), dateTo.ToString( "s" ) );
		}

		public static string CreateProductsByPageParams( int limit, int page )
		{
			return string.Format( "limit/{0}/page/{1}", limit, page );
		}
	}
}