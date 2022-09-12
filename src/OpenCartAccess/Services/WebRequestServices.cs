using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using OpenCartAccess.Misc;
using OpenCartAccess.Models;
using OpenCartAccess.Models.Configuration;
using ServiceStack;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] 
[assembly: InternalsVisibleTo("OpenCartAccessTests")]

namespace OpenCartAccess.Services
{
	internal interface IWebRequestServices
	{
		T GetResponse< T >( OpenCartCommand command, string commandParams, Mark mark ) where T : new();
		Task< T > GetResponseAsync< T >( OpenCartCommand command, string commandParams, Mark mark ) where T : new();
		T PutData< T >( OpenCartCommand command, string endpoint, string jsonContent, Mark mark ) where T : new();
		Task< T > PutDataAsync< T >( OpenCartCommand command, string endpoint, string jsonContent, Mark mark ) where T : new();
	}
	
	internal class WebRequestServices: IWebRequestServices
	{
		private readonly OpenCartConfig _config;

		public WebRequestServices( OpenCartConfig config )
		{
			this._config = config;
		}

		public T GetResponse< T >( OpenCartCommand command, string commandParams, Mark mark ) where T : new()
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateGetServiceGetRequest( string.Concat( this._config.ShopUrl, command.Command, commandParams ) );
			this.LogGetRequest( request.RequestUri, mark );

			return this.ParseException( mark, () =>
			{
				T result;
				using( var response = request.GetResponse() )
					result = this.ParseResponse< T >( response, mark );
				return result;
			} );
		}

		public async Task< T > GetResponseAsync< T >( OpenCartCommand command, string commandParams, Mark mark ) where T : new()
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateGetServiceGetRequest( string.Concat( this._config.ShopUrl, command.Command, commandParams ) );
			this.LogGetRequest( request.RequestUri, mark );

			return await this.ParseExceptionAsync( mark, async () =>
			{
				T result;
				using( var response = await request.GetResponseAsync() )
					result = this.ParseResponse< T >( response, mark );
				return result;
			} );
		}

		public T PutData< T >( OpenCartCommand command, string endpoint, string jsonContent, Mark mark ) where T : new()
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			this.LogUpdateRequest( request.RequestUri, jsonContent, mark );

			return this.ParseException( mark, () =>
			{
				T result;
				using( var response = request.GetResponse() )
					result = this.ParseResponse< T >( response, mark );
				return result;
			} );
		}

		public async Task< T > PutDataAsync< T >( OpenCartCommand command, string endpoint, string jsonContent, Mark mark ) where T : new()
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			this.LogUpdateRequest( request.RequestUri, jsonContent, mark );

			return await this.ParseExceptionAsync( mark, async () =>
			{
				T result;
				using( var response = await request.GetResponseAsync() )
					result = this.ParseResponse< T >( response, mark );
				return result;
			} );
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

		private void CreateRequestHeaders( HttpWebRequest request )
		{
			request.Headers.Add( "X-Oc-Merchant-Id", this._config.ApiKey );
			request.Headers.Add( "X-Oc-Merchant-Language", "en" );
		}
		#endregion

		#region Misc
		private T ParseResponse< T >( WebResponse response, Mark mark ) where T : new()
		{
			var httpResponse = ( HttpWebResponse )response;

			using( var stream = response.GetResponseStream() )
			using( var reader = new StreamReader( stream ) )
			{
				var jsonResponse = reader.ReadToEnd();
				if( jsonResponse.Contains( "Invalid secret key" ) )
					throw new WebException( jsonResponse, WebExceptionStatus.ProtocolError );

				this.LogResponse( httpResponse.Method, response.ResponseUri, jsonResponse, mark );

				if( jsonResponse.Contains( "},[],{" ) )
					jsonResponse = jsonResponse.Replace( "},[],{", "},{" );

				var result = !string.IsNullOrEmpty( jsonResponse ) ? jsonResponse.FromJson< T >() : new T();
				return result;
			}
		}

		private T ParseException< T >( Mark mark, Func< T > body )
		{
			try
			{
				return body();
			}
			catch( WebException ex )
			{
				throw this.HandleException( ex, mark );
			}
		}

		private async Task< T > ParseExceptionAsync< T >( Mark mark, Func< Task< T > > body )
		{
			try
			{
				return await body();
			}
			catch( WebException ex )
			{
				throw this.HandleException( ex, mark );
			}
		}

		private WebException HandleException( WebException ex, Mark mark )
		{
			if( ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError ||
			    ex.Response.ContentType == null /*|| ex.Response.ContentType.Contains( "text/html" )*/ )
			{
				this.LogException( ex, mark );
				return ex;
			}

			var httpResponse = ( HttpWebResponse )ex.Response;

			using( var stream = httpResponse.GetResponseStream() )
			using( var reader = new StreamReader( stream ) )
			{
				var jsonResponse = reader.ReadToEnd();
				this.LogException( ex, httpResponse, jsonResponse, mark );
				return ex;
			}
		}
		#endregion

		#region Logging
		private void LogGetRequest( Uri requestUri, Mark mark )
		{
			OpenCartLogger.Trace( mark, "GET request\tRequest: {0}", requestUri );
		}

		private void LogUpdateRequest( Uri requestUri, string jsonContent, Mark mark )
		{
			OpenCartLogger.Trace( mark, "PUT request\tRequest: {0}Data: {1}", requestUri, jsonContent );
		}

		private void LogResponse( string method, Uri requestUri, string jsonResponse, Mark mark )
		{
			OpenCartLogger.Trace( mark, "{0} response\tRequest: {1}\tResponse: {2}", method, requestUri, jsonResponse );
		}

		private void LogException( WebException ex, Mark mark )
		{
			OpenCartLogger.Trace( ex, mark, "Failed response\tMessage: {0}\tStatus: {1}", ex.Message, ex.Status );
		}

		private void LogException( WebException ex, HttpWebResponse response, string jsonResponse, Mark mark )
		{
			OpenCartLogger.Trace( ex, mark, "Failed response\tRequest: {0}\tMessage: {1}\tStatus: {2}\tJsonResponse: {3}",
				response.ResponseUri, ex.Message, response.StatusCode, jsonResponse );
		}
		#endregion
	}
}