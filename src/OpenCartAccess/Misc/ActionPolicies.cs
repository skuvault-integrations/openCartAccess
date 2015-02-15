using System;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
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
			OpenCartLogger.Log.Trace( ex, "Retrying OpenCart API get call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
		} );

		public static ActionPolicyAsync OpenCartGetPolicyAsync
		{
			get { return _openCartGetPolicyAsync; }
		}

		private static readonly ActionPolicyAsync _openCartGetPolicyAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( 50, async ( ex, i ) =>
		{
			OpenCartLogger.Log.Trace( ex, "Retrying OpenCart API get call for the {0} time", i );
			await Task.Delay( TimeSpan.FromSeconds( 0.6 ) );
		} );

		public static ActionPolicy OpenCartSubmitPolicy
		{
			get { return _openCartSumbitPolicy; }
		}

		private static readonly ActionPolicy _openCartSumbitPolicy = ActionPolicy.Handle< Exception >().Retry( 50, ( ex, i ) =>
		{
			OpenCartLogger.Log.Trace( ex, "Retrying OpenCart API submit call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
		} );

		public static ActionPolicyAsync OpenCartSubmitPolicyAsync
		{
			get { return _openCartSumbitPolicyAsync; }
		}

		private static readonly ActionPolicyAsync _openCartSumbitPolicyAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( 50, async ( ex, i ) =>
		{
			OpenCartLogger.Log.Trace( ex, "Retrying OpenCart API submit call for the {0} time", i );
			await Task.Delay( TimeSpan.FromSeconds( 0.6 ) );
		} );
	}
}