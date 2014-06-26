using System.Collections.Generic;
using System.Threading.Tasks;
using OpenCartAccess.Models.Product;

namespace OpenCartAccess
{
	public interface IOpenCartProductsService
	{
		IEnumerable< OpenCartProduct > GetProducts();
		Task< IEnumerable< OpenCartProduct > > GetProductsAsync();

		void UpdateProducts( IEnumerable< OpenCartProduct > products );
		Task UpdateProductsAsync( IEnumerable< OpenCartProduct > products );
	}
}