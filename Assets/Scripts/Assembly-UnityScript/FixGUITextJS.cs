using System;
using ArabicSupport;
using UnityEngine;

[Serializable]
public class FixGUITextJS : MonoBehaviour
{
	public string text;

	public bool tashkeel;

	public bool hinduNumbers;

	public FixGUITextJS()
	{
		text = string.Empty;
		tashkeel = true;
		hinduNumbers = true;
	}

	public virtual void Start()
	{
		gameObject.GetComponent<GUIText>().text = ArabicFixer.Fix(text, tashkeel, hinduNumbers);
	}

	public virtual void Update()
	{
	}

	public virtual void Main()
	{
	}
}
