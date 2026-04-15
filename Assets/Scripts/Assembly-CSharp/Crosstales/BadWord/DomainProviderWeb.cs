using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crosstales.BadWord
{
	public class DomainProviderWeb : DomainProvider
	{
		[Header("Resources")]
		[Tooltip("Main URL to the resources.")]
		public string MainURL = string.Empty;

		[Tooltip("An array with all resources.")]
		public DomainWeb[] Resources;

		public override void Load()
		{
			StartCoroutine(loadWeb());
		}

		public override void Save()
		{
			Debug.LogWarning("Save not implemented!");
		}

		private IEnumerator loadWeb()
		{
			domains.Clear();
			if (Resources != null && Resources.Length > 0)
			{
				DomainWeb[] resources = Resources;
				foreach (DomainWeb res in resources)
				{
					if (!string.IsNullOrEmpty(res.Name))
					{
						if (!string.IsNullOrEmpty(res.URL))
						{
							WWW www = new WWW(MainURL + res.URL);
							yield return www;
							while (!www.isDone)
							{
								yield return null;
							}
							if (string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.text))
							{
								List<string> list = Helper.SplitStringToLines(www.text, res.SkipHeaderLines, res.SkipFooterLines, res.SplitChar);
								if (list.Count > 0)
								{
									domains.Add(new Domains(res, list));
								}
								else
								{
									Debug.LogWarning("Web resource: '" + MainURL + res.URL + "' does not contain any active domains!");
								}
								continue;
							}
							Debug.LogWarning("Could not load web resource: '" + MainURL + res.URL + "'" + Environment.NewLine + www.error + Environment.NewLine + "Did you set the correct 'Main URL' and 'Resources' in the editor?" + Environment.NewLine + "=> 'ReadTextLines()' will return an empty string for the given resource.");
						}
						else
						{
							Debug.LogWarning("Resource field 'URL' is null or empty!" + Environment.NewLine + "Please add a valid url.");
						}
					}
					else
					{
						Debug.LogWarning("Resource field 'Source' is null or empty!" + Environment.NewLine + "Please add a valid source name.");
					}
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
