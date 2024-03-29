﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using OpenCartAccess.Misc;
using OpenCartAccess.Models;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Product;
using OpenCartAccess.Services;
using ServiceStack;

namespace OpenCartAccess
{
	public class OpenCartProductsService: IOpenCartProductsService
	{
		private readonly IWebRequestServices _webRequestServices;
		private readonly string _shopUrl;

		internal OpenCartProductsService( OpenCartConfig config, IWebRequestServices webRequestServices )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._webRequestServices = webRequestServices;
			this._shopUrl = config.ShopUrl;
		}

		#region Get
		public bool TryGetProducts( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			try
			{
				var productsResponse = this._webRequestServices.GetResponse< OpenCartProductsResponse >( OpenCartCommand.GetProducts, ParamsBuilder.EmptyParams, mark );
				return true;
			}
			catch( Exception )
			{
				return false;
			}
		}

		public async Task< bool > TryGetProductsAsync( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			try
			{
				var productsResponse = await this._webRequestServices.GetResponseAsync< OpenCartProductsResponse >( OpenCartCommand.GetProducts, ParamsBuilder.EmptyParams, mark );
				return true;
			}
			catch( Exception )
			{
				return false;
			}
		}

		public async Task< IEnumerable< OpenCartProduct > > GetProductsAsync( Mark mark = null )
		{
			var products = new HashSet< OpenCartProduct >();
			mark = mark.CreateNewIfBlank();
			for( var i = 1; i < int.MaxValue; i++ )
			{
				var endpoint = ParamsBuilder.CreateProductsByPageParams( ParamsBuilder.RequestMaxLimit, i );
				var productsResponse = await ActionPolicies.GetPolicyAsync( mark ).Get( async () =>
					await this._webRequestServices.GetResponseAsync< OpenCartProductsResponse >( OpenCartCommand.GetProducts, endpoint, mark ) );
				if( productsResponse.Products == null || !productsResponse.Products.Any() )
					break;

				// OpenCart sites can support paging, or not, or support it in a strange manner disrespecting page size limit, based on the client site version/implementation.
				// When OpenCart endpoint supports paging, somehow the size of returned page can be more than specified in the request. Ideally, we could've check
				// if returned items count in more than specified page size - then paging is not supported. But because of this we cannot apply such a logic.
				// So checking if next page returns new products compared to the previous one. In some cases it will result in one redundant request, but not sure how to guarantee
				// we get all the products otherwise.
				var newProductsResponse = productsResponse.Products.Where( p => p != null ).ToHashSet();
				if ( !this.AreNewProductsReceived( products, newProductsResponse ) )
				{
					OpenCartLogger.Warning( "[{Mark}]\t[OpenCart] Shop {ShopUrl} has problems with pagination. Probably shop has customization which do not follow latest API logic", mark, this._shopUrl );
					break;
				}
				
				products.UnionWith( newProductsResponse );
				if( productsResponse.Products.Count < ParamsBuilder.RequestMaxLimit )
					break;
			}

			return products;
		}
		#endregion

		#region Update
		public void UpdateProducts( IEnumerable< OpenCartProduct > products, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var jsonContent = this.ConvertProductsToJson( products );
			ActionPolicies.SubmitPolicy( mark ).Get(
				() => this._webRequestServices.PutData< OpenCartProductsResponse >( OpenCartCommand.UpdateProducts, ParamsBuilder.EmptyParams, jsonContent, mark ) );
		}

		public async Task UpdateProductsAsync( IEnumerable< OpenCartProduct > products, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var jsonContent = this.ConvertProductsToJson( products );
			await ActionPolicies.SubmitPolicyAsync( mark ).Get(
				async () => await this._webRequestServices.PutDataAsync< OpenCartProductsResponse >( OpenCartCommand.UpdateProducts, ParamsBuilder.EmptyParams, jsonContent, mark ) );
		}
		#endregion

		#region Misc
		private string ConvertProductsToJson( IEnumerable< OpenCartProduct > products )
		{
			var productsToUpdate = products.Select( p => new { product_id = p.Id.ToString( CultureInfo.InvariantCulture ), quantity = p.Quantity.ToString( CultureInfo.InvariantCulture ) } ).ToArray();
			return productsToUpdate.ToJson();
		}

		private bool AreNewProductsReceived( HashSet< OpenCartProduct > existingProducts, HashSet< OpenCartProduct > receivedProducts ) => 
			!existingProducts.IsSupersetOf( receivedProducts );
		#endregion
	}
}