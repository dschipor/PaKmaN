using System;
using System.Collections;
using System.Data;

public class Attribute
{
	ArrayList _values;
	String _name;
	object _value;

	public Attribute(String name, String[] values)
	{
		_name = name;
		_values = new ArrayList(values);
		_values.Sort();
	}
	
	public Attribute(object value)
	{
		_value = value;
		_name = String.Empty;
		_values = null;
	}
	
	public String AttributeName
	{
		get
		{
			return _name;
		}
	}
	
	
	public String[] values
	{
		get
		{
			if (_values != null)
				return (String[])_values.ToArray(typeof(String));
			else
				return null;
		}
	}
	
	public bool isValidValue(String value)
	{
		return indexValue(value) >= 0;
	}
	
	public int indexValue(String value)
	{
		if (_values != null)
			return _values.BinarySearch(value);
		else
			return -1;
	}
	
	public override String ToString()
	{
		if (!string.IsNullOrEmpty(_name))
		{
			return _name;
		}
		else
		{
			return _value.ToString();
		}
	}

	public bool IsValue
	{
		get { return string.IsNullOrEmpty(_name); }
	}

	public object Value
	{
		get 
		{
			return _value;
		}
	}
}


public class TreeNode
{
	private ArrayList mChildren = null;
	private Attribute mAttribute;
	
	 public TreeNode(Attribute attribute)
	{
		if (attribute.values != null)
		{
			mChildren = new ArrayList(attribute.values.Length);
			for (int i = 0; i < attribute.values.Length; i++)
				mChildren.Add(null);
		}
		else
		{
			mChildren = new ArrayList(1);
			mChildren.Add(null);
		}
		mAttribute = attribute;
	}
	
	public void AddTreeNode(TreeNode treeNode, String ValueName)
	{
		int index = mAttribute.indexValue(ValueName);
		mChildren[index] = treeNode;
	}
	
	public int childrenCount
	{
		get
		{
			return mChildren.Count;
		}
	}
	
	
	public TreeNode getChild(int index)
	{
		return (TreeNode)mChildren[index];
	}
	
	public Attribute attribute
	{
		get
		{
			return mAttribute;
		}
	}
	
	public TreeNode getChildByBranchName(String branchName)
	{
		int index = mAttribute.indexValue(branchName);
		return (TreeNode)mChildren[index];
	}

	public string PrintNode(String tabs = "o ")
	{
		String printout;
		var color = attribute.IsValue 
			? (attribute.Value is bool && (bool)attribute.Value ? "#53ff9d" : "#ff7200")
			: "#ffffff";
		
		printout =  string.Format("\n<color=#cccccc>{0}</color><color={1}><b>{2}</b></color>", tabs, color, attribute);
		
		if (attribute.values != null)
		{
			for (int i = 0; i < attribute.values.Length; i++)
			{
				var value = attribute.values[i];
				printout += "\t" + "<color=#05e7fa><b>" + value + "</b></color>";
				//printout += tabs + "\t" + "<" + attribute.values[i] + ">";
				TreeNode childNode = getChildByBranchName(attribute.values[i]);
				printout += childNode.PrintNode("\t" + tabs);
			}
		}
		return printout;
	}
}


public class DecisionTreeID3
{


	private DataTable mSamples;
	private int mTotalPositives = 0;
	private int mTotal = 0;
	private String mTargetAttribute = "result";
	private double mEntropySet = 0.0;
	
	private int CountTotalPositives(DataTable samples)
	{
		int result = 0;
		foreach (DataRow aRow in samples.Rows)
		{
			if ((bool)aRow[mTargetAttribute] == true)
				result++;
		}
		return result;
	}
	
	private double CalcEntropy(int positives, int negatives)
	{
		int total = positives + negatives;
		double ratioPositive = (double)positives/total;
		double ratioNegative = (double)negatives/total;
		if (ratioPositive != 0)
			ratioPositive = -(ratioPositive) * System.Math.Log(ratioPositive, 2);
		if (ratioNegative != 0)
			ratioNegative = - (ratioNegative) * System.Math.Log(ratioNegative, 2);
		double result =  ratioPositive + ratioNegative;
		return result;
	}
	
	private void GetValuesToAttribute(DataTable samples, Attribute attribute, String value, out int positives, out int negatives)
	{
		positives = 0;
		negatives = 0;
		foreach (DataRow aRow in samples.Rows)
		{
			if (  ((String)aRow[attribute.AttributeName] == value) )
				if ( (bool)aRow[mTargetAttribute] == true) 
					positives++;
			else
				negatives++;
		}		
	}
	
	
	private double Gain(DataTable samples, Attribute attribute)
	{
		String[] values = attribute.values;
		double sum = 0.0;
		for (int i = 0; i < values.Length; i++)
		{
			int positives, negatives;
			
			positives = negatives = 0;
			
			GetValuesToAttribute(samples, attribute, values[i], out positives, out negatives);
			
			double entropy = CalcEntropy(positives, negatives);				
			sum += -(double)(positives + negatives)/mTotal * entropy;
		}
		return mEntropySet + sum;
	}
	
	private Attribute GetBestAttribute(DataTable samples, Attribute[] attributes)
	{
		double maxGain = double.MinValue;
		Attribute result = null;
		
		foreach (Attribute attribute in attributes)
		{
			double aux = Gain(samples, attribute);
			if (aux > maxGain)
			{
				maxGain = aux;
				result = attribute;
			}
		}
		return result;
	}
	
