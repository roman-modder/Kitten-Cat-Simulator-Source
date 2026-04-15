using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class CarGUI : MonoBehaviour
{
	public CarController car;

	private const float MphtoMps = 2.237f;

	private string display = "{0:0} mph \nGear: {1:0}/{2:0}\nRevs {3:0%}\nThrottle: {4:0%}\n";

	private void Update()
	{
		object[] args = new object[5]
		{
			car.CurrentSpeed * 2.237f,
			car.GearNum + 1,
			car.NumGears,
			car.RevsFactor,
			car.AccelInput
		};
		GetComponent<GUIText>().text = string.Format(display, args);
	}
}
