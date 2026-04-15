using System;
using UnityEngine;

namespace Crosstales.BadWord
{
	[Serializable]
	public class BadWordFile : Source
	{
		[Header("Settings")]
		[Tooltip("Text file containing all regular expressions for this source.")]
		public string File = string.Empty;

		[Tooltip("Defines how many lines are skipped at the start of the file. Standard: 0")]
		public int SkipHeaderLines;

		[Tooltip("Defines how many lines are skipped at the end of the file. Standard: 0")]
		public int SkipFooterLines;

		[Tooltip("Character to split the lines (e.g. “content#description”). Standard: #")]
		public char SplitChar = '#';
	}
}
