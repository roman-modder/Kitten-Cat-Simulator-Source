using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/SwipeMenuJS")]
public class SwipeMenuJS : MonoBehaviour
{
	public float snapBackTime;

	public float completionDuration;

	public bool cyclical;

	private float targetPos;

	public float swipeSpeedInchesPerSec;

	public float minChangeDragInches;

	public float windowSize;

	public float curPos;

	public float displayPos;

	private float dpi;

	private float dampVel;

	public int curItem;

	public int itemCount;

	private bool pressed;

	private bool selected;

	private bool complete;

	private bool justCompleted;

	private float timeSinceSelection;

	public SwipeMenuJS()
	{
		snapBackTime = 0.2f;
		completionDuration = 1f;
		cyclical = true;
		swipeSpeedInchesPerSec = 2f;
		minChangeDragInches = 1f;
		windowSize = 100f;
		dpi = 72f;
	}

	public virtual void Init(int itemCount, int curItem, float windowSize, float dpi, bool cyclical)
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

	public virtual void SetWindowSize(float windowSize, float dpi)
	{
		if (this.windowSize != windowSize || this.dpi != dpi)
		{
			displayPos = (targetPos = (float)curItem * this.windowSize);
			this.windowSize = windowSize;
			this.dpi = dpi;
			pressed = false;
		}
	}

	public virtual int GetCurItem()
	{
		return curItem;
	}

	public virtual bool Completed()
	{
		return complete;
	}

	public virtual bool JustCompleted()
	{
		return justCompleted;
	}

	public virtual bool Selected()
	{
		return selected;
	}

	public virtual bool Pressed()
	{
		return pressed;
	}

	public virtual float GetTimeSinceSelection()
	{
		return timeSinceSelection;
	}

	public virtual void OnPress()
	{
		if (!selected)
		{
			pressed = true;
		}
	}

	public virtual void OnRelease(float vel)
	{
		if (selected)
		{
			return;
		}
		pressed = false;
		float num = curPos - (float)curItem * windowSize;
		if (!(Mathf.Abs(num) <= windowSize * 0.5f))
		{
			curItem = Mathf.RoundToInt(curPos / windowSize);
		}
		else if (!(Mathf.Abs(num) <= minChangeDragInches * dpi))
		{
			curItem += ((!(num <= 0f)) ? 1 : (-1));
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

	public virtual void OnTap()
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

	public virtual void Move(float delta)
	{
		if (!selected)
		{
			curPos += delta;
		}
	}

	public virtual void UpdateMenu()
	{
		if (selected)
		{
			justCompleted = false;
			if (!complete)
			{
				if (!(timeSinceSelection < completionDuration))
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

	public virtual void Main()
	{
	}
}
