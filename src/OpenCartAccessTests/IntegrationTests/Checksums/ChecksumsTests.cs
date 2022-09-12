using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
using Netco.Logging;
using NUnit.Framework;
using OpenCartAccess;
using OpenCartAccess.Models.Configuration;

namespace OpenCartAccessTests.IntegrationTests.Checksums
{
	[ TestFixture ]
	internal class ChecksumsTests
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
		public void TryGetCheckSums()
		{
			var service = this.OpenCartFactory.CreateChecksumService( this.Config );
			var checkResult = service.TryGetCheckSums();

			checkResult.ShouldBeEquivalentTo( true );
		}

		[ Test ]
		public async Task TryGetCheckSumsAsync()
		{
			var service = this.OpenCartFactory.CreateChecksumService( this.Config );
			var checkResult = await service.TryGetCheckSumsAsync();

			checkResult.ShouldBeEquivalentTo( true );
		}

		[ Test ]
		public void GetCheckSums()
		{
			var service = this.OpenCartFactory.CreateChecksumService( this.Config );
			var checkResult = service.GetCheckSums();

			checkResult.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetCheckSumsAsync()
		{
			var service = this.OpenCartFactory.CreateChecksumService( this.Config );
			var checkResult = await service.GetCheckSumsAsync();

			checkResult.Count().Should().BeGreaterThan( 0 );
		}
	}
}