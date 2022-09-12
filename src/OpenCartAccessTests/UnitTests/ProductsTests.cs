using System;
using System.Collections.Generic;
using System.Linq;
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
		public void GetProductsAsync_AllProducts_andOneRequest_WhenPagingIsWorkingForClient_AndProductsLessThanPageSize()
		{
			// Arrange
			var mockProducts = this.InitOpenCartProducts( ParamsBuilder.RequestMaxLimit - 1 );

			this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() )
				.Returns( new OpenCartProductsResponse
				{
					Products = mockProducts.Take( ParamsBuilder.RequestMaxLimit ).ToList()
				} );

			// Act
			var products = this._openCartProductsService.GetProductsAsync().GetAwaiter().GetResult();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public void GetProductsAsync_AllProducts_andOneRequest_WhenPagingIsWorkingForClient_AndProductsEqualsPageSize()
		{
			// Arrange
			var mockProducts = this.InitOpenCartProducts( ParamsBuilder.RequestMaxLimit );

			this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() )
				.Returns( new OpenCartProductsResponse
				{
					Products = mockProducts.Take( ParamsBuilder.RequestMaxLimit ).ToList()
				} );
			
			this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() )
				.Returns( new OpenCartProductsResponse
				{
					Products = new List< OpenCartProduct >()
				} );

			// Act
			var products = this._openCartProductsService.GetProductsAsync().GetAwaiter().GetResult();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );
			
			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public void GetProductsAsync_AllProducts_andTwoRequest_WhenPagingIsWorkingForClient_AndProductsMoreThanPageSize_1()
		{
			// Arrange
			var mockProducts = this.InitOpenCartProducts( ParamsBuilder.RequestMaxLimit + 1 );

			this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() )
				.Returns( new OpenCartProductsResponse
				{
					Products = mockProducts.Take( ParamsBuilder.RequestMaxLimit ).ToList()
				} );

			this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() )
				.Returns( new OpenCartProductsResponse
				{
					Products = mockProducts.Skip( ParamsBuilder.RequestMaxLimit ).Take( ParamsBuilder.RequestMaxLimit ).ToList()
				} );

			// Act
			var products = this._openCartProductsService.GetProductsAsync().GetAwaiter().GetResult();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );

			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public void GetProductsAsync_AllProducts_andOneRequest_WhenPagingIsNotWorkingForClient_AndProductsLessThanPageSize()
		{
			// Arrange
			var mockProducts = this.InitOpenCartProducts( ParamsBuilder.RequestMaxLimit - 1 );

			this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					Arg.Any< string >(),
					Arg.Any< Mark >() )
				.Returns( new OpenCartProductsResponse { Products = mockProducts } );

			// Act
			var products = this._openCartProductsService.GetProductsAsync().GetAwaiter().GetResult();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public void GetProductsAsync_AllProducts_andOneRequest_WhenPagingIsNotWorkingForClient_AndProductsEqualsPageSize()
		{
			// Arrange
			var mockProducts = this.InitOpenCartProducts( ParamsBuilder.RequestMaxLimit );

			this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					Arg.Any< string >(),
					Arg.Any< Mark >() )
				.Returns( new OpenCartProductsResponse { Products = mockProducts } );

			// Act
			var products = this._openCartProductsService.GetProductsAsync().GetAwaiter().GetResult();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );
			
			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() );
		}

		[ Test ]
		public void GetProductsAsync_AllProducts_andTwoRequest_WhenPagingIsNotWorkingForClient_AndProductsMoreThanPageSize()
		{
			// Arrange
			var mockProducts = this.InitOpenCartProducts( ParamsBuilder.RequestMaxLimit + 1 );

			this._webRequestServiceMock.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					Arg.Any< string >(),
					Arg.Any< Mark >() )
				.Returns( new OpenCartProductsResponse { Products = mockProducts } );

			// Act
			var products = this._openCartProductsService.GetProductsAsync().GetAwaiter().GetResult();

			// Assert
			Assert.That( products, Is.EqualTo( mockProducts ) );
			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/1",
					Arg.Any< Mark >() );

			this._webRequestServiceMock
				.Received( 1 )
				.GetResponseAsync< OpenCartProductsResponse >(
					OpenCartCommand.GetProducts,
					$"limit/{ParamsBuilder.RequestMaxLimit}/page/2",
					Arg.Any< Mark >() );
		}

		private List< OpenCartProduct > InitOpenCartProducts( int count )
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