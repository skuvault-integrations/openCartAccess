using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
using Netco.Logging;
using NUnit.Framework;
using OpenCartAccess;
using OpenCartAccess.Models.Configuration;

namespace OpenCartAccessTests.Checksums
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
		public void GetChecksums()
		{
			var service = this.OpenCartFactory.CreateChecksumService( this.Config );
			var checkResult = service.CheckSumPresented();

			checkResult.ShouldBeEquivalentTo( true );
		}

		[ Test ]
		public async Task GetChecksumsAsync()
		{
			var service = this.OpenCartFactory.CreateChecksumService( this.Config );
			var checkResult = await service.CheckSumPresentedAsync();

			checkResult.ShouldBeEquivalentTo( true );
		}
	}
}