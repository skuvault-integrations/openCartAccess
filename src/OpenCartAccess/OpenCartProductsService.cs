using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Product;
using OpenCartAccess.Services;
using ServiceStack;

namespace OpenCartAccess
{
	public class OpenCartProductsService : IOpenCartProductsService
	{
		private readonly WebRequestServices _webRequestServices;

		public OpenCartProductsService( OpenCartConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._webRequestServices = new WebRequestServices( config );
		}

		#region Get
		public IEnumerable< OpenCartProduct > GetProducts()
		{
			var productsResponse = this._webRequestServices.GetResponse< OpenCartProductsResponse >( OpenCartCommand.GetProducts, ParamsBuilder.EmptyParams );
			return productsResponse.Products;
		}

		public Task< IEnumerable< OpenCartProduct > > GetProductsAsync()
		{
			return null;
		}
		#endregion

		#region Update
		public void UpdateProducts( IEnumerable< OpenCartProduct > products )
		{
			foreach( var product in products )
				this.UpdateProductQuantity( product );
		}

		public async Task UpdateProductsAsync( IEnumerable< OpenCartProduct > products )
		{
			foreach( var product in products )
				await this.UpdateProductQuantityAsync( product );
		}

		private void UpdateProductQuantity( OpenCartProduct product )
		{
			var jsonContent = new { model = "Product 21", quantity = product.Quantity }.ToJson();
			this._webRequestServices.PutData( OpenCartCommand.UpdateProduct, product.Id.ToString( CultureInfo.InvariantCulture ), jsonContent );
		}

		private async Task UpdateProductQuantityAsync( OpenCartProduct product )
		{

		}
		#endregion
	}
}