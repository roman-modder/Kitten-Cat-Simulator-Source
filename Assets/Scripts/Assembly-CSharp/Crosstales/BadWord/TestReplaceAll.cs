using UnityEngine;

namespace Crosstales.BadWord
{
	public class TestReplaceAll : TestBase
	{
		protected override void speedTest(ManagerMask mask)
		{
			stopWatch.Reset();
			stopWatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				MultiManager.ReplaceAll(createRandomString(TextStartLength + TextGrowPerIteration * i), mask, TestSources);
			}
			stopWatch.Stop();
			Debug.Log(string.Concat("## ", mask, ": ", stopWatch.ElapsedMilliseconds, ";", (float)stopWatch.ElapsedMilliseconds / (float)Iterations, " ##"));
		}

		protected override void sanityTest(ManagerMask mask)
		{
			if (MultiManager.ReplaceAll(null, mask).Equals(string.Empty))
			{
				if (Debugging)
				{
					Debug.Log("Nullable test passed");
				}
			}
			else
			{
				Debug.LogError("Nullable test failed");
				failCounter++;
			}
			if (MultiManager.ReplaceAll(string.Empty, mask).Equals(string.Empty))
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
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				if (MultiManager.ReplaceAll(TestBase.scunthorpe, mask, "test").Equals(TestBase.scunthorpe))
				{
					if (Debugging)
					{
						Debug.Log("Wrong 'source' test passed");
					}
				}
				else
				{
					Debug.LogError("Wrong 'source' test failed");
					failCounter++;
				}
				if (MultiManager.ReplaceAll(TestBase.scunthorpe, mask, (string[])null).Equals(TestBase.scunthorpe))
				{
					if (Debugging)
					{
						Debug.Log("Null for 'source' test passed");
					}
				}
				else
				{
					Debug.LogError("Null for 'source' test failed");
					failCounter++;
				}
				if (MultiManager.ReplaceAll(TestBase.scunthorpe, mask).Equals(TestBase.scunthorpe))
				{
					if (Debugging)
					{
						Debug.Log("Zero-length array for 'source' test passed");
					}
				}
				else
				{
					Debug.LogError("Zero-length array for 'source' test failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				if (MultiManager.ReplaceAll(TestBase.badword, mask).Equals(new string(ReplaceChar, TestBase.badword.Length)))
				{
					if (Debugging)
					{
						Debug.Log("Normal bad word replace test passed");
					}
				}
				else
				{
					Debug.LogError("Normal bad word replace test failed");
					failCounter++;
				}
				if (MultiManager.ReplaceAll(TestBase.scunthorpe, mask).Equals(TestBase.scunthorpe))
				{
					if (Debugging)
					{
						Debug.Log("Normal bad word non-replace test passed");
					}
				}
				else
				{
					Debug.LogError("Normal bad word non-replace word test failed");
					failCounter++;
				}
				if (MultiManager.ReplaceAll(TestBase.badword, mask, "english").Equals(new string(ReplaceChar, TestBase.badword.Length)))
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
				if (MultiManager.ReplaceAll(TestBase.noBadword, mask, "english").Equals(TestBase.noBadword))
				{
					if (Debugging)
					{
						Debug.Log("Bad word resource non-replace test passed");
					}
				}
				else
				{
					Debug.LogError("Bad word resource non-replace word test failed");
					failCounter++;
				}
				if (MultiManager.ReplaceAll(TestBase.arabicBadword, mask).Equals(new string(ReplaceChar, TestBase.arabicBadword.Length)))
				{
					if (Debugging)
					{
						Debug.Log("Arabic bad word replace test passed");
					}
				}
				else
				{
					Debug.LogError("Arabic bad word replace failed");
					failCounter++;
				}
				if (MultiManager.ReplaceAll(TestBase.globalBadword, mask).Equals(new string(ReplaceChar, TestBase.globalBadword.Length)))
				{
					if (Debugging)
					{
						Debug.Log("Global bad word replace test passed");
					}
				}
				else
				{
					Debug.LogError("Global bad word replace failed");
					failCounter++;
				}
				if (MultiManager.ReplaceAll(TestBase.nameBadword, mask).Equals(new string(ReplaceChar, TestBase.nameBadword.Length)))
				{
					if (Debugging)
					{
						Debug.Log("Name replace test passed");
					}
				}
				else
				{
					Debug.LogError("Name replace failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				if (MultiManager.ReplaceAll(TestBase.domain, mask).Equals(new string(ReplaceChar, TestBase.domain.Length)))
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
				if (MultiManager.ReplaceAll(TestBase.noDomain, mask).Equals(TestBase.noDomain))
				{
					if (Debugging)
					{
						Debug.Log("Domain non-replace test passed");
					}
				}
				else
				{
					Debug.LogError("Domain non-replace word test failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization)
			{
				string text = new string('A', cf.CharacterNumber);
				string text2 = new string('A', cf.CharacterNumber - 1);
				if (MultiManager.ReplaceAll(text, mask).Equals(text.ToLowerInvariant()))
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
				if (MultiManager.ReplaceAll(text2, mask).Equals(text2))
				{
					if (Debugging)
					{
						Debug.Log("Capital non-replace test passed");
					}
				}
				else
				{
					Debug.LogError("Capital non-replace word test failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.Punctuation) != ManagerMask.Punctuation)
			{
				return;
			}
			string testString = new string('!', pf.CharacterNumber);
			string text3 = new string('!', pf.CharacterNumber - 1);
			if (MultiManager.ReplaceAll(testString, mask).Equals(text3))
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
			if (MultiManager.ReplaceAll(text3, mask).Equals(text3))
			{
				if (Debugging)
				{
					Debug.Log("Punctuation non-replace test passed");
				}
			}
			else
			{
				Debug.LogError("Punctuation non-replace word test failed");
				failCounter++;
			}
		}
	}
}
