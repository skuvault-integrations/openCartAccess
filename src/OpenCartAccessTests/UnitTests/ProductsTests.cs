using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using OpenCartAccess;
using OpenCartAccess.Models;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Product;
using OpenCartAccess.Services;

namespace OpenCartAccessTests.UnitTests
{
	[ TestFixture ]
	public class ProductsTests
	{
		private IWebRequestServices _webRequestServiceMock;
		private readonly Random _random = new Random();
		private OpenCartProductsService _openCartProductsService;

		[ SetUp ]
		public void Init()
		{
			this._webRequestServiceMock = Substitute.For< IWebRequestServices >();
			var apiKey = Guid.NewGuid().ToString();
			var shopUrl = Guid.NewGuid().ToString();
			var config = new OpenCartConfig( apiKey, shopUrl );
			this._openCartProductsService = new OpenCartProductsService( config, this._webRequestServiceMock );
		}

		[ Test ]
		public async Task GetProductsAsync_AllProductsAreReturned_andOneRequestIsSent_WhenPagingIsWorking_AndProductsCountIsLessThanPageSize()
		{
			// Arrange
			var mockProducts = this.MockGetProductsByPages( ParamsBuilder.RequestMaxLimit - 1 );

			// Act
			var products = await this._openCartProductsService.GetProductsAsync();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public async Task GetProductsAsync_AllProductsAreReturned_andTwoRequestsAreSent_WhenPagingIsWorking_AndProductCountEqualsPageSize()
		{
			// Arrange
			var mockProducts = this.MockGetProductsByPages( ParamsBuilder.RequestMaxLimit );

			// Act
			var products = await this._openCartProductsService.GetProductsAsync();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );
			
			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public async Task GetProductsAsync_AllProductsAreReturned_andTwoRequestAreSent_WhenPagingIsWorking_AndProductsCountIsMoreThanPageSize()
		{
			// Arrange
			var mockProducts = this.MockGetProductsByPages( ParamsBuilder.RequestMaxLimit + 1 );

			// Act
			var products = await this._openCartProductsService.GetProductsAsync();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );

			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public async Task GetProductsAsync_AllProductsAreReturned_andOneRequestIsSent_WhenPagingIsNotWorking_AndProductsCountIsLessThanPageSize()
		{
			// Arrange
			var mockProducts = this.MockGetProductsNoPaging( ParamsBuilder.RequestMaxLimit - 1 );

			// Act
			var products = await this._openCartProductsService.GetProductsAsync();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public async Task GetProductsAsync_AllProductsAreReturned_andTwoRequestsAreSent_WhenPagingIsNotWorking_AndProductsCountEqualsPageSize()
		{
			// Arrange
			var mockProducts = this.MockGetProductsNoPaging( ParamsBuilder.RequestMaxLimit );

			// Act
			var products = await this._openCartProductsService.GetProductsAsync();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );
			
			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public async Task GetProductsAsync_AllProductsAreReturned_andTwoRequestAreSent_WhenPagingIsNotWorking_AndProductsCountIsMoreThanPageSize()
		{
			// Arrange
			var mockProducts = this.MockGetProductsNoPaging( ParamsBuilder.RequestMaxLimit + 1 );

			// Act
			var products = await this._openCartProductsService.GetProductsAsync();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );

			await this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() );
		}

		private IList< OpenCartProduct > MockGetProductsNoPaging( int productsCount )
		{
			var products = this.GenerateOpenCartProducts( productsCount );
			this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					Arg.Any< string >(),
					Arg.Any< Mark >() )
				.Returns( new OpenCartProductsResponse { Products = products } );
			return products;
		}

		private IList< OpenCartProduct > MockGetProductsByPages( int productsCount )
		{
			var products = this.GenerateOpenCartProducts( productsCount );
			var pageNumber = Math.Ceiling( ( decimal )products.Count / ParamsBuilder.RequestMaxLimit );
            
			// if count of products can be get integer number of pages then we should make additional request because we should ask data while count of products
			// more or equals page size
			if( products.Count % ParamsBuilder.RequestMaxLimit == 0 )
				pageNumber++;

			for( var i = 1; i <= pageNumber; i++ )
				this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
						OpenCartCommand.GetProducts,
						$"limit/{ParamsBuilder.RequestMaxLimit}/page/{i}",
						Arg.Any< Mark >() )
					.Returns( new OpenCartProductsResponse
					{
						Products = products.Skip( ( i - 1 ) * ParamsBuilder.RequestMaxLimit ).Take( ParamsBuilder.RequestMaxLimit ).ToList()
					} );
			
			return products;
		}
		
		private List< OpenCartProduct > GenerateOpenCartProducts( int count )
		{
			var products = new List< OpenCartProduct >();
			for( var i = 0; i < count; i++ )
				products.Add( new OpenCartProduct
				{
					Id = this._random.Next(),
					Sku = Guid.NewGuid().ToString(),
					Quantity = this._random.Next()
				} );

			return products;
		}
	}
}