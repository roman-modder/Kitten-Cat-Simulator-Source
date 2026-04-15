using System.Collections.Generic;
using UnityEngine;

namespace Crosstales.BadWord
{
	public class TestReplace : TestBase
	{
		protected override void speedTest(ManagerMask mask)
		{
			List<string> list = new List<string>();
			list.Add(TestBase.badword);
			stopWatch.Reset();
			stopWatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				MultiManager.Replace(createRandomString(TextStartLength + TextGrowPerIteration * i), list, mask);
			}
			stopWatch.Stop();
			Debug.Log(string.Concat("## ", mask, ": ", stopWatch.ElapsedMilliseconds, ";", (float)stopWatch.ElapsedMilliseconds / (float)Iterations, " ##"));
		}

		protected override void sanityTest(ManagerMask mask)
		{
			List<string> badWords = new List<string>();
			if (MultiManager.Replace(null, badWords, mask).Equals(string.Empty))
			{
				if (Debugging)
				{
					Debug.Log("Nullable 'text test passed");
				}
			}
			else
			{
				Debug.LogError("Nullable 'text' test failed");
				failCounter++;
			}
			if (MultiManager.Replace(TestBase.scunthorpe, null, mask).Equals(TestBase.scunthorpe))
			{
				if (Debugging)
				{
					Debug.Log("Nullable 'badWords test passed");
				}
			}
			else
			{
				Debug.LogError("Nullable 'badWords' test failed");
				failCounter++;
			}
			if (MultiManager.Replace(string.Empty, badWords, mask).Equals(string.Empty))
			{
				if (Debugging)
				{
					Debug.Log("Empty test passed");
				}
			}
			else
			{
				Debug.LogError("Empty test failed");
				failCounter++;
			}
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				List<string> list = new List<string>();
				list.Add(TestBase.badword);
				if (MultiManager.Replace(TestBase.badword, list, mask).Equals(new string(ReplaceChar, TestBase.badword.Length)))
				{
					if (Debugging)
					{
						Debug.Log("Bad word resource replace test passed");
					}
				}
				else
				{
					Debug.LogError("Bad word resource replace failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				List<string> list2 = new List<string>();
				list2.Add(TestBase.domain);
				if (MultiManager.Replace(TestBase.domain, list2, mask).Equals(new string(ReplaceChar, TestBase.domain.Length)))
				{
					if (Debugging)
					{
						Debug.Log("Domain replace test passed");
					}
				}
				else
				{
					Debug.LogError("Domain match failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization)
			{
				string text = new string('A', cf.CharacterNumber);
				List<string> list3 = new List<string>();
				list3.Add(text);
				if (MultiManager.Replace(text, list3, mask).Equals(text.ToLowerInvariant()))
				{
					if (Debugging)
					{
						Debug.Log("Capital replace test passed");
					}
				}
				else
				{
					Debug.LogError("Capital replace failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.Punctuation) != ManagerMask.Punctuation)
			{
				return;
			}
			string text2 = new string('!', pf.CharacterNumber);
			List<string> list4 = new List<string>();
			list4.Add(text2);
			if (MultiManager.Replace(text2, list4, mask).Equals(new string('!', pf.CharacterNumber - 1)))
			{
				if (Debugging)
				{
					Debug.Log("Punctuation replace test passed");
				}
			}
			else
			{
				Debug.LogError("Punctuation replace failed");
				failCounter++;
			}
		}
	}
}
