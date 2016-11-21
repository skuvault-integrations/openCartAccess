using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.ThrottlerServices;
using Netco.Utils;
using OpenCartAccess.Models;

namespace OpenCartAccess.Misc
{
	public static class ActionPolicies
	{
#if DEBUG
		private const int RetryCount = 1;
#else
		private const int RetryCount = 10;
#endif

		public static ActionPolicy GetPolicy( Mark mark, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicy.From( ex => !( ex is ThrottlerException ) ).Retry( RetryCount, ( ex, i ) =>
			{
				OpenCartLogger.Trace( ex, mark, "Retrying OpenCart API get call for the {0} time. Caller:{1}.", i, callerName );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}

		public static ActionPolicyAsync GetPolicyAsync( Mark mark, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicyAsync.From( ex => !( ex is ThrottlerException ) ).RetryAsync( RetryCount, async ( ex, i ) =>
			{
				OpenCartLogger.Trace( ex, mark, "Retrying OpenCart API get call for the {0} time. Caller:{1}.", i, callerName );
				await Task.Delay( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}

		public static ActionPolicy SubmitPolicy( Mark mark, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicy.From( ex => !( ex is ThrottlerException ) ).Retry( RetryCount, ( ex, i ) =>
			{
				OpenCartLogger.Trace( ex, mark, "Retrying OpenCart API submit call for the {0} time. Caller:{1}.", i, callerName );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}

		public static ActionPolicyAsync SubmitPolicyAsync( Mark mark, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicyAsync.From( ex => !( ex is ThrottlerException ) ).RetryAsync( RetryCount, async ( ex, i ) =>
			{
				OpenCartLogger.Trace( ex, mark, "OpenCart Shopify API submit call for the {0} time. Caller:{1}.", i, callerName );
				await Task.Delay( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}
	}
}