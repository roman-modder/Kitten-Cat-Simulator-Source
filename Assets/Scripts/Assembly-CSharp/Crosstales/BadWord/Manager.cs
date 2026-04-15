using UnityEngine;

namespace Crosstales.BadWord
{
	public abstract class Manager : MonoBehaviour
	{
		[Header("Marker")]
		[Tooltip("Mark prefix for bad words. Standard: bold and red")]
		public string MarkPrefix = "<b><color=red>";

		[Tooltip("Mark postfix for bad words. Standard: bold and red")]
		public string MarkPostfix = "</color></b>";
	}
}
