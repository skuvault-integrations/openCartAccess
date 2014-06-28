using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using OpenCartAccess.Misc;
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
			var productsResponse = new OpenCartProductsResponse();
			ActionPolicies.OpenCartGetPolicy.Do( () =>
			{
				productsResponse = this._webRequestServices.GetResponse< OpenCartProductsResponse >( OpenCartCommand.GetProducts, ParamsBuilder.EmptyParams );
			} );
			return productsResponse.Products;
		}

		public async Task< IEnumerable< OpenCartProduct > > GetProductsAsync()
		{
			var productsResponse = new OpenCartProductsResponse();
			await ActionPolicies.OpenCartGetPolicyAsync.Do( async () =>
			{
				productsResponse = await this._webRequestServices.GetResponseAsync< OpenCartProductsResponse >( OpenCartCommand.GetProducts, ParamsBuilder.EmptyParams );
			} );
			return productsResponse.Products;
		}
		#endregion

		#region Update
		public void UpdateProducts( IEnumerable< OpenCartProduct > products )
		{
			var jsonContent = this.ConvertProductsToJson( products );
			ActionPolicies.OpenCartSubmitPolicy.Do( () => this._webRequestServices.PutData( OpenCartCommand.UpdateProducts, ParamsBuilder.EmptyParams, jsonContent ) );
		}

		public async Task UpdateProductsAsync( IEnumerable< OpenCartProduct > products )
		{
			var jsonContent = this.ConvertProductsToJson( products );
			await ActionPolicies.OpenCartSubmitPolicyAsync.Do( async () =>
			{
				await this._webRequestServices.PutDataAsync( OpenCartCommand.UpdateProducts, ParamsBuilder.EmptyParams, jsonContent );
			} );
		}
		#endregion

		#region Misc
		private string ConvertProductsToJson( IEnumerable< OpenCartProduct > products )
		{
			var productsToUpdate = products.Select( p => new { product_id = p.Id, quantity = p.Quantity } ).ToArray();
			return productsToUpdate.ToJson();
		}
		#endregion
	}
}