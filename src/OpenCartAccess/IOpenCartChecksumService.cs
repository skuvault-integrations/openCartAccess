using System.Collections.Generic;
using System.Threading.Tasks;
using OpenCartAccess.Models;
using OpenCartAccess.Models.Checksum;

namespace OpenCartAccess
{
	public interface IOpenCartChecksumService
	{
		bool TryGetCheckSums( Mark mark = null );
		Task< bool > TryGetCheckSumsAsync( Mark mark = null );

		IEnumerable< OpenCartChecksum > GetCheckSums( Mark mark = null );
		Task< IEnumerable< OpenCartChecksum > > GetCheckSumsAsync( Mark mark = null );
	}
}