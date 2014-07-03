namespace OpenCartAccess.Models.Configuration
{
	internal class OpenCartCommand
	{
		public static readonly OpenCartCommand Unknown = new OpenCartCommand( string.Empty );
		public static readonly OpenCartCommand GetProducts = new OpenCartCommand( "/api/rest/products" );
		public static readonly OpenCartCommand GetOrders = new OpenCartCommand( "/api/rest/orders/details/" );
		public static readonly OpenCartCommand UpdateProducts = new OpenCartCommand( "/api/rest/bulk_products" );
		public static readonly OpenCartCommand GetChecksums = new OpenCartCommand( "/api/rest/checksums" );

		private OpenCartCommand( string command )
		{
			this.Command = command;
		}

		public string Command { get; private set; }
	}
}