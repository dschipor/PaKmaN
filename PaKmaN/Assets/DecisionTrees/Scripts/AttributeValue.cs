using System;

[Serializable]
public class AttributeValue
{
	public AttributeValue(string name, string value, DecisionQuery basedOn = null)
	{
		this.name = name;
		this.value = value;
		this.basedOn = basedOn;
	}

	public AttributeValue()
	{
	}

	public string name;

	public string value;

	// The decision that was used to set this value.... yes - mind blowing
	public DecisionQuery basedOn;
}

