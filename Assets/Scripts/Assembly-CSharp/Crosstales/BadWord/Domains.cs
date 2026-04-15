using System;
using System.Collections.Generic;
using System.Text;

namespace Crosstales.BadWord
{
	[Serializable]
	public class Domains
	{
		public Source Source;

		public List<string> DomainList;

		public Domains(Source source, List<string> domainList)
		{
			Source = source;
			DomainList = domainList;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetType().Name + " {");
			stringBuilder.Append(string.Concat("Source='", Source, "',"));
			stringBuilder.Append(string.Concat("DomainList='", DomainList, "'"));
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
	}
}
