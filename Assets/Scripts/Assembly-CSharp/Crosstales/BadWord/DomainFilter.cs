using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Crosstales.BadWord
{
	public class DomainFilter : Filter
	{
		private List<DomainProvider> domainProvider;

		public string ReplaceCharacters;

		private List<DomainProvider> tempDomainProvider;

		private Dictionary<string, Regex> domainsRegex = new Dictionary<string, Regex>();

		private Dictionary<string, List<Regex>> debugDomainsRegex = new Dictionary<string, List<Regex>>();

		private bool isReady;

		private bool isReadyFirstime;

		public List<DomainProvider> DomainProvider
		{
			get
			{
				return domainProvider;
			}
			set
			{
				domainProvider = value;
				if (domainProvider != null && domainProvider.Count > 0)
				{
					foreach (DomainProvider item in domainProvider)
					{
						if (item != null)
						{
							if (Constants.DEBUG_DOMAINS)
							{
								debugDomainsRegex.AddRange(item.DebugDomainsRegex);
							}
							else
							{
								domainsRegex.AddRange(item.DomainsRegex);
							}
						}
						else
						{
							Debug.LogError("DomainProvider is null!");
						}
					}
					return;
				}
				Debug.LogWarning("No 'DomainProvider' added!" + Environment.NewLine + "If you want to use this functionality, please add your desired 'DomainProvider' in the editor or script.");
			}
		}

		public DomainFilter(List<DomainProvider> domainProvider, string replaceCharacters, string markPrefix, string markPostfix)
		{
			tempDomainProvider = domainProvider;
			ReplaceCharacters = replaceCharacters;
			MarkPrefix = markPrefix;
			MarkPostfix = markPostfix;
		}

		public override bool Ready()
		{
			bool flag = true;
			if (!isReady)
			{
				foreach (DomainProvider item in tempDomainProvider)
				{
					if (item != null && !item.Ready)
					{
						flag = false;
						break;
					}
				}
				if (!isReadyFirstime && flag)
				{
					DomainProvider = tempDomainProvider;
					foreach (DomainProvider item2 in DomainProvider)
					{
						if (!(item2 != null))
						{
							continue;
						}
						foreach (Source source in item2.Sources)
						{
							sources.Add(source.Name, source);
						}
					}
					isReadyFirstime = true;
				}
			}
			isReady = flag;
			return flag;
		}

		public override bool Contains(string testString, params string[] sources)
		{
			bool result = false;
			if (Ready())
			{
				if (string.IsNullOrEmpty(testString))
				{
					logContains();
				}
				else if (Constants.DEBUG_DOMAINS)
				{
					if (sources == null || sources.Length == 0)
					{
						foreach (List<Regex> value3 in debugDomainsRegex.Values)
						{
							foreach (Regex item in value3)
							{
								Match match = item.Match(testString);
								if (match.Success)
								{
									Debug.Log(string.Concat("Test string contains a domain: '", match.Value, "' detected by regex '", item, "'"));
									result = true;
									break;
								}
							}
						}
					}
					else
					{
						foreach (string text in sources)
						{
							List<Regex> value;
							if (debugDomainsRegex.TryGetValue(text, out value))
							{
								foreach (Regex item2 in value)
								{
									Match match2 = item2.Match(testString);
									if (match2.Success)
									{
										Debug.Log(string.Concat("Test string contains a domain: '", match2.Value, "' detected by regex '", item2, "'' from source '", text, "'"));
										result = true;
										break;
									}
								}
							}
							else
							{
								logResourceNotFound(text);
							}
						}
					}
				}
				else if (sources == null || sources.Length == 0)
				{
					foreach (Regex value4 in domainsRegex.Values)
					{
						if (value4.Match(testString).Success)
						{
							result = true;
							break;
						}
					}
				}
				else
				{
					foreach (string text2 in sources)
					{
						Regex value2;
						if (domainsRegex.TryGetValue(text2, out value2))
						{
							Match match3 = value2.Match(testString);
							if (match3.Success)
							{
								result = true;
								break;
							}
						}
						else
						{
							logResourceNotFound(text2);
						}
					}
				}
			}
			else
			{
				logFilterNotReady();
			}
			return result;
		}

		public override List<string> GetAll(string testString, params string[] sources)
		{
			List<string> list = new List<string>();
			if (Ready())
			{
				if (string.IsNullOrEmpty(testString))
				{
					logGetAll();
				}
				else if (Constants.DEBUG_DOMAINS)
				{
					if (sources == null || sources.Length == 0)
					{
						foreach (List<Regex> value3 in debugDomainsRegex.Values)
						{
							foreach (Regex item in value3)
							{
								MatchCollection matchCollection = item.Matches(testString);
								foreach (Match item2 in matchCollection)
								{
									foreach (Capture capture5 in item2.Captures)
									{
										Debug.Log(string.Concat("Test string contains a domain: '", capture5.Value, "' detected by regex '", item, "'"));
										if (!list.Contains(capture5.Value))
										{
											list.Add(capture5.Value);
										}
									}
								}
							}
						}
					}
					else
					{
						foreach (string text in sources)
						{
							List<Regex> value;
							if (debugDomainsRegex.TryGetValue(text, out value))
							{
								foreach (Regex item3 in value)
								{
									MatchCollection matchCollection2 = item3.Matches(testString);
									foreach (Match item4 in matchCollection2)
									{
										foreach (Capture capture6 in item4.Captures)
										{
											Debug.Log(string.Concat("Test string contains a domain: '", capture6.Value, "' detected by regex '", item3, "'' from source '", text, "'"));
											if (!list.Contains(capture6.Value))
											{
												list.Add(capture6.Value);
											}
										}
									}
								}
							}
							else
							{
								logResourceNotFound(text);
							}
						}
					}
				}
				else if (sources == null || sources.Length == 0)
				{
					foreach (Regex value4 in domainsRegex.Values)
					{
						MatchCollection matchCollection3 = value4.Matches(testString);
						foreach (Match item5 in matchCollection3)
						{
							foreach (Capture capture7 in item5.Captures)
							{
								if (!list.Contains(capture7.Value))
								{
									list.Add(capture7.Value);
								}
							}
						}
					}
				}
				else
				{
					foreach (string text2 in sources)
					{
						Regex value2;
						if (domainsRegex.TryGetValue(text2, out value2))
						{
							MatchCollection matchCollection4 = value2.Matches(testString);
							foreach (Match item6 in matchCollection4)
							{
								foreach (Capture capture8 in item6.Captures)
								{
									if (!list.Contains(capture8.Value))
									{
										list.Add(capture8.Value);
									}
								}
							}
						}
						else
						{
							logResourceNotFound(text2);
						}
					}
				}
			}
			else
			{
				logFilterNotReady();
			}
			return (from x in list.Distinct()
				orderby x
				select x).ToList();
		}

		public override string ReplaceAll(string testString, params string[] sources)
		{
			string text = testString;
			if (Ready())
			{
				if (string.IsNullOrEmpty(testString))
				{
					logReplaceAll();
					text = string.Empty;
				}
				else if (Constants.DEBUG_DOMAINS)
				{
					if (sources == null || sources.Length == 0)
					{
						foreach (List<Regex> value3 in debugDomainsRegex.Values)
						{
							foreach (Regex item in value3)
							{
								MatchCollection matchCollection = item.Matches(testString);
								foreach (Match item2 in matchCollection)
								{
									foreach (Capture capture5 in item2.Captures)
									{
										Debug.Log(string.Concat("Test string contains a domain: '", capture5.Value, "' detected by regex '", item, "'"));
										text = text.Replace(capture5.Value, Helper.CreateReplaceString(ReplaceCharacters, capture5.Value.Length));
									}
								}
							}
						}
					}
					else
					{
						foreach (string text2 in sources)
						{
							List<Regex> value;
							if (debugDomainsRegex.TryGetValue(text2, out value))
							{
								foreach (Regex item3 in value)
								{
									MatchCollection matchCollection2 = item3.Matches(testString);
									foreach (Match item4 in matchCollection2)
									{
										foreach (Capture capture6 in item4.Captures)
										{
											Debug.Log(string.Concat("Test string contains a domain: '", capture6.Value, "' detected by regex '", item3, "'' from source '", text2, "'"));
											text = text.Replace(capture6.Value, Helper.CreateReplaceString(ReplaceCharacters, capture6.Value.Length));
										}
									}
								}
							}
							else
							{
								logResourceNotFound(text2);
							}
						}
					}
				}
				else if (sources == null || sources.Length == 0)
				{
					foreach (Regex value4 in domainsRegex.Values)
					{
						MatchCollection matchCollection3 = value4.Matches(testString);
						foreach (Match item5 in matchCollection3)
						{
							foreach (Capture capture7 in item5.Captures)
							{
								text = text.Replace(capture7.Value, Helper.CreateReplaceString(ReplaceCharacters, capture7.Value.Length));
							}
						}
					}
				}
				else
				{
					foreach (string text3 in sources)
					{
						Regex value2;
						if (domainsRegex.TryGetValue(text3, out value2))
						{
							MatchCollection matchCollection4 = value2.Matches(testString);
							foreach (Match item6 in matchCollection4)
							{
								foreach (Capture capture8 in item6.Captures)
								{
									text = text.Replace(capture8.Value, Helper.CreateReplaceString(ReplaceCharacters, capture8.Value.Length));
								}
							}
						}
						else
						{
							logResourceNotFound(text3);
						}
					}
				}
			}
			else
			{
				logFilterNotReady();
			}
			return text;
		}

		public override string Replace(string text, List<string> badWords)
		{
			string text2 = text;
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogWarning("Parameter 'text' is null or empty!" + Environment.NewLine + "=> 'Replace()' will return an empty string.");
				text2 = string.Empty;
			}
			else if (badWords == null || badWords.Count == 0)
			{
				Debug.LogWarning("Parameter 'badWords' is null or empty!" + Environment.NewLine + "=> 'Replace()' will return the original string.");
			}
			else
			{
				foreach (string badWord in badWords)
				{
					text2 = text2.Replace(badWord, Helper.CreateReplaceString(ReplaceCharacters, badWord.Length));
				}
			}
			return text2;
		}
	}
}
