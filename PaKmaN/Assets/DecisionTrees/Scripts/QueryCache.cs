using System;
	public struct QueryCache: IEquatable<QueryCache>
	{
		public QueryCache(DecisionQuery query)
		{
			query.ComputeHash();
			if (query.valueIndices == null)
				throw new Exception ("WTF");
			valueIndices = query.valueIndices;
			hashCode = query.GetHashCode();
		}

		public int[] valueIndices;
		public int hashCode;

		#region Methods
		bool IEquatable<QueryCache>.Equals(QueryCache other)
		{
			if (this.valueIndices.Length != other.valueIndices.Length)
				return false;

			for(int s = 0; s < this.valueIndices.Length; s++)
			{
				if (this.valueIndices[s] != other.valueIndices[s])
					return false;
			}
			return true;
		}

		public override int GetHashCode ()
		{
			return hashCode;
		}
		
		public override bool Equals (object obj)
		{
			return ((IEquatable<QueryCache>)this).Equals((QueryCache)obj);
		}
		#endregion


	}

