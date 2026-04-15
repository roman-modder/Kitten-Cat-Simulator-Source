using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuAgeCheck : MonoBehaviour
{
	public GameObject panelMenu;

	public MenuPolicy menuPolicy;

	public GameObject btnTemplate;

	public Button btnAcceptAge;

	private int selectedAgeYear;

	private Text[] txtLabels = new Text[100];

	private Text txtLabelCenter;

	private void Awake()
	{
		btnAcceptAge.interactable = false;
		SetupAgeButtons();
		StartCoroutine(SelectDefault());
	}

	private IEnumerator SelectDefault()
	{
		Canvas.ForceUpdateCanvases();
		yield return null;
		btnTemplate.GetComponentInParent<ScrollRect>().verticalScrollbar.value = 0.83f;
		Canvas.ForceUpdateCanvases();
	}

	private GameObject SpawnAgeButton(string strName)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(btnTemplate.gameObject);
		gameObject.GetComponentInChildren<Text>().text = strName;
		gameObject.transform.SetParent(btnTemplate.transform.parent, false);
		return gameObject;
	}

	private void SetupAgeButtons()
	{
		GameObject gameObject = null;
		int num = 0;
		for (int num2 = DateTime.Now.Year; num2 > DateTime.Now.Year - txtLabels.Length; num2--)
		{
			gameObject = SpawnAgeButton(num2.ToString());
			txtLabels[num] = gameObject.GetComponentInChildren<Text>();
			num++;
			string s = gameObject.GetComponentInChildren<Text>().text;
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				AgeButtonClick(s);
			});
			if (num2 == 2000)
			{
				gameObject = SpawnAgeButton("----");
				txtLabelCenter = gameObject.GetComponentInChildren<Text>();
				txtLabelCenter.color = Color.blue;
			}
		}
		btnTemplate.gameObject.SetActive(false);
	}

	private void AgeButtonClick(string num)
	{
		btnAcceptAge.interactable = true;
		selectedAgeYear = int.Parse(num);
		for (int i = 0; i < txtLabels.Length; i++)
		{
			if (txtLabels[i].text == selectedAgeYear.ToString())
			{
				txtLabels[i].color = Color.blue;
			}
			else
			{
				txtLabels[i].color = Color.black;
			}
		}
		txtLabelCenter.color = Color.black;
	}

	public void AcceptAge()
	{
		PlayerPrefs.SetInt("policy_year", selectedAgeYear);
		PlayerPrefs.Save();
		Hide();
		menuPolicy.Show();
	}

	private void Hide()
	{
		panelMenu.gameObject.SetActive(false);
	}

	public static int GetAge()
	{
		return DateTime.Now.Year - PlayerPrefs.GetInt("policy_year", 2000);
	}
}
