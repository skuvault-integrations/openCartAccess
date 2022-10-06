using System;
using System.Collections.Generic;
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
		private readonly IWebRequestServices _webRequestServices;

		internal OpenCartChecksumService( OpenCartConfig config, IWebRequestServices webRequestServices )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._webRequestServices = webRequestServices;
		}

		public bool TryGetCheckSums( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			try
			{
				var checksumsResponse = this._webRequestServices.GetResponse< OpenCartChecksumsResponse >( OpenCartCommand.GetChecksums, ParamsBuilder.EmptyParams, mark );
				return this.GetCheckResult( checksumsResponse );
			}
			catch( Exception )
			{
				return false;
			}
		}

		public async Task< bool > TryGetCheckSumsAsync( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			try
			{
				var checksumsResponse = await this._webRequestServices.GetResponseAsync< OpenCartChecksumsResponse >( OpenCartCommand.GetChecksums, ParamsBuilder.EmptyParams, mark );
				return this.GetCheckResult( checksumsResponse );
			}
			catch( Exception )
			{
				return false;
			}
		}

		public IEnumerable< OpenCartChecksum > GetCheckSums( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var checksumsResponse = ActionPolicies.GetPolicy( mark ).Get( () =>
					this._webRequestServices.GetResponse< OpenCartChecksumsResponse >( OpenCartCommand.GetChecksums, ParamsBuilder.EmptyParams, mark ) );

			return checksumsResponse.Checksums;
		}

		public async Task< IEnumerable< OpenCartChecksum > > GetCheckSumsAsync( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var checksumsResponse = await ActionPolicies.GetPolicyAsync( mark ).Get( async () =>
					await this._webRequestServices.GetResponseAsync< OpenCartChecksumsResponse >( OpenCartCommand.GetChecksums, ParamsBuilder.EmptyParams, mark ) );

			return checksumsResponse.Checksums;
		}

		private bool GetCheckResult( OpenCartChecksumsResponse response )
		{
			return response.Checksums.Any( c => !string.IsNullOrWhiteSpace( c.Value ) );
		}
	}
}