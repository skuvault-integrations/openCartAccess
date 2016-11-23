using System.Collections.Generic;
using System.Threading.Tasks;
using OpenCartAccess.Models;
using OpenCartAccess.Models.Product;

namespace OpenCartAccess
{
	public interface IOpenCartProductsService
	{
		bool TryGetProducts( Mark mark = null );
		Task< bool > TryGetProductsAsync( Mark mark = null );

		IEnumerable< OpenCartProduct > GetProducts( Mark mark = null );
		Task< IEnumerable< OpenCartProduct > > GetProductsAsync( Mark mark = null );

		void UpdateProducts( IEnumerable< OpenCartProduct > products, Mark mark = null );
		Task UpdateProductsAsync( IEnumerable< OpenCartProduct > products, Mark mark = null );
	}
}