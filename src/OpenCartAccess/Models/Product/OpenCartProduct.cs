using System.Runtime.Serialization;

namespace OpenCartAccess.Models.Product
{
	[ DataContract ]
	public sealed class OpenCartProduct
	{
		[ DataMember( Name = "id" ) ]
		public int Id { get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku { get; set; }

		[ DataMember( Name = "quantity" ) ]
		public int Quantity { get; set; }

		#region Equality members
		public bool Equals( OpenCartProduct other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return this.Id.Equals( other.Id ) &&
			       this.Sku.Equals( other.Sku ) &&
			       this.Quantity.Equals( other.Quantity );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return this.Equals( ( OpenCartProduct )obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = this.Id.GetHashCode();
				result = ( result * 397 ) ^ this.Sku.GetHashCode();
				result = ( result * 397 ) ^ this.Quantity.GetHashCode();

				return result;
			}
		}
		#endregion
	}
}