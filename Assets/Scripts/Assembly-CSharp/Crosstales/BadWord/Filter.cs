using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crosstales.BadWord
{
	public abstract class Filter
	{
		public string MarkPrefix = "<color=red>";

		public string MarkPostfix = "</color>";

		protected Dictionary<string, Source> sources = new Dictionary<string, Source>();

		public abstract bool Ready();

		public abstract bool Contains(string testString, params string[] sources);

		public abstract List<string> GetAll(string testString, params string[] sources);

		public abstract string ReplaceAll(string testString, params string[] sources);

		public abstract string Replace(string text, List<string> badWords);

		public virtual List<Source> Sources()
		{
			List<Source> result = new List<Source>();
			if (Ready())
			{
				result = (from y in sources
					orderby y.Key
					select y.Value).ToList();
			}
			else
			{
				logFilterNotReady();
			}
			return result;
		}

		public virtual string Mark(string text, List<string> badWords, string prefix = "", string postfix = "")
		{
			string text2 = text;
			string text3 = prefix;
			string text4 = postfix;
			if (string.IsNullOrEmpty(text))
			{
				text2 = string.Empty;
			}
			else
			{
				if (string.IsNullOrEmpty(prefix))
				{
					text3 = MarkPrefix ?? string.Empty;
				}
				if (string.IsNullOrEmpty(postfix))
				{
					text4 = MarkPostfix ?? string.Empty;
				}
				if (badWords != null && badWords.Count != 0)
				{
					foreach (string badWord in badWords)
					{
						text2 = text2.Replace(badWord, text3 + badWord + text4);
					}
				}
			}
			return text2;
		}

		public virtual string Unmark(string text, string prefix = "", string postfix = "")
		{
			string text2 = text;
			string oldValue = prefix;
			string oldValue2 = postfix;
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(prefix))
			{
				oldValue = MarkPrefix ?? string.Empty;
			}
			if (string.IsNullOrEmpty(postfix))
			{
				oldValue2 = MarkPostfix ?? string.Empty;
			}
			text2 = text2.Replace(oldValue, string.Empty);
			return text2.Replace(oldValue2, string.Empty);
		}

		protected void logFilterNotReady()
		{
			Debug.LogWarning("Filter is not ready - please wait until the 'Ready()' function returns true.");
		}

		protected void logResourceNotFound(string res)
		{
		}

		protected void logContains()
		{
		}

		protected void logGetAll()
		{
		}

		protected void logReplaceAll()
		{
		}
	}
}
