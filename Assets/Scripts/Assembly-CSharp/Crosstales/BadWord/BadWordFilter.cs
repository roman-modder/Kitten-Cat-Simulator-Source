using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Crosstales.BadWord
{
	public class BadWordFilter : Filter
	{
		private List<BadWordProvider> badWordProviderLTR;

		private List<BadWordProvider> badWordProviderRTL;

		public string ReplaceCharacters;

		public bool Fuzzy;

		private List<BadWordProvider> tempBadWordProviderLTR;

		private List<BadWordProvider> tempBadWordProviderRTL;

		private Dictionary<string, Regex> exactBadwordsRegex = new Dictionary<string, Regex>();

		private Dictionary<string, Regex> fuzzyBadwordsRegex = new Dictionary<string, Regex>();

		private Dictionary<string, List<Regex>> debugExactBadwordsRegex = new Dictionary<string, List<Regex>>();

		private Dictionary<string, List<Regex>> debugFuzzyBadwordsRegex = new Dictionary<string, List<Regex>>();

		private bool isReady;

		private bool isReadyFirstime;

		public List<BadWordProvider> BadWordProviderLTR
		{
			get
			{
				return badWordProviderLTR;
			}
			set
			{
				badWordProviderLTR = value;
				if (badWordProviderLTR != null && badWordProviderLTR.Count > 0)
				{
					foreach (BadWordProvider item in badWordProviderLTR)
					{
						if (item != null)
						{
							if (Constants.DEBUG_BADWORDS)
							{
								debugExactBadwordsRegex.AddRange(item.DebugExactBadwordsRegex);
								debugFuzzyBadwordsRegex.AddRange(item.DebugFuzzyBadwordsRegex);
							}
							else
							{
								exactBadwordsRegex.AddRange(item.ExactBadwordsRegex);
								fuzzyBadwordsRegex.AddRange(item.FuzzyBadwordsRegex);
							}
						}
						else
						{
							Debug.LogError("A LTR-BadWordProvider is null!");
						}
					}
					return;
				}
				badWordProviderLTR = new List<BadWordProvider>();
				Debug.LogWarning("No 'BadWordProviderLTR' added!" + Environment.NewLine + "If you want to use this functionality, please add your desired 'BadWordProviderLTR' in the editor or script.");
			}
		}

		public List<BadWordProvider> BadWordProviderRTL
		{
			get
			{
				return badWordProviderRTL;
			}
			set
			{
				badWordProviderRTL = value;
				if (badWordProviderRTL != null && badWordProviderRTL.Count > 0)
				{
					foreach (BadWordProvider item in badWordProviderRTL)
					{
						if (item != null)
						{
							if (Constants.DEBUG_BADWORDS)
							{
								debugExactBadwordsRegex.AddRange(item.DebugExactBadwordsRegex);
								debugFuzzyBadwordsRegex.AddRange(item.DebugFuzzyBadwordsRegex);
							}
							else
							{
								exactBadwordsRegex.AddRange(item.ExactBadwordsRegex);
								fuzzyBadwordsRegex.AddRange(item.FuzzyBadwordsRegex);
							}
						}
						else
						{
							Debug.LogError("A RTL-BadWordProvider is null!");
						}
					}
					return;
				}
				badWordProviderRTL = new List<BadWordProvider>();
				Debug.LogWarning("No 'BadWordProviderRTL' added!" + Environment.NewLine + "If you want to use this functionality, please add your desired 'BadWordProviderRTL' in the editor or script.");
			}
		}

		public BadWordFilter(List<BadWordProvider> badWordProviderLTR, List<BadWordProvider> badWordProviderRTL, string replaceCharacters, bool fuzzy, string markPrefix, string markPostfix)
		{
			tempBadWordProviderLTR = badWordProviderLTR;
			tempBadWordProviderRTL = badWordProviderRTL;
			ReplaceCharacters = replaceCharacters;
			Fuzzy = fuzzy;
			MarkPrefix = markPrefix;
			MarkPostfix = markPostfix;
		}

		public override bool Ready()
		{
			bool flag = true;
			if (!isReady)
			{
				foreach (BadWordProvider item in tempBadWordProviderLTR)
				{
					if (item != null && !item.Ready)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					foreach (BadWordProvider item2 in tempBadWordProviderRTL)
					{
						if (item2 != null && !item2.Ready)
						{
							flag = false;
							break;
						}
					}
				}
				if (!isReadyFirstime && flag)
				{
					BadWordProviderLTR = tempBadWordProviderLTR;
					BadWordProviderRTL = tempBadWordProviderRTL;
					foreach (BadWordProvider item3 in BadWordProviderLTR)
					{
						if (!(item3 != null))
						{
							continue;
						}
						foreach (Source source in item3.Sources)
						{
							sources.Add(source.Name, source);
						}
					}
					foreach (BadWordProvider item4 in BadWordProviderRTL)
					{
						if (!(item4 != null))
						{
							continue;
						}
						foreach (Source source2 in item4.Sources)
						{
							sources.Add(source2.Name, source2);
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
				else if (Constants.DEBUG_BADWORDS)
				{
					if (sources == null || sources.Length == 0)
					{
						if (Fuzzy)
						{
							foreach (List<Regex> value3 in debugFuzzyBadwordsRegex.Values)
							{
								foreach (Regex item in value3)
								{
									Match match = item.Match(testString);
									if (match.Success)
									{
										Debug.Log(string.Concat("Test string contains a bad word: '", match.Value, "' detected by regex '", item, "'"));
										result = true;
										break;
									}
								}
							}
						}
						else
						{
							foreach (List<Regex> value4 in debugExactBadwordsRegex.Values)
							{
								foreach (Regex item2 in value4)
								{
									Match match2 = item2.Match(testString);
									if (match2.Success)
									{
										Debug.Log(string.Concat("Test string contains a bad word: '", match2.Value, "' detected by regex '", item2, "'"));
										result = true;
										break;
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
							if (Fuzzy)
							{
								if (debugFuzzyBadwordsRegex.TryGetValue(text, out value))
								{
									foreach (Regex item3 in value)
									{
										Match match3 = item3.Match(testString);
										if (match3.Success)
										{
											Debug.Log(string.Concat("Test string contains a bad word: '", match3.Value, "' detected by regex '", item3, "'' from source '", text, "'"));
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
							else if (debugExactBadwordsRegex.TryGetValue(text, out value))
							{
								foreach (Regex item4 in value)
								{
									Match match4 = item4.Match(testString);
									if (match4.Success)
									{
										Debug.Log(string.Concat("Test string contains a bad word: '", match4.Value, "' detected by regex '", item4, "'' from source '", text, "'"));
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
					if (Fuzzy)
					{
						foreach (Regex value5 in fuzzyBadwordsRegex.Values)
						{
							if (value5.Match(testString).Success)
							{
								result = true;
								break;
							}
						}
					}
					else
					{
						foreach (Regex value6 in exactBadwordsRegex.Values)
						{
							if (value6.Match(testString).Success)
							{
								result = true;
								break;
							}
						}
					}
				}
				else
				{
					foreach (string text2 in sources)
					{
						Regex value2;
						if (Fuzzy)
						{
							if (fuzzyBadwordsRegex.TryGetValue(text2, out value2))
							{
								Match match5 = value2.Match(testString);
								if (match5.Success)
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
						else if (exactBadwordsRegex.TryGetValue(text2, out value2))
						{
							Match match6 = value2.Match(testString);
							if (match6.Success)
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
				else if (Constants.DEBUG_BADWORDS)
				{
					if (sources == null || sources.Length == 0)
					{
						if (Fuzzy)
						{
							foreach (List<Regex> value3 in debugFuzzyBadwordsRegex.Values)
							{
								foreach (Regex item in value3)
								{
									MatchCollection matchCollection = item.Matches(testString);
									foreach (Match item2 in matchCollection)
									{
										foreach (Capture capture9 in item2.Captures)
										{
											Debug.Log(string.Concat("Test string contains a bad word: '", capture9.Value, "' detected by regex '", item, "'"));
											if (!list.Contains(capture9.Value))
											{
												list.Add(capture9.Value);
											}
										}
									}
								}
							}
						}
						else
						{
							foreach (List<Regex> value4 in debugExactBadwordsRegex.Values)
							{
								foreach (Regex item3 in value4)
								{
									MatchCollection matchCollection2 = item3.Matches(testString);
									foreach (Match item4 in matchCollection2)
									{
										foreach (Capture capture10 in item4.Captures)
										{
											Debug.Log(string.Concat("Test string contains a bad word: '", capture10.Value, "' detected by regex '", item3, "'"));
											if (!list.Contains(capture10.Value))
											{
												list.Add(capture10.Value);
											}
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
							if (Fuzzy)
							{
								if (debugFuzzyBadwordsRegex.TryGetValue(text, out value))
								{
									foreach (Regex item5 in value)
									{
										MatchCollection matchCollection3 = item5.Matches(testString);
										foreach (Match item6 in matchCollection3)
										{
											foreach (Capture capture11 in item6.Captures)
											{
												Debug.Log(string.Concat("Test string contains a bad word: '", capture11.Value, "' detected by regex '", item5, "'' from source '", text, "'"));
												if (!list.Contains(capture11.Value))
												{
													list.Add(capture11.Value);
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
							else if (debugExactBadwordsRegex.TryGetValue(text, out value))
							{
								foreach (Regex item7 in value)
								{
									MatchCollection matchCollection4 = item7.Matches(testString);
									foreach (Match item8 in matchCollection4)
									{
										foreach (Capture capture12 in item8.Captures)
										{
											Debug.Log(string.Concat("Test string contains a bad word: '", capture12.Value, "' detected by regex '", item7, "'' from source '", text, "'"));
											if (!list.Contains(capture12.Value))
											{
												list.Add(capture12.Value);
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
					if (Fuzzy)
					{
						foreach (Regex value5 in fuzzyBadwordsRegex.Values)
						{
							MatchCollection matchCollection5 = value5.Matches(testString);
							foreach (Match item9 in matchCollection5)
							{
								foreach (Capture capture13 in item9.Captures)
								{
									if (!list.Contains(capture13.Value))
									{
										list.Add(capture13.Value);
									}
								}
							}
						}
					}
					else
					{
						foreach (Regex value6 in exactBadwordsRegex.Values)
						{
							MatchCollection matchCollection6 = value6.Matches(testString);
							foreach (Match item10 in matchCollection6)
							{
								foreach (Capture capture14 in item10.Captures)
								{
									if (!list.Contains(capture14.Value))
									{
										list.Add(capture14.Value);
									}
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
						if (Fuzzy)
						{
							if (fuzzyBadwordsRegex.TryGetValue(text2, out value2))
							{
								MatchCollection matchCollection7 = value2.Matches(testString);
								foreach (Match item11 in matchCollection7)
								{
									foreach (Capture capture15 in item11.Captures)
									{
										if (!list.Contains(capture15.Value))
										{
											list.Add(capture15.Value);
										}
									}
								}
							}
							else
							{
								logResourceNotFound(text2);
							}
						}
						else if (exactBadwordsRegex.TryGetValue(text2, out value2))
						{
							MatchCollection matchCollection8 = value2.Matches(testString);
							foreach (Match item12 in matchCollection8)
							{
								foreach (Capture capture16 in item12.Captures)
								{
									if (!list.Contains(capture16.Value))
									{
										list.Add(capture16.Value);
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
				else if (Constants.DEBUG_BADWORDS)
				{
					if (sources == null || sources.Length == 0)
					{
						if (Fuzzy)
						{
							foreach (List<Regex> value3 in debugFuzzyBadwordsRegex.Values)
							{
								foreach (Regex item in value3)
								{
									MatchCollection matchCollection = item.Matches(testString);
									foreach (Match item2 in matchCollection)
									{
										foreach (Capture capture9 in item2.Captures)
										{
											Debug.Log(string.Concat("Test string contains a bad word: '", capture9.Value, "' detected by regex '", item, "'"));
											text = text.Replace(capture9.Value, Helper.CreateReplaceString(ReplaceCharacters, capture9.Value.Length));
										}
									}
								}
							}
						}
						else
						{
							foreach (List<Regex> value4 in debugExactBadwordsRegex.Values)
							{
								foreach (Regex item3 in value4)
								{
									MatchCollection matchCollection2 = item3.Matches(testString);
									foreach (Match item4 in matchCollection2)
									{
										foreach (Capture capture10 in item4.Captures)
										{
											Debug.Log(string.Concat("Test string contains a bad word: '", capture10.Value, "' detected by regex '", item3, "'"));
											text = text.Replace(capture10.Value, Helper.CreateReplaceString(ReplaceCharacters, capture10.Value.Length));
										}
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
							if (Fuzzy)
							{
								if (debugFuzzyBadwordsRegex.TryGetValue(text2, out value))
								{
									foreach (Regex item5 in value)
									{
										MatchCollection matchCollection3 = item5.Matches(testString);
										foreach (Match item6 in matchCollection3)
										{
											foreach (Capture capture11 in item6.Captures)
											{
												Debug.Log(string.Concat("Test string contains a bad word: '", capture11.Value, "' detected by regex '", item5, "'' from source '", text2, "'"));
												text = text.Replace(capture11.Value, Helper.CreateReplaceString(ReplaceCharacters, capture11.Value.Length));
											}
										}
									}
								}
								else
								{
									logResourceNotFound(text2);
								}
							}
							else if (debugExactBadwordsRegex.TryGetValue(text2, out value))
							{
								foreach (Regex item7 in value)
								{
									MatchCollection matchCollection4 = item7.Matches(testString);
									foreach (Match item8 in matchCollection4)
									{
										foreach (Capture capture12 in item8.Captures)
										{
											Debug.Log(string.Concat("Test string contains a bad word: '", capture12.Value, "' detected by regex '", item7, "'' from source '", text2, "'"));
											text = text.Replace(capture12.Value, Helper.CreateReplaceString(ReplaceCharacters, capture12.Value.Length));
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
					if (Fuzzy)
					{
						foreach (Regex value5 in fuzzyBadwordsRegex.Values)
						{
							MatchCollection matchCollection5 = value5.Matches(testString);
							foreach (Match item9 in matchCollection5)
							{
								foreach (Capture capture13 in item9.Captures)
								{
									text = text.Replace(capture13.Value, Helper.CreateReplaceString(ReplaceCharacters, capture13.Value.Length));
								}
							}
						}
					}
					else
					{
						foreach (Regex value6 in exactBadwordsRegex.Values)
						{
							MatchCollection matchCollection6 = value6.Matches(testString);
							foreach (Match item10 in matchCollection6)
							{
								foreach (Capture capture14 in item10.Captures)
								{
									text = text.Replace(capture14.Value, Helper.CreateReplaceString(ReplaceCharacters, capture14.Value.Length));
								}
							}
						}
					}
				}
				else
				{
					foreach (string text3 in sources)
					{
						Regex value2;
						if (Fuzzy)
						{
							if (fuzzyBadwordsRegex.TryGetValue(text3, out value2))
							{
								MatchCollection matchCollection7 = value2.Matches(testString);
								foreach (Match item11 in matchCollection7)
								{
									foreach (Capture capture15 in item11.Captures)
									{
										text = text.Replace(capture15.Value, Helper.CreateReplaceString(ReplaceCharacters, capture15.Value.Length));
									}
								}
							}
							else
							{
								logResourceNotFound(text3);
							}
						}
						else if (exactBadwordsRegex.TryGetValue(text3, out value2))
						{
							MatchCollection matchCollection8 = value2.Matches(testString);
							foreach (Match item12 in matchCollection8)
							{
								foreach (Capture capture16 in item12.Captures)
								{
									text = text.Replace(capture16.Value, Helper.CreateReplaceString(ReplaceCharacters, capture16.Value.Length));
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
