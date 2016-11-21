using System;
using Netco.Logging;
using OpenCartAccess.Models;

namespace OpenCartAccess.Misc
{
	public class OpenCartLogger
	{
		public static ILogger Log{ get; private set; }

		static OpenCartLogger()
		{
			Log = NetcoLogger.GetLogger( "OpenCartLogger" );
		}

		public static void Trace( Mark mark, string format, params object[] args )
		{
			var markStr = string.Format( "[{0}]\t", mark );
			Log.Trace( markStr + format, args );
		}

		public static void Trace( Exception ex, Mark mark, string format, params object[] args )
		{
			var markStr = string.Format( "[{0}]\t", mark );
			Log.Trace( ex, markStr + format, args );
		}
	}
}