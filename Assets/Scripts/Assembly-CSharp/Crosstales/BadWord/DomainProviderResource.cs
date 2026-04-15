using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crosstales.BadWord
{
	public class DomainProviderResource : DomainProvider
	{
		[Header("Resources")]
		[Tooltip("An array with all resources.")]
		public DomainResource[] Resources;

		public override void Load()
		{
			StartCoroutine(loadResource());
		}

		public override void Save()
		{
			Debug.LogWarning("Save is not possible for Unity resources!");
		}

		private IEnumerator loadResource()
		{
			domains.Clear();
			if (Resources != null && Resources.Length > 0)
			{
				DomainResource[] resources = Resources;
				foreach (DomainResource res in resources)
				{
					if (!string.IsNullOrEmpty(res.Name))
					{
						if (res.Resource != null)
						{
							List<string> list = Helper.SplitStringToLines(res.Resource.text, res.SkipHeaderLines, res.SkipFooterLines, res.SplitChar);
							if (list.Count > 0)
							{
								domains.Add(new Domains(res, list));
							}
							else
							{
								Debug.LogWarning("Resource: '" + res.Name + "' does not contain any active domains!");
							}
						}
						else
						{
							Debug.LogWarning("Resource field 'Resource' is null or empty!" + Environment.NewLine + "Please add a valid resource.");
						}
					}
					else
					{
						Debug.LogWarning("Resource field 'Source' is null or empty!" + Environment.NewLine + "Please add a valid source name.");
					}
					yield return null;
				}
			}
			else
			{
				logNoResourcesAdded();
			}
			init();
		}
	}
}
