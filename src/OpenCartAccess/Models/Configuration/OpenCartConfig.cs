using CuttingEdge.Conditions;

namespace OpenCartAccess.Models.Configuration
{
	public class OpenCartConfig
	{
		public string ApiKey { get; private set; }
		public string ShopUrl { get; private set; }

		public OpenCartConfig( string apiKey, string shopUrl )
		{
			Condition.Requires( apiKey, "apiKey" ).IsNotNullOrWhiteSpace();
			Condition.Requires( shopUrl, "shopUrl" ).IsNotNullOrWhiteSpace();

			this.ApiKey = apiKey;
			this.ShopUrl = shopUrl;
		}
	}
}