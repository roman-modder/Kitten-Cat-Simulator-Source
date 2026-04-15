using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crosstales.BadWord
{
	public class DomainManager : Manager
	{
		[Header("Domain Provider")]
		[Tooltip("List of all domain providers.")]
		public List<DomainProvider> DomainProvider;

		[Header("Settings")]
		[Tooltip("Replace characters for domains. Standard: *")]
		public string ReplaceChars = "*";

		private static DomainFilter filter;

		private static bool initalized;

		private const string clazz = "DomainManager";

		private static bool loggedFilterIsNull;

		private static bool loggedOnlyOneInstance;

		private void Start()
		{
			if (!initalized)
			{
				StartCoroutine(initalize());
			}
			else if (Application.isPlaying)
			{
				if (!loggedOnlyOneInstance)
				{
					Debug.LogWarning("Only one active instance of 'DomainManager' allowed in project!" + Environment.NewLine + "This object will now be destroyed.");
					loggedOnlyOneInstance = true;
				}
				UnityEngine.Object.Destroy(base.gameObject, 1f);
			}
		}

		private IEnumerator initalize()
		{
			filter = new DomainFilter(DomainProvider, ReplaceChars, MarkPrefix, MarkPostfix);
			while (!Ready())
			{
				yield return null;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
			initalized = true;
		}

		public static DomainFilter Filter()
		{
			return filter;
		}

		public static bool Ready()
		{
			bool result = false;
			if (filter != null)
			{
				result = filter.Ready();
			}
			else
			{
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		public static List<Source> Sources()
		{
			List<Source> result = new List<Source>();
			if (filter != null)
			{
				result = filter.Sources();
			}
			else
			{
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		public static bool Contains(string testString, params string[] sources)
		{
			bool result = false;
			if (filter != null)
			{
				result = filter.Contains(testString, sources);
			}
			else
			{
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		public static List<string> GetAll(string testString, params string[] sources)
		{
			List<string> result = new List<string>();
			if (filter != null)
			{
				result = filter.GetAll(testString, sources);
			}
			else
			{
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		public static string ReplaceAll(string testString, params string[] sources)
		{
			string result = testString;
			if (filter != null)
			{
				result = filter.ReplaceAll(testString, sources);
			}
			else
			{
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		public static string Replace(string text, List<string> badWords)
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Replace(text, badWords);
			}
			else
			{
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		public static string Mark(string text, List<string> badWords, string prefix = "", string postfix = "")
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Mark(text, badWords, prefix, postfix);
			}
			else
			{
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		public static string Unmark(string text, string prefix = "", string postfix = "")
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Unmark(text, prefix, postfix);
			}
			else
			{
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		private static void logFilterIsNull(string clazz)
		{
			if (!loggedFilterIsNull)
			{
				Debug.LogWarning("Filter is null!" + Environment.NewLine + "Did you add the '" + clazz + "' to the current scene?");
				loggedFilterIsNull = true;
			}
		}
	}
}
