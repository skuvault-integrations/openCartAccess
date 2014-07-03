using System.Threading.Tasks;

namespace OpenCartAccess
{
	public interface IOpenCartChecksumService
	{
		bool CheckSumPresented();
		Task< bool > CheckSumPresentedAsync();
	}
}