using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Represents a decision query.
/// </summary>
[Serializable]
public class DecisionQuery: IEquatable<DecisionQuery>
{
	#region Constuction
	public DecisionQuery()
	{
	}

	public DecisionQuery(string decisionTreeName, string logicalSplitName, params AttributeValue[] predicate)
	{
		Set(decisionTreeName, logicalSplitName, predicate);
	}

	public void Set(string decisionTreeName, string outcome, params AttributeValue[] predicate)
	{
		this.decisionTreeName = decisionTreeName;
		this.outcomeName = outcome;
		this.predicate = predicate;
		this._hash = 0;
		this.valueIndices = null;
	}
	#endregion

	#region Fields
	public string decisionTreeName;
	public string outcomeName;
	public AttributeValue[] predicate;

	[HideInInspector]
	public bool? result;

	[HideInInspector]
	[NonSerialized]
	public TreeNode root;

	[HideInInspector]
	[NonSerialized]
	public DecisionTreeInfo info;

	[HideInInspector]
	public int[] valueIndices;

	private int _hash;
	#endregion

	#region Methods
	bool IEquatable<DecisionQuery>.Equals(DecisionQuery other)
	{
		for(int s = 0; s < this.valueIndices.Length; s++)
		{
			for(int d = 0; d < other.valueIndices.Length; d++)
			{
				if (this.valueIndices[s] != other.valueIndices[d])
					return false;
			}
		}
		return true;
	}

	public override int GetHashCode ()
	{
		if (_hash == 0)
			ComputeHash();

		return _hash;
	}

	public override bool Equals (object obj)
	{
		if (!(obj is DecisionQuery))
			return false;

		if (valueIndices == null)
		{
			ComputeHash ();
		}

		return ((IEquatable<DecisionQuery>)this).Equals((DecisionQuery)obj);
	}

	public void ComputeHash ()
	{
		if (this.info == null 
		    || this.predicate == null 
		    || this.predicate.Length == 0)
		{
			Hawk.LogError("Decision query is in an invalid state and the hash cannot be computed.");
			return;
		}
		List<int> result = new List<int>();
		for(int i = 0; i < info.inputs.Length; i++)
		{
			var atr = info.inputs[i];
			var value = this[atr.name];
			if (value == null)
			{
				Hawk.LogError("Incomplete Decision Tree, no predicate for {0}!", atr.name); 
				return;
			}

			bool found = false;
			for(int x = 0; x < atr.values.Length; x++)
			{
				if (atr.values[x] == value)
				{
					result.Add(x);
					found = true;
					break;
				}
			}
			if (!found)
			{
				Hawk.LogError("Attribute {0} has an invalid value! Value: '{1}'", atr.name, value); 
				return;
			}
		}
		valueIndices = result.ToArray();
		this._hash = valueIndices.ComputeHash();
	}

	public bool Yes 
	{
		get { return result != null && result.Value; }
	}

	public string this[string name]
	{
		get 
		{
			for(int i = 0; i < this.predicate.Length; i++)
			{
				if (predicate[i].name == name)
				{
					return predicate[i].value;
				}
			}
			return null;
		}
		set
		{
			for(int i = 0; i < this.predicate.Length; i++)
			{
				if (predicate[i].name == name)
				{
					predicate[i].value = value;
					return;
				}
			}
			var val = new AttributeValue(name, value, null);
			predicate = predicate.WithElement(val);
		}
	}

	#endregion
}

