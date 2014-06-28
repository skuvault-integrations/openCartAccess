using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
using NUnit.Framework;
using OpenCartAccess;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Product;

namespace OpenCartAccessTests.Products
{
	[ TestFixture ]
	internal class ProductsTests
	{
		private readonly IOpenCartFactory OpenCartFactory = new OpenCartFactory();
		private OpenCartConfig Config;

		[ SetUp ]
		public void Init()
		{
			const string credentialsFilePath = @"..\..\Files\OpenCartCredentials.csv";

			var cc = new CsvContext();
			var testConfig = cc.Read< TestConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();

			if( testConfig != null )
				this.Config = new OpenCartConfig( testConfig.ApiKey, testConfig.ShopUrl );
		}

		[ Test ]
		public void GetProducts()
		{
			var service = this.OpenCartFactory.CreateProductsService( this.Config );
			var products = service.GetProducts();

			products.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetProductsASync()
		{
			var service = this.OpenCartFactory.CreateProductsService( this.Config );
			var products = await service.GetProductsAsync();

			products.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void ProductOptionsQuantityUpdated()
		{
			var service = this.OpenCartFactory.CreateProductsService( this.Config );

			service.UpdateProducts( new List< OpenCartProduct >
			{
				new OpenCartProduct { Id = 47, Quantity = 44 },
				new OpenCartProduct { Id = 28, Quantity = 33 }
			} );
		}

		[ Test ]
		public async Task ProductOptionsQuantityUpdatedAsync()
		{
			var service = this.OpenCartFactory.CreateProductsService( this.Config );

			await service.UpdateProductsAsync( new List< OpenCartProduct >
			{
				new OpenCartProduct { Id = 47, Quantity = 44 },
				new OpenCartProduct { Id = 28, Quantity = 33 }
			} );
		}
	}
}