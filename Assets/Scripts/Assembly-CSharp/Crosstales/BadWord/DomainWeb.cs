using System;
using UnityEngine;

namespace Crosstales.BadWord
{
	[Serializable]
	public class DomainWeb : Source
	{
		[Header("Settings")]
		[Tooltip("URL of text file containing all regular expressions for this source.")]
		public string URL = string.Empty;

		[Tooltip("Defines how many lines are skipped at the start of the file. Standard: 0")]
		public int SkipHeaderLines;

		[Tooltip("Defines how many lines are skipped at the end of the file. Standard: 0")]
		public int SkipFooterLines;

		[Tooltip("Character to split the lines (e.g. “content#description”). Standard: #")]
		public char SplitChar = '#';
	}
}
