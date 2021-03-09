using System;
using UnityEngine;
using System.Collections.Generic;

public static class DecisionExtensions
{

	/// <summary>
	/// Computes the hash for an array of integers.
	/// </summary>
	/// <returns>The hash.</returns>
	/// <param name="values">Values.</param>
	public static int ComputeHash(this int[] values)
	{
		int result = 0;
		int shift = 0;
		for (int i = 0; i < values.Length; i++)
		{
			shift = (shift + 11) % 21;
			result ^= (values[i]+1024) << shift;
		}
		return result;
	}

	public static T[] WithElement<T>(this T[] array, T element)
	{
		if (array == null)
		{
			return array = new T[] { element };
		}

		int index = array.Length;
		Array.Resize(ref array, index+1);
		array[index] = element;
		return array;
	}

}

