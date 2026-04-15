using System;
using ArabicSupport;
using UnityEngine;

[Serializable]
public class Fix3dTextJS : MonoBehaviour
{
	public string text;

	public bool tashkeel;

	public bool hinduNumbers;

	public Fix3dTextJS()
	{
		text = string.Empty;
		tashkeel = true;
		hinduNumbers = true;
	}

	public virtual void Start()
	{
		((TextMesh)gameObject.GetComponent(typeof(TextMesh))).text = ArabicFixer.Fix(text, tashkeel, hinduNumbers);
	}

	public virtual void Update()
	{
	}

	public virtual void Main()
	{
	}
}
