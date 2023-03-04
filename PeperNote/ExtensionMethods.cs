namespace PeperNote;
using System;

public static class ExtensionMethods
{
	public static T CastOrDefault<T>(this object val, T defaultValue)
	{
		return val is T tmp ? tmp : defaultValue;
	}

	public static T Clip<T>(this T value, T minValue, T maxValue) where T : IComparable<T>
	{
		T result;
		if(value.CompareTo(minValue) < 0)
			result = minValue;
		else if(value.CompareTo(maxValue) > 0)
			result = maxValue;
		else
			result = value;
		return result;
	}
}
