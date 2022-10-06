using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
using Netco.Logging;
using NUnit.Framework;
using OpenCartAccess;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Product;

namespace OpenCartAccessTests.IntegrationTests.Products
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
			NetcoLogger.LoggerFactory = new ConsoleLoggerFactory();

			var cc = new CsvContext();
			var testConfig = cc.Read< TestConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();

			if( testConfig != null )
				this.Config = new OpenCartConfig( testConfig.ApiKey, testConfig.ShopUrl );
		}

		[ Test ]
		public void TryGetProducts()
		{
			var service = this.OpenCartFactory.CreateProductsService( this.Config );
			var products = service.TryGetProducts();

			products.Should().BeTrue();
		}

		[ Test ]
		public async Task TryGetProductsAsync()
		{
			var service = this.OpenCartFactory.CreateProductsService( this.Config );
			var products = await service.TryGetProductsAsync();

			products.Should().BeTrue();
		}

		[ Test ]
		public async Task GetProductsAsync()
		{
			var service = this.OpenCartFactory.CreateProductsService( this.Config );
			var products = (await service.GetProductsAsync()).ToList();

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