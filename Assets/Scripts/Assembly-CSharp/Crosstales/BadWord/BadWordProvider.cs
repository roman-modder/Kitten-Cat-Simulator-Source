using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Crosstales.BadWord
{
	public abstract class BadWordProvider : Provider
	{
		private Dictionary<string, Regex> exactBadwordsRegex = new Dictionary<string, Regex>();

		private Dictionary<string, Regex> fuzzyBadwordsRegex = new Dictionary<string, Regex>();

		private Dictionary<string, List<Regex>> debugExactBadwordsRegex = new Dictionary<string, List<Regex>>();

		private Dictionary<string, List<Regex>> debugFuzzyBadwordsRegex = new Dictionary<string, List<Regex>>();

		protected const string exactRegexStart = "(?<![\\w\\d])";

		protected const string exactRegexEnd = "s?(?![\\w\\d])";

		protected const string fuzzyRegexStart = "\\b\\w*";

		protected const string fuzzyRegexEnd = "\\w*\\b";

		protected List<BadWords> badwords = new List<BadWords>();

		public Dictionary<string, Regex> ExactBadwordsRegex
		{
			get
			{
				return exactBadwordsRegex;
			}
			protected set
			{
				exactBadwordsRegex = value;
			}
		}

		public Dictionary<string, Regex> FuzzyBadwordsRegex
		{
			get
			{
				return fuzzyBadwordsRegex;
			}
			protected set
			{
				fuzzyBadwordsRegex = value;
			}
		}

		public Dictionary<string, List<Regex>> DebugExactBadwordsRegex
		{
			get
			{
				return debugExactBadwordsRegex;
			}
			protected set
			{
				debugExactBadwordsRegex = value;
			}
		}

		public Dictionary<string, List<Regex>> DebugFuzzyBadwordsRegex
		{
			get
			{
				return debugFuzzyBadwordsRegex;
			}
			protected set
			{
				debugFuzzyBadwordsRegex = value;
			}
		}

		protected override void init()
		{
			ExactBadwordsRegex.Clear();
			FuzzyBadwordsRegex.Clear();
			base.Sources.Clear();
			if (Constants.DEBUG_BADWORDS)
			{
				Debug.Log("++ BadWordProvider '" + Name + "' started in debug-mode ++");
			}
			foreach (BadWords badword in badwords)
			{
				if (Constants.DEBUG_BADWORDS)
				{
					List<Regex> list = new List<Regex>(badword.BadWordList.Count);
					List<Regex> list2 = new List<Regex>(badword.BadWordList.Count);
					foreach (string badWord in badword.BadWordList)
					{
						list.Add(new Regex("(?<![\\w\\d])" + badWord + "s?(?![\\w\\d])", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
						list2.Add(new Regex("\\b\\w*" + badWord + "\\w*\\b", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
					}
					DebugExactBadwordsRegex.Add(badword.Source.Name, list);
					DebugFuzzyBadwordsRegex.Add(badword.Source.Name, list2);
				}
				else
				{
					ExactBadwordsRegex.Add(badword.Source.Name, new Regex("(?<![\\w\\d])(" + string.Join("|", badword.BadWordList.ToArray()) + ")s?(?![\\w\\d])", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
					FuzzyBadwordsRegex.Add(badword.Source.Name, new Regex("\\b\\w*(" + string.Join("|", badword.BadWordList.ToArray()) + ")\\w*\\b", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
				}
				base.Sources.Add(badword.Source);
				if (Constants.DEBUG_BADWORDS)
				{
					Debug.Log("Bad word resource '" + badword.Source.Name + "' loaded and " + badword.BadWordList.Count + " entries found.");
				}
			}
			base.Ready = true;
		}
	}
}
