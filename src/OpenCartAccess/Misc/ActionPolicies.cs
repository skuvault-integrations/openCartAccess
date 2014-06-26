using System;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Logging;
using Netco.Utils;

namespace OpenCartAccess.Misc
{
	public static class ActionPolicies
	{
		public static ActionPolicy OpenCartGetPolicy
		{
			get { return _openCartGetPolicy; }
		}

		private static readonly ActionPolicy _openCartGetPolicy = ActionPolicy.Handle< Exception >().Retry( 50, ( ex, i ) =>
		{
			typeof( ActionPolicies ).Log().Trace( ex, "Retrying OpenCart API get call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
		} );

		public static ActionPolicy OpenCartSubmitPolicy
		{
			get { return _openCartSumbitPolicy; }
		}

		private static readonly ActionPolicy _openCartSumbitPolicy = ActionPolicy.Handle< Exception >().Retry( 50, ( ex, i ) =>
		{
			typeof( ActionPolicies ).Log().Trace( ex, "Retrying OpenCart API submit call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
		} );

		public static ActionPolicyAsync QueryAsync
		{
			get { return _queryAsync; }
		}

		private static readonly ActionPolicyAsync _queryAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( 50, async ( ex, i ) =>
		{
			typeof( ActionPolicies ).Log().Trace( ex, "Retrying OpenCart API get call for the {0} time", i );
			await Task.Delay( TimeSpan.FromSeconds( 0.6 ) );
		} );
	}
}