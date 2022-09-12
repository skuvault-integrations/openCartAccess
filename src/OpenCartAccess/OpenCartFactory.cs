using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Services;

namespace OpenCartAccess
{
	public interface IOpenCartFactory
	{
		IOpenCartOrdersService CreateOrdersService( OpenCartConfig config );
		IOpenCartProductsService CreateProductsService( OpenCartConfig config );

		IOpenCartChecksumService CreateChecksumService( OpenCartConfig config );
	}

	public class OpenCartFactory : IOpenCartFactory
	{
		public IOpenCartOrdersService CreateOrdersService( OpenCartConfig config )
		{
			return new OpenCartOrdersService( config, new WebRequestServices( config ) );
		}

		public IOpenCartProductsService CreateProductsService( OpenCartConfig config )
		{
			return new OpenCartProductsService( config, new WebRequestServices( config ) );
		}

		public IOpenCartChecksumService CreateChecksumService( OpenCartConfig config )
		{
			return new OpenCartChecksumService( config, new WebRequestServices( config ) );
		}
	}
}