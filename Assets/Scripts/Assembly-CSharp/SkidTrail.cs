using System.Collections;
using UnityEngine;

public class SkidTrail : MonoBehaviour
{
	[SerializeField]
	private float persistTime;

	[SerializeField]
	private float fadeDuration;

	private float startAlpha;

	private IEnumerator Start()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (base.transform.parent == null)
			{
				Color startCol = GetComponent<Renderer>().material.color;
				yield return new WaitForSeconds(persistTime);
				float t = Time.time;
				while (Time.time < t + fadeDuration)
				{
					float i = Mathf.InverseLerp(t, t + fadeDuration, Time.time);
					GetComponent<Renderer>().material.color = startCol * new Color(1f, 1f, 1f, 1f - i);
					yield return null;
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
