using System.Linq;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using OpenCartAccess.Misc;
using OpenCartAccess.Models.Checksum;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Services;

namespace OpenCartAccess
{
	public class OpenCartChecksumService : IOpenCartChecksumService
	{
		private readonly WebRequestServices _webRequestServices;

		public OpenCartChecksumService( OpenCartConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._webRequestServices = new WebRequestServices( config );
		}

		public bool CheckSumPresented()
		{
			var checksumsResponse = new OpenCartChecksumsResponse();
			ActionPolicies.OpenCartGetPolicy.Do( () =>
			{
				checksumsResponse = this._webRequestServices.GetResponse< OpenCartChecksumsResponse >( OpenCartCommand.GetChecksums, ParamsBuilder.EmptyParams ) ?? new OpenCartChecksumsResponse();
			} );

			return this.GetCheckResult( checksumsResponse );
		}

		public async Task< bool > CheckSumPresentedAsync()
		{
			var checksumsResponse = new OpenCartChecksumsResponse();
			await ActionPolicies.OpenCartGetPolicyAsync.Do( async () =>
			{
				checksumsResponse = await this._webRequestServices.GetResponseAsync< OpenCartChecksumsResponse >( OpenCartCommand.GetChecksums, ParamsBuilder.EmptyParams ) ?? new OpenCartChecksumsResponse();
			} );

			return this.GetCheckResult( checksumsResponse );
		}

		private bool GetCheckResult( OpenCartChecksumsResponse response )
		{
			return response.Checksums.Any( c => !string.IsNullOrWhiteSpace( c.Value ) );
		}
	}
}