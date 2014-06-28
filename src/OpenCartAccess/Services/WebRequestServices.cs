using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Netco.Logging;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Product;
using ServiceStack;

namespace OpenCartAccess.Services
{
	internal class WebRequestServices
	{
		private readonly OpenCartConfig _config;

		public WebRequestServices( OpenCartConfig config )
		{
			this._config = config;
		}

		public T GetResponse< T >( OpenCartCommand command, string commandParams )
		{
			T result;
			var request = this.CreateGetServiceGetRequest( string.Concat( this._config.ShopUrl, command.Command, commandParams ) );
			using( var response = request.GetResponse() )
				result = ParseResponse< T >( response );

			return result;
		}

		public async Task< T > GetResponseAsync< T >( OpenCartCommand command, string commandParams )
		{
			T result;
			var request = this.CreateGetServiceGetRequest( string.Concat( this._config.ShopUrl, command.Command, commandParams ) );
			using( var response = await request.GetResponseAsync() )
				result = ParseResponse< T >( response );

			return result;
		}

		public void PutData( OpenCartCommand command, string endpoint, string jsonContent )
		{
			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			using( var response = ( HttpWebResponse )request.GetResponse() )
			{
				var result = ParseResponse< OpenCartProductsResponse >( response );
				this.LogUpdateInfo( endpoint, response.StatusCode, jsonContent, result.Status, result.Error );
			}
		}

		public async Task PutDataAsync( OpenCartCommand command, string endpoint, string jsonContent )
		{
			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			using( var response = ( HttpWebResponse )await request.GetResponseAsync() )
			{
				var result = ParseResponse< OpenCartProductsResponse >( response );
				this.LogUpdateInfo( endpoint, response.StatusCode, jsonContent, result.Status, result.Error );
			}
		}

		#region WebRequest configuration
		private HttpWebRequest CreateGetServiceGetRequest( string url )
		{
			var uri = new Uri( url );
			var request = ( HttpWebRequest )WebRequest.Create( uri );

			request.Method = WebRequestMethods.Http.Get;
			this.CreateRequestHeaders( request );

			return request;
		}

		private HttpWebRequest CreateServicePutRequest( OpenCartCommand command, string endpoint, string content )
		{
			var uri = new Uri( string.Concat( this._config.ShopUrl, command.Command, endpoint ) );
			var request = ( HttpWebRequest )WebRequest.Create( uri );

			request.Method = WebRequestMethods.Http.Put;
			request.ContentType = "application/json";
			this.CreateRequestHeaders( request );

			using( var writer = new StreamWriter( request.GetRequestStream() ) )
				writer.Write( content );

			return request;
		}
		#endregion

		#region Misc
		private void CreateRequestHeaders( HttpWebRequest request )
		{
			request.Headers.Add( "X-Oc-Merchant-Id", this._config.ApiKey );
			request.Headers.Add( "X-Oc-Merchant-Language", "en" );
		}

		private T ParseResponse< T >( WebResponse response )
		{
			var result = default( T );

			using( var stream = response.GetResponseStream() )
			{
				var reader = new StreamReader( stream );
				var jsonResponse = reader.ReadToEnd();

				this.Log().Trace( "[opencart]\tResponse\t{0} - {1}", response.ResponseUri, jsonResponse );

				if( !string.IsNullOrEmpty( jsonResponse ) )
					result = jsonResponse.FromJson< T >();
			}

			return result;
		}

		private void LogUpdateInfo( string url, HttpStatusCode statusCode, string jsonContent, string requestStatus, string requestResult )
		{
			this.Log().Trace( "[opencart]\tPUT/POST call for the url '{0}' has been completed with code '{1}'.\n{2}-{3}\n{4}", url, statusCode, jsonContent, requestStatus, requestResult );
		}
		#endregion
	}
}