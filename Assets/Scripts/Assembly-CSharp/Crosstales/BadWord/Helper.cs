using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Crosstales.BadWord
{
	public static class Helper
	{
		private static readonly Regex lineEndingsRegex = new Regex("\\r\\n|\\r|\\n");

		private static System.Random rd = new System.Random();

		public static List<string> SplitStringToLines(string text, int skipHeaderLines = 0, int skipFooterLines = 0, char splitChar = '#')
		{
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogWarning("Parameter 'text' is null or empty!" + Environment.NewLine + "=> 'ReadTextLines()' will return an empty string for the given resource.");
			}
			else
			{
				string[] array = lineEndingsRegex.Split(text);
				for (int i = 0; i < array.Length; i++)
				{
					if (i + 1 > skipHeaderLines && i < array.Length - skipFooterLines && !string.IsNullOrEmpty(array[i]) && !array[i].StartsWith("#", StringComparison.OrdinalIgnoreCase))
					{
						list.Add(array[i].Split(splitChar)[0]);
					}
				}
			}
			return list;
		}

		public static string CreateReplaceString(string replaceChars, int stringLength)
		{
			if (replaceChars.Length > 1)
			{
				char[] array = new char[stringLength];
				for (int i = 0; i < stringLength; i++)
				{
					array[i] = replaceChars[rd.Next(0, replaceChars.Length)];
				}
				return new string(array);
			}
			if (replaceChars.Length == 1)
			{
				return new string(replaceChars[0], stringLength);
			}
			return string.Empty;
		}

		public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("Collection is null");
			}
			foreach (KeyValuePair<T, S> item in collection)
			{
				if (!source.ContainsKey(item.Key))
				{
					source.Add(item.Key, item.Value);
				}
				else
				{
					Debug.LogWarning("Duplicate key found: " + item.Key);
				}
			}
		}

		public static Color HSVToRGB(float h, float s, float v, float a = 1f)
		{
			if (s == 0f)
			{
				return new Color(v, v, v, a);
			}
			h /= 60f;
			int num = Mathf.FloorToInt(h);
			float num2 = h - (float)num;
			float num3 = v * (1f - s);
			float num4 = v * (1f - s * num2);
			float num5 = v * (1f - s * (1f - num2));
			switch (num)
			{
			case 0:
				return new Color(v, num5, num3, a);
			case 1:
				return new Color(num4, v, num3, a);
			case 2:
				return new Color(num3, v, num5, a);
			case 3:
				return new Color(num3, num4, v, a);
			case 4:
				return new Color(num5, num3, v, a);
			default:
				return new Color(v, num3, num4, a);
			}
		}
	}
}
