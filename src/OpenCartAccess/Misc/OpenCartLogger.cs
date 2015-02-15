using Netco.Logging;

namespace OpenCartAccess.Misc
{
	public class OpenCartLogger
	{
		public static ILogger Log{ get; private set; }

		static OpenCartLogger()
		{
			Log = NetcoLogger.GetLogger( "OpenCartLogger" );
		}
	}
}