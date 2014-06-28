using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
using NUnit.Framework;
using OpenCartAccess;
using OpenCartAccess.Models.Configuration;

namespace OpenCartAccessTests.Orders
{
	[ TestFixture ]
	internal class OrderTests
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
		public void GetOrders()
		{
			var service = this.OpenCartFactory.CreateOrdersService( this.Config );
			var orders = service.GetOrders( DateTime.UtcNow.AddDays( -400 ), DateTime.UtcNow );

			orders.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetOrdersAsync()
		{
			var service = this.OpenCartFactory.CreateOrdersService( this.Config );
			var orders = await service.GetOrdersAsync( DateTime.UtcNow.AddDays( -400 ), DateTime.UtcNow );

			orders.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetOrdersByDateTimeRange()
		{
			var service = this.OpenCartFactory.CreateOrdersService( this.Config );
			var orders = service.GetOrders( DateTime.UtcNow.AddMinutes( -3 ), DateTime.UtcNow );

			orders.Count().Should().BeGreaterThan( 0 );
		}
	}
}