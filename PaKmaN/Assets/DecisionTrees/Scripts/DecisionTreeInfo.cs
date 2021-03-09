using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class DecisionTreeInfo
{
	public string decisionTreeName;
	public string tableName;
	public string predictColumn;
	public DecisionTreeOutcome[] outcomes;
	public AttributeInfo[] inputs;

	public AttributeInfo GetAttributeInfo (string name)
	{
		for(int a = 0; a < inputs.Length; a++)
		{
			if (inputs[a].name == name)
				return inputs[a];
		}
		return null;
	}


	public Attribute[] GetAttributes()
	{
		List<Attribute> result = new List<Attribute>();
		foreach(var info in inputs)
		{
			result.Add(new Attribute(info.name, info.values));

		}
		return result.ToArray();
	}

	public DecisionTreeOutcome GetOutcome(string name)
	{
		for(int i = 0; i < outcomes.Length; i++)
		{
			if (outcomes[i].name == name)
				return outcomes[i];
		}
		return null;
	}


}


