using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Indicates that the value of the result column must indicate a new decision tree
/// </summary>
[Serializable]
public class DecisionTreeOutcome
{
	public enum OutcomeType { Int, Text };
	public string name;
	public OutcomeType type = OutcomeType.Int;
	public int value;
	public string textValue;

	[HideInInspector]
	public TreeNode root;

	#region Cache
	private Dictionary<QueryCache, bool> cache = new Dictionary<QueryCache, bool>();

	public bool? CachedResolve (DecisionQuery query)
	{
		bool result;
		QueryCache item = new QueryCache(query);
		if (!cache.TryGetValue(item, out result))
			return null;
		return result;
	}

	public void Cache (DecisionQuery query, bool value)
	{
		QueryCache item = new QueryCache(query);
		if (cache.ContainsKey(item))
			cache[item] = value;
		else
		{
			cache.Add(item, value);
		}
	}
	#endregion

}


