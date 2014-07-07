using System;

namespace OpenCartAccess.Services
{
	internal class ParamsBuilder
	{
		public static readonly string EmptyParams = string.Empty;

		public static string CreateNewOrdersParams( DateTime dateFrom, DateTime dateTo )
		{
			return string.Format( "added_from/{0}/added_to/{1}", dateFrom, dateTo );
		}

		public static string CreateModifiedOrdersParams( DateTime dateFrom, DateTime dateTo )
		{
			return string.Format( "modified_from/{0}/modified_to/{1}", dateFrom.ToString( "yyyy-MM-dd HH:mm:ss" ), dateTo.ToString( "yyyy-MM-dd HH:mm:ss" ) );
		}
	}
}