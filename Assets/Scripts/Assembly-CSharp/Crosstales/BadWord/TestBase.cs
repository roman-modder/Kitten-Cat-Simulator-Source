using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Crosstales.BadWord
{
	public abstract class TestBase : MonoBehaviour
	{
		public int Iterations = 50;

		public int TextStartLength = 100;

		public int TextGrowPerIteration;

		public ManagerMask[] Managers;

		public string[] TestSources;

		public string RandomChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.?!-*";

		public char ReplaceChar = '*';

		public bool Debugging;

		protected System.Random rd = new System.Random();

		protected Stopwatch stopWatch = new Stopwatch();

		protected int failCounter;

		protected static readonly string badword = "Fuuuccckkk";

		protected static readonly string noBadword = "assume";

		protected static readonly string domain = "goOgle.cOm";

		protected static readonly string noDomain = "my.cOmMand";

		protected static readonly string scunthorpe = "scuntHorPe";

		protected static readonly string arabicBadword = "آنتاكلب";

		protected static readonly string globalBadword = "h!+leR";

		protected static readonly string nameBadword = "bAmbi";

		protected BadWordFilter bwf;

		protected DomainFilter df;

		protected CapitalizationFilter cf;

		protected PunctuationFilter pf;

		private bool isFirsttime = true;

		public virtual void Update()
		{
			if (isFirsttime)
			{
				StartCoroutine(runTest());
				isFirsttime = false;
			}
		}

		protected virtual IEnumerator runTest()
		{
			UnityEngine.Debug.Log("*** '" + base.name + "' started. ***");
			while (!MultiManager.Ready())
			{
				yield return null;
			}
			bwf = (BadWordFilter)MultiManager.Filter();
			df = (DomainFilter)MultiManager.Filter(ManagerMask.Domain);
			cf = (CapitalizationFilter)MultiManager.Filter(ManagerMask.Capitalization);
			pf = (PunctuationFilter)MultiManager.Filter(ManagerMask.Punctuation);
			bwf.ReplaceCharacters = new string(ReplaceChar, 1);
			df.ReplaceCharacters = new string(ReplaceChar, 1);
			ManagerMask[] managers = Managers;
			foreach (ManagerMask mask in managers)
			{
				speedTest(mask);
				yield return null;
				sanityTest(mask);
				yield return null;
			}
			if (failCounter > 0)
			{
				UnityEngine.Debug.LogError("--- '" + base.name + "' ended with failures: " + failCounter + " ---");
			}
			else
			{
				UnityEngine.Debug.Log("+++ '" + base.name + "' successfully ended. +++");
			}
		}

		protected virtual string createRandomString(int stringLength)
		{
			char[] array = new char[stringLength];
			for (int i = 0; i < stringLength; i++)
			{
				array[i] = RandomChars[rd.Next(0, RandomChars.Length)];
			}
			return new string(array);
		}

		protected abstract void speedTest(ManagerMask mask);

		protected abstract void sanityTest(ManagerMask mask);
	}
}