	private bool AllSamplesPositives(DataTable samples, String targetAttribute)
	{			
		foreach (DataRow row in samples.Rows)
		{
			if ( (bool)row[targetAttribute] == false)
				return false;
		}
		
		return true;
	}
	
	
	private bool AllSamplesNegatives(DataTable samples, String targetAttribute)
	{
		foreach (DataRow row in samples.Rows)
		{
			if ( (bool)row[targetAttribute] == true)
				return false;
		}
		
		return true;			
	}
	
	private ArrayList GetDistinctValues(DataTable samples, String targetAttribute)
	{
		ArrayList distinctValues = new ArrayList(samples.Rows.Count);
		foreach(DataRow row in samples.Rows)
		{
			if (distinctValues.IndexOf(row[targetAttribute]) == -1)
				distinctValues.Add(row[targetAttribute]);
		}
		
		return distinctValues;
	}
	
	
	private object GetMostCommonValue(DataTable samples, String targetAttribute)
	{
		ArrayList distinctValues = GetDistinctValues(samples, targetAttribute);
		int[] count = new int[distinctValues.Count];
		foreach(DataRow row in samples.Rows)
		{
			int index = distinctValues.IndexOf(row[targetAttribute]);
			count[index]++;
		}
		
		int MaxIndex = 0;
		int MaxCount = 0;
		
		for (int i = 0; i < count.Length; i++)
		{
			if (count[i] > MaxCount)
			{
				MaxCount = count[i];
				MaxIndex = i;
			}
		}
		
		return distinctValues[MaxIndex];
	}
	
	
	private TreeNode InternalMountTree(DataTable samples, String targetAttribute, Attribute[] attributes)
	{
		if (AllSamplesPositives(samples, targetAttribute) == true)
			return new TreeNode(new Attribute(true));
		
		if (AllSamplesNegatives(samples, targetAttribute) == true)
			return new TreeNode(new Attribute(false));
		
		if (attributes.Length == 0)
			return new TreeNode(new Attribute(GetMostCommonValue(samples, targetAttribute)));			
		
		mTotal = samples.Rows.Count;
		mTargetAttribute = targetAttribute;
		mTotalPositives = CountTotalPositives(samples);
		mEntropySet = CalcEntropy(mTotalPositives, mTotal - mTotalPositives);
		
		Attribute bestAttribute = GetBestAttribute(samples, attributes); 
		if (bestAttribute == null)
			return new TreeNode(new Attribute(false));
	
		TreeNode root = new TreeNode(bestAttribute);
		
		DataTable aSample = samples.Clone();			
		
		foreach(String value in bestAttribute.values)
		{				
			
			aSample.Rows.Clear();
			
			DataRow[] rows = samples.Select(bestAttribute.AttributeName + " = " + "'"  + value + "'");
			
			foreach(DataRow row in rows)
			{					
				aSample.Rows.Add(row.ItemArray);
			}				
			
			ArrayList aAttributes = new ArrayList(attributes.Length - 1);
			for(int i = 0; i < attributes.Length; i++)
			{
				if (attributes[i].AttributeName != bestAttribute.AttributeName)
					aAttributes.Add(attributes[i]);
			}
			
			if (aSample.Rows.Count == 0)
			{
				return new TreeNode(new Attribute(GetMostCommonValue(aSample, targetAttribute)));
			}
			else
			{				
				DecisionTreeID3 dc3 = new DecisionTreeID3();
				TreeNode ChildNode =  dc3.MountTree(aSample, targetAttribute, (Attribute[])aAttributes.ToArray(typeof(Attribute)));
				root.AddTreeNode(ChildNode, value);
			}
		}
		return root;
	}
	
	
	public TreeNode MountTree(DataTable samples, String targetAttribute, Attribute[] attributes)
	{
		mSamples = samples;
		return InternalMountTree(mSamples, targetAttribute, attributes);
	}
	


	public TreeNode Resolve(TreeNode node, string targetAttribute, params AttributeValue[] values)
	{
		for(int i = 0; i < values.Length; i++)
		{
			var value = values[i];
			if (node.attribute.AttributeName != value.name)
				continue;

			var child = node.getChildByBranchName ( value.value );
			if (child != null)
			{
				return Resolve(child, targetAttribute, values);
			}
		}
		return node;
	}

	public string PrintResolve (TreeNode node, string targetAttribute, string tabs, AttributeValue[] values, string resolved = "")
	{
		var color = node.attribute.IsValue 
			? (node.attribute.Value is bool && (bool)node.attribute.Value ? "#53ff9d" : "#ff7200")
			: "#ffffff";
		resolved =  tabs + string.Format("<color={0}><b>", color) + node.attribute + "</b></color>";
		for(int i = 0; i < values.Length; i++)
		{
			var value = values[i];
			if (node.attribute.AttributeName != value.name)
				continue;
			
			var child = node.getChildByBranchName ( value.value );
			if (child != null)
			{
				resolved += "<color=#cccccc>" + "</color>" + "\t" + "<color=#05e7fa><b>" + value.value + "</b></color>\n";
				// Print sub query
				if (value.basedOn != null && value.basedOn.root != null)
				{
					string currentTabs = tabs;
					resolved += currentTabs + "\n{\n";
					resolved += PrintResolve(value.basedOn.root, value.basedOn.info.predictColumn, currentTabs, value.basedOn.predicate);
					resolved += currentTabs + "\n}\n";
				}
				resolved += PrintResolve(child, targetAttribute, "\t" + tabs, values, resolved);
			}
		}
		return resolved;
	}
}
