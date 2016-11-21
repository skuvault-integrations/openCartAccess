using System.Linq;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using OpenCartAccess.Misc;
using OpenCartAccess.Models;
using OpenCartAccess.Models.Checksum;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Services;

namespace OpenCartAccess
{
	public class OpenCartChecksumService: IOpenCartChecksumService
	{
		private readonly WebRequestServices _webRequestServices;

		public OpenCartChecksumService( OpenCartConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._webRequestServices = new WebRequestServices( config );
		}

		public bool CheckSumPresented( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var checksumsResponse = ActionPolicies.GetPolicy( mark ).Get( () =>
					this._webRequestServices.GetResponse< OpenCartChecksumsResponse >( OpenCartCommand.GetChecksums, ParamsBuilder.EmptyParams, mark ) );

			return this.GetCheckResult( checksumsResponse ?? new OpenCartChecksumsResponse() );
		}

		public async Task< bool > CheckSumPresentedAsync( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var checksumsResponse = await ActionPolicies.GetPolicyAsync( mark ).Get( async () =>
					await this._webRequestServices.GetResponseAsync< OpenCartChecksumsResponse >( OpenCartCommand.GetChecksums, ParamsBuilder.EmptyParams, mark ) );

			return this.GetCheckResult( checksumsResponse ?? new OpenCartChecksumsResponse() );
		}

		private bool GetCheckResult( OpenCartChecksumsResponse response )
		{
			return response.Checksums.Any( c => !string.IsNullOrWhiteSpace( c.Value ) );
		}
	}
}