using UnityEngine;
using UnityEngine.UI;

public class PlayerCountLabel : MonoBehaviour
{
	public Slider slider;

	public void OnChange()
	{
		GetComponent<Text>().text = slider.value + " Players";
	}
}
