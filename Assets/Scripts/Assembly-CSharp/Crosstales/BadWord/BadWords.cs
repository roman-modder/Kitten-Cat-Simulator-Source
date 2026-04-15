using System;
using System.Collections.Generic;
using System.Text;

namespace Crosstales.BadWord
{
	[Serializable]
	public class BadWords
	{
		public Source Source;

		public List<string> BadWordList;

		public BadWords(Source source, List<string> badWordList)
		{
			Source = source;
			BadWordList = badWordList;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetType().Name + " {");
			stringBuilder.Append(string.Concat("Source='", Source, "',"));
			stringBuilder.Append(string.Concat("BadWordList='", BadWordList, "'"));
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
	}
}
