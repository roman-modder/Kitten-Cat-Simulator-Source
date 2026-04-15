using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.BadWord
{
	public class GUIMain : MonoBehaviour
	{
		public bool AutoTest = true;

		public bool AutoReplace;

		public bool Fuzzy;

		public float IntervalCheck = 0.5f;

		public float IntervalReplace = 0.5f;

		public InputField Text;

		public Text OutputText;

		public Text BadWordList;

		public Text BadWordCounter;

		public Text Version;

		public Toggle TestEnabled;

		public Toggle ReplaceEnabled;

		public Toggle Badword;

		public Toggle Domain;

		public Toggle Capitalization;

		public Toggle Punctuation;

		public InputField BadwordReplaceChars;

		public InputField DomainReplaceChars;

		public InputField CapsTrigger;

		public InputField PuncTrigger;

		public Toggle FuzzyEnabled;

		public Image BadWordListImage;

		public Color32 GoodColor = new Color32(0, byte.MaxValue, 0, 192);

		public Color32 BadColor = new Color32(byte.MaxValue, 0, 0, 192);

		public ManagerMask BadwordManager = ManagerMask.BadWord;

		public ManagerMask DomainManager = ManagerMask.Domain;

		public ManagerMask CapsManager = ManagerMask.Capitalization;

		public ManagerMask PuncManager = ManagerMask.Punctuation;

		public List<string> Sources = new List<string>();

		private List<string> badWords = new List<string>();

		private float elapsedTimeCheck;

		private float elapsedTimeReplace;

		private BadWordFilter bwf;

		private DomainFilter df;

		private CapitalizationFilter cf;

		private PunctuationFilter pf;

		private bool tested;

		public void Start()
		{
			bwf = (BadWordFilter)MultiManager.Filter();
			df = (DomainFilter)MultiManager.Filter(ManagerMask.Domain);
			cf = (CapitalizationFilter)MultiManager.Filter(ManagerMask.Capitalization);
			pf = (PunctuationFilter)MultiManager.Filter(ManagerMask.Punctuation);
			Version.text = Constants.ASSET_NAME + " - " + Constants.ASSET_VERSION;
			if (!AutoTest)
			{
				TestEnabled.isOn = false;
			}
			if (!AutoReplace)
			{
				ReplaceEnabled.isOn = false;
			}
			if (BadwordManager != ManagerMask.BadWord)
			{
				Badword.isOn = false;
			}
			if (DomainManager != ManagerMask.Domain)
			{
				Domain.isOn = false;
			}
			if (CapsManager != ManagerMask.Capitalization)
			{
				Capitalization.isOn = false;
			}
			if (PuncManager != ManagerMask.Punctuation)
			{
				Punctuation.isOn = false;
			}
			bwf.Fuzzy = Fuzzy;
			if (!Fuzzy)
			{
				FuzzyEnabled.isOn = false;
			}
			BadwordReplaceChars.text = bwf.ReplaceCharacters;
			DomainReplaceChars.text = df.ReplaceCharacters;
			CapsTrigger.text = cf.CharacterNumber.ToString();
			PuncTrigger.text = pf.CharacterNumber.ToString();
			BadWordList.text = ((badWords.Count <= 0) ? "Not tested" : string.Empty);
			Text.text = string.Empty;
		}

		public void Update()
		{
			elapsedTimeCheck += Time.deltaTime;
			elapsedTimeReplace += Time.deltaTime;
			if (AutoTest && !AutoReplace && elapsedTimeCheck > IntervalCheck)
			{
				Test();
				elapsedTimeCheck = 0f;
			}
			if (AutoReplace && elapsedTimeReplace > IntervalReplace)
			{
				Replace();
				elapsedTimeReplace = 0f;
			}
			bwf.ReplaceCharacters = BadwordReplaceChars.text;
			df.ReplaceCharacters = DomainReplaceChars.text;
			int result;
			bool flag = int.TryParse(CapsTrigger.text, out result);
			cf.CharacterNumber = ((!flag) ? 2 : ((result <= 2) ? 2 : result));
			CapsTrigger.text = cf.CharacterNumber.ToString();
			flag = int.TryParse(PuncTrigger.text, out result);
			pf.CharacterNumber = ((!flag) ? 2 : ((result <= 2) ? 2 : result));
			PuncTrigger.text = pf.CharacterNumber.ToString();
			if (tested)
			{
				if (badWords.Count > 0)
				{
					BadWordList.text = string.Join(Environment.NewLine, badWords.ToArray());
					BadWordListImage.color = BadColor;
				}
				else
				{
					BadWordList.text = "No bad words found";
					BadWordListImage.color = GoodColor;
				}
			}
			BadWordCounter.text = badWords.Count.ToString();
			OutputText.text = MultiManager.Mark(Text.text, badWords, string.Empty, string.Empty);
		}

		public void TestChanged(bool val)
		{
			AutoTest = val;
		}

		public void ReplaceChanged(bool val)
		{
			AutoReplace = val;
		}

		public void BadwordChanged(bool val)
		{
			BadwordManager = (val ? ManagerMask.BadWord : ManagerMask.None);
		}

		public void DomainChanged(bool val)
		{
			DomainManager = (val ? ManagerMask.Domain : ManagerMask.None);
		}

		public void CapitalizationChanged(bool val)
		{
			CapsManager = (val ? ManagerMask.Capitalization : ManagerMask.None);
		}

		public void PunctuationChanged(bool val)
		{
			PuncManager = (val ? ManagerMask.Punctuation : ManagerMask.None);
		}

		public void FuzzyChanged(bool val)
		{
			bwf.Fuzzy = val;
		}

		public void FullscreenChanged(bool val)
		{
			Screen.fullScreen = val;
		}

		public void Test()
		{
			tested = true;
			if (!string.IsNullOrEmpty(Text.text))
			{
				badWords = MultiManager.GetAll(Text.text, BadwordManager | DomainManager | CapsManager | PuncManager, Sources.ToArray());
			}
		}

		public void Replace()
		{
			tested = true;
			if (!string.IsNullOrEmpty(Text.text))
			{
				Text.text = MultiManager.ReplaceAll(Text.text, BadwordManager | DomainManager | CapsManager | PuncManager, Sources.ToArray());
				badWords.Clear();
			}
		}

		public void OpenAssetURL()
		{
			Application.OpenURL(Constants.ASSET_URL);
		}

		public void OpenCTURL()
		{
			Application.OpenURL(Constants.ASSET_AUTHOR_URL);
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}
