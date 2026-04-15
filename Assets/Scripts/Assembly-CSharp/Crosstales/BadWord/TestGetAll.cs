using UnityEngine;

namespace Crosstales.BadWord
{
	public class TestGetAll : TestBase
	{
		protected override void speedTest(ManagerMask mask)
		{
			stopWatch.Reset();
			stopWatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				MultiManager.GetAll(createRandomString(TextStartLength + TextGrowPerIteration * i), mask, TestSources);
			}
			stopWatch.Stop();
			Debug.Log(string.Concat("## ", mask, ": ", stopWatch.ElapsedMilliseconds, ";", (float)stopWatch.ElapsedMilliseconds / (float)Iterations, " ##"));
		}

		protected override void sanityTest(ManagerMask mask)
		{
			if (MultiManager.GetAll(null, mask).Count == 0)
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
			if (MultiManager.GetAll(string.Empty, mask).Count == 0)
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
				if (MultiManager.GetAll(TestBase.scunthorpe, mask, "test").Count == 0)
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
				if (MultiManager.GetAll(TestBase.scunthorpe, mask, (string[])null).Count == 0)
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
				if (MultiManager.GetAll(TestBase.scunthorpe, mask).Count == 0)
				{
					if (Debugging)
					{
						Debug.Log("Zero-length array bfor 'source' test passed");
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
				if (MultiManager.GetAll(TestBase.badword, mask).Count == 1)
				{
					if (Debugging)
					{
						Debug.Log("Normal bad word match test passed");
					}
				}
				else
				{
					Debug.LogError("Normal bad word match failed");
					failCounter++;
				}
				if (MultiManager.GetAll(TestBase.scunthorpe, mask).Count == 0)
				{
					if (Debugging)
					{
						Debug.Log("Normal bad word non-match test passed");
					}
				}
				else
				{
					Debug.LogError("Normal bad word non-match word test failed");
					failCounter++;
				}
				if (MultiManager.GetAll(TestBase.badword, mask, "english").Count == 1)
				{
					if (Debugging)
					{
						Debug.Log("Bad word resource match test passed");
					}
				}
				else
				{
					Debug.LogError("Bad word resource match failed");
					failCounter++;
				}
				if (MultiManager.GetAll(TestBase.noBadword, mask, "english").Count == 0)
				{
					if (Debugging)
					{
						Debug.Log("Bad word resource non-match test passed");
					}
				}
				else
				{
					Debug.LogError("Bad word resource non-match word test failed");
					failCounter++;
				}
				if (MultiManager.GetAll(TestBase.arabicBadword, mask).Count == 1)
				{
					if (Debugging)
					{
						Debug.Log("Arabic bad word match test passed");
					}
				}
				else
				{
					Debug.LogError("Arabic bad word match failed");
					failCounter++;
				}
				if (MultiManager.GetAll(TestBase.globalBadword, mask).Count == 1)
				{
					if (Debugging)
					{
						Debug.Log("Global bad word match test passed");
					}
				}
				else
				{
					Debug.LogError("Global bad word match failed");
					failCounter++;
				}
				if (MultiManager.GetAll(TestBase.nameBadword, mask).Count == 1)
				{
					if (Debugging)
					{
						Debug.Log("Name match test passed");
					}
				}
				else
				{
					Debug.LogError("Name match failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				if (MultiManager.GetAll(TestBase.domain, mask).Count == 1)
				{
					if (Debugging)
					{
						Debug.Log("Domain match test passed");
					}
				}
				else
				{
					Debug.LogError("Domain match failed");
					failCounter++;
				}
				if (MultiManager.GetAll(TestBase.noDomain, mask).Count == 0)
				{
					if (Debugging)
					{
						Debug.Log("Domain non-match test passed");
					}
				}
				else
				{
					Debug.LogError("Domain non-match word test failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization)
			{
				string testString = new string('A', cf.CharacterNumber);
				string testString2 = new string('A', cf.CharacterNumber - 1);
				if (MultiManager.GetAll(testString, mask).Count == 1)
				{
					if (Debugging)
					{
						Debug.Log("Capital match test passed");
					}
				}
				else
				{
					Debug.LogError("Capital match failed");
					failCounter++;
				}
				if (MultiManager.GetAll(testString2, mask).Count == 0)
				{
					if (Debugging)
					{
						Debug.Log("Capital non-match test passed");
					}
				}
				else
				{
					Debug.LogError("Capital non-match word test failed");
					failCounter++;
				}
			}
			if ((mask & ManagerMask.Punctuation) != ManagerMask.Punctuation)
			{
				return;
			}
			string testString3 = new string('!', pf.CharacterNumber);
			string testString4 = new string('!', pf.CharacterNumber - 1);
			if (MultiManager.GetAll(testString3, mask).Count == 1)
			{
				if (Debugging)
				{
					Debug.Log("Punctuation match test passed");
				}
			}
			else
			{
				Debug.LogError("Punctuation match failed");
				failCounter++;
			}
			if (MultiManager.GetAll(testString4, mask).Count == 0)
			{
				if (Debugging)
				{
					Debug.Log("Punctuation non-match test passed");
				}
			}
			else
			{
				Debug.LogError("Punctuation non-match word test failed");
				failCounter++;
			}
		}
	}
}
