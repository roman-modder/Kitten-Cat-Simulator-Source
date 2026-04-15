using UnityEngine;

namespace Crosstales.BadWord
{
	public class RandomRotator : MonoBehaviour
	{
		public Vector3 Speed;

		public Vector2 ChangeInterval = new Vector2(10f, 45f);

		private Transform tf;

		private Vector3 speed;

		private float elapsedTime;

		private float changeTime;

		private void Start()
		{
			tf = base.transform;
			elapsedTime = (changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y));
		}

		private void Update()
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > changeTime)
			{
				speed.x = Random.Range(0f - Mathf.Abs(Speed.x), Mathf.Abs(Speed.x));
				speed.y = Random.Range(0f - Mathf.Abs(Speed.y), Mathf.Abs(Speed.y));
				speed.z = Random.Range(0f - Mathf.Abs(Speed.z), Mathf.Abs(Speed.z));
				changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);
				elapsedTime = 0f;
			}
			tf.Rotate(speed.x * Time.deltaTime, speed.y * Time.deltaTime, speed.z * Time.deltaTime);
		}
	}
}
