using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Crosstales.BadWord
{
	public abstract class DomainProvider : Provider
	{
		private Dictionary<string, Regex> domainsRegex = new Dictionary<string, Regex>();

		private Dictionary<string, List<Regex>> debugDomainsRegex = new Dictionary<string, List<Regex>>();

		private const string domainRegexStart = "\\b{0,1}((ht|f)tp(s?)\\:\\/\\/)?[\\w\\-\\.\\@]*[\\.]";

		private const string domainRegexEnd = "(:(0-9)*)?(\\/|\\b)([a-zA-Z0-9\\-\\.\\?\\!\\,\\=\\'\\/\\&\\%#_]*)?\\b";

		protected List<Domains> domains = new List<Domains>();

		public Dictionary<string, Regex> DomainsRegex
		{
			get
			{
				return domainsRegex;
			}
			protected set
			{
				domainsRegex = value;
			}
		}

		public Dictionary<string, List<Regex>> DebugDomainsRegex
		{
			get
			{
				return debugDomainsRegex;
			}
			protected set
			{
				debugDomainsRegex = value;
			}
		}

		protected override void init()
		{
			DomainsRegex.Clear();
			base.Sources.Clear();
			if (Constants.DEBUG_DOMAINS)
			{
				Debug.Log("++ DomainProvider '" + Name + "' started in debug-mode ++");
			}
			foreach (Domains domain in domains)
			{
				if (Constants.DEBUG_DOMAINS)
				{
					List<Regex> list = new List<Regex>(domain.DomainList.Count);
					foreach (string domain2 in domain.DomainList)
					{
						list.Add(new Regex("\\b{0,1}((ht|f)tp(s?)\\:\\/\\/)?[\\w\\-\\.\\@]*[\\.]" + domain2 + "(:(0-9)*)?(\\/|\\b)([a-zA-Z0-9\\-\\.\\?\\!\\,\\=\\'\\/\\&\\%#_]*)?\\b", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
					}
					DebugDomainsRegex.Add(domain.Source.Name, list);
				}
				else
				{
					DomainsRegex.Add(domain.Source.Name, new Regex("\\b{0,1}((ht|f)tp(s?)\\:\\/\\/)?[\\w\\-\\.\\@]*[\\.](" + string.Join("|", domain.DomainList.ToArray()) + ")(:(0-9)*)?(\\/|\\b)([a-zA-Z0-9\\-\\.\\?\\!\\,\\=\\'\\/\\&\\%#_]*)?\\b", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
				}
				base.Sources.Add(domain.Source);
				if (Constants.DEBUG_DOMAINS)
				{
					Debug.Log(string.Concat("Domain resource '", domain.Source, "' loaded and ", domain.DomainList.Count, " entries found."));
				}
			}
			base.Ready = true;
		}
	}
}
