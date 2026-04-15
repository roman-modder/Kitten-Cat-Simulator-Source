using UnityEngine;

namespace Crosstales.BadWord
{
	public class DomainProviderFile : DomainProvider
	{
		[Header("Resources")]
		[Tooltip("File path to the resources under Windows.")]
		public string FilePathWindows = string.Empty;

		[Tooltip("File path to the resources under OSX.")]
		public string FilePathOSX = string.Empty;

		[Tooltip("File path to the resources under Linux.")]
		public string FilePathLinux = string.Empty;

		[Tooltip("An array with all resources.")]
		public DomainFile[] Resources;

		private string filePath = string.Empty;

		public override void Load()
		{
			if (!Provider.loggedUnsupportedPlatform)
			{
				Debug.LogWarning("The current platform is not supported!");
				Provider.loggedUnsupportedPlatform = true;
			}
		}

		public override void Save()
		{
			Debug.LogWarning("Save not implemented!");
		}
	}
}
