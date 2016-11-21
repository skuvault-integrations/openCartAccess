using System.Threading.Tasks;
using OpenCartAccess.Models;

namespace OpenCartAccess
{
	public interface IOpenCartChecksumService
	{
		bool CheckSumPresented( Mark mark = null );
		Task< bool > CheckSumPresentedAsync( Mark mark = null );
	}
}