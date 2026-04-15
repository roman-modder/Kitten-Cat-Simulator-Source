using System.Collections.Generic;
using System.Linq;

namespace Crosstales.BadWord
{
	public static class MultiManager
	{
		public static Filter Filter(ManagerMask mask = ManagerMask.BadWord)
		{
			Filter result = BadWordManager.Filter();
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain)
			{
				result = DomainManager.Filter();
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization)
			{
				result = CapitalizationManager.Filter();
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation)
			{
				result = PunctuationManager.Filter();
			}
			return result;
		}

		public static bool Ready()
		{
			return BadWordManager.Ready() && DomainManager.Ready() && CapitalizationManager.Ready() && PunctuationManager.Ready();
		}

		public static List<Source> Sources(ManagerMask mask = ManagerMask.All)
		{
			List<Source> list = new List<Source>();
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(BadWordManager.Sources());
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(DomainManager.Sources());
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(CapitalizationManager.Sources());
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(PunctuationManager.Sources());
			}
			return list.OrderBy((Source x) => x.Name).ToList();
		}

		public static bool Contains(string testString, ManagerMask mask = ManagerMask.All, params string[] sources)
		{
			return (((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All) && BadWordManager.Contains(testString, sources)) || (((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All) && DomainManager.Contains(testString, sources)) || (((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All) && CapitalizationManager.Contains(testString)) || (((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All) && PunctuationManager.Contains(testString));
		}

		public static List<string> GetAll(string testString, ManagerMask mask = ManagerMask.All, params string[] sources)
		{
			List<string> list = new List<string>();
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(BadWordManager.GetAll(testString, sources));
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(DomainManager.GetAll(testString, sources));
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(CapitalizationManager.GetAll(testString, sources));
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(PunctuationManager.GetAll(testString, sources));
			}
			return (from x in list.Distinct()
				orderby x
				select x).ToList();
		}

		public static string ReplaceAll(string testString, ManagerMask mask = ManagerMask.All, params string[] sources)
		{
			string text = testString ?? string.Empty;
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text = BadWordManager.ReplaceAll(text, sources);
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text = DomainManager.ReplaceAll(text, sources);
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text = CapitalizationManager.ReplaceAll(text, sources);
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text = PunctuationManager.ReplaceAll(text, sources);
			}
			return text;
		}

		public static string Replace(string text, List<string> badWords, ManagerMask mask = ManagerMask.All)
		{
			string text2 = text ?? string.Empty;
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text2 = BadWordManager.Replace(text2, badWords);
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text2 = DomainManager.Replace(text2, badWords);
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text2 = CapitalizationManager.Replace(text2, badWords);
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text2 = PunctuationManager.Replace(text2, badWords);
			}
			return text2;
		}

		public static string Mark(string text, List<string> badWords, string prefix = "", string postfix = "")
		{
			string text2 = text ?? string.Empty;
			return BadWordManager.Mark(text2, badWords, prefix, postfix);
		}

		public static string Unmark(string text, string prefix = "", string postfix = "")
		{
			string text2 = text ?? string.Empty;
			return BadWordManager.Unmark(text2, prefix, postfix);
		}
	}
}
