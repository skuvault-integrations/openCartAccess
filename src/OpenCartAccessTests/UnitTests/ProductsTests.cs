using NUnit.Framework;
using OpenCartAccess.Models;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Product;
using OpenCartAccess.Services;

namespace OpenCartAccessTests.UnitTests
{
	[ TestFixture ]
	public class ProductsTests
	{
		//private IWebRequestServices _webRequestServiceMock = Substitute.For< IWebRequestServices >();

		[ SetUp ]
		public void Init()
		{
		}

		[ Test ]
		public void GetProductsAsync()
		{
			// this._webRequestServiceMock.GetResponseAsync<OpenCartProductsResponse>( OpenCartCommand.GetProducts, Arg.Any<string>(), Arg.Any<Mark>() )
			// 	.Returns(  );
		}
	}
}