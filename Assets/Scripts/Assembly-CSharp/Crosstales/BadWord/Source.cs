using System;
using System.Text;
using UnityEngine;

namespace Crosstales.BadWord
{
	[Serializable]
	public abstract class Source
	{
		[Header("Information")]
		[Tooltip("Name of the source.")]
		public string Name = string.Empty;

		[Tooltip("Description for the source.")]
		public string Description = string.Empty;

		[Tooltip("Icon to represent the source (e.g. country flag)")]
		public Sprite Icon;

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetType().Name + " {");
			stringBuilder.Append("Name='" + Name + "',");
			stringBuilder.Append("Description='" + Description + "',");
			stringBuilder.Append(string.Concat("Icon='", Icon, "'"));
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
	}
}
