using UnityEngine;

namespace Crosstales.BadWord
{
	public class RandomColor : MonoBehaviour
	{
		public Vector2 ChangeInterval = new Vector2(5f, 15f);

		private float elapsedTime;

		private float changeTime;

		private Renderer currentRenderer;

		private Color32 startColor;

		private Color32 endColor;

		private float lerpTime;

		private void Start()
		{
			currentRenderer = GetComponent<Renderer>();
			elapsedTime = (changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y));
			startColor = currentRenderer.material.color;
		}

		private void Update()
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > changeTime)
			{
				endColor = Helper.HSVToRGB(Random.Range(0f, 360f), 1f, 1f);
				changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);
				lerpTime = (elapsedTime = 0f);
			}
			currentRenderer.material.color = Color.Lerp(startColor, endColor, lerpTime);
			if (lerpTime < 1f)
			{
				lerpTime += Time.deltaTime / (changeTime - 0.2f);
			}
			else
			{
				startColor = currentRenderer.material.color;
			}
		}
	}
}
