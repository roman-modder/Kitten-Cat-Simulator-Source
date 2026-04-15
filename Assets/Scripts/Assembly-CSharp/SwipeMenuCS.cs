using System;
using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/SwipeMenuCS")]
public class SwipeMenuCS : MonoBehaviour
{
	public float snapBackTime = 0.2f;

	public float completionDuration = 1f;

	public bool cyclical = true;

	private float targetPos;

	public float swipeSpeedInchesPerSec = 1f;

	public float minChangeDragInches = 1f;

	[NonSerialized]
	public float displayPos;

	[NonSerialized]
	public float windowSize = 100f;

	[NonSerialized]
	public float curPos;

	private float dpi = 72f;

	private float dampVel;

	[NonSerialized]
	public int curItem;

	[NonSerialized]
	public int itemCount;

	private bool pressed;

	private bool selected;

	private bool complete;

	private bool justCompleted;

	private float timeSinceSelection;

	public void Init(int itemCount, int curItem, float windowSize, float dpi, bool cyclical)
	{
		complete = false;
		justCompleted = false;
		pressed = false;
		selected = false;
		this.windowSize = Mathf.Max(2f, windowSize);
		this.dpi = Mathf.Max(40f, dpi);
		this.itemCount = Mathf.Max(1, itemCount);
		this.curItem = Mathf.Clamp(curItem, 0, this.itemCount - 1);
		displayPos = (targetPos = (curPos = (float)this.curItem * this.windowSize));
	}

	public void SetWindowSize(float windowSize, float dpi)
	{
		if (this.windowSize != windowSize || this.dpi != dpi)
		{
			displayPos = (targetPos = (float)curItem * this.windowSize);
			this.windowSize = windowSize;
			this.dpi = dpi;
			pressed = false;
		}
	}

	public int GetCurItem()
	{
		return curItem;
	}

	public bool Completed()
	{
		return complete;
	}

	public bool JustCompleted()
	{
		return justCompleted;
	}

	public bool Selected()
	{
		return selected;
	}

	public bool Pressed()
	{
		return pressed;
	}

	public float GetTimeSinceSelection()
	{
		return timeSinceSelection;
	}

	public void OnPress()
	{
		if (!selected)
		{
			pressed = true;
		}
	}

	public void OnRelease(float vel)
	{
		if (selected)
		{
			return;
		}
		pressed = false;
		float num = curPos - (float)curItem * windowSize;
		if (Mathf.Abs(num) > windowSize * 0.5f)
		{
			curItem = Mathf.RoundToInt(curPos / windowSize);
		}
		else if (Mathf.Abs(num) > minChangeDragInches * dpi)
		{
			curItem += ((num > 0f) ? 1 : (-1));
		}
		else
		{
			int value = Mathf.FloorToInt(vel / (swipeSpeedInchesPerSec * dpi));
			curItem += Mathf.Clamp(value, -1, 1);
		}
		if (cyclical)
		{
			if (curItem >= itemCount)
			{
				curItem %= itemCount;
			}
			else if (curItem < 0)
			{
				curItem = curItem % itemCount + itemCount;
			}
		}
		else
		{
			curItem = Mathf.Clamp(curItem, 0, itemCount - 1);
		}
		targetPos = (float)curItem * windowSize;
	}

	public void OnTap()
	{
		if (!selected)
		{
			complete = false;
			justCompleted = false;
			timeSinceSelection = 0f;
			selected = true;
			pressed = false;
		}
	}

	public void Move(float delta)
	{
		if (!selected)
		{
			curPos += delta;
		}
	}

	public void UpdateMenu()
	{
		if (selected)
		{
			justCompleted = false;
			if (!complete)
			{
				if (timeSinceSelection >= completionDuration)
				{
					complete = true;
					justCompleted = true;
				}
				else
				{
					timeSinceSelection += Time.deltaTime;
				}
			}
		}
		if (!pressed)
		{
			curPos = targetPos;
		}
		displayPos = Mathf.SmoothDamp(displayPos, curPos, ref dampVel, snapBackTime);
	}
}
