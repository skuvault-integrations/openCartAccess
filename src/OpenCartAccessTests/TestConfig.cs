using LINQtoCSV;

namespace OpenCartAccessTests
{
	internal class TestConfig
	{
		[ CsvColumn( Name = "ShopUrl", FieldIndex = 1 ) ]
		public string ShopUrl { get; set; }

		[ CsvColumn( Name = "ApiKey", FieldIndex = 2 ) ]
		public string ApiKey { get; set; }
	}
}