using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.BadWord
{
	public class GUISource : MonoBehaviour
	{
		public GameObject ItemPrefab;

		public GameObject Target;

		public Scrollbar Scroll;

		public GUIMain GuiMain;

		public int ColumnCount = 1;

		public Vector2 SpaceWidth = new Vector2(8f, 8f);

		public Vector2 SpaceHeight = new Vector2(8f, 8f);

		private int frameCounter;

		private bool isReady;

		private void Start()
		{
			StartCoroutine(buildLanguageList());
		}

		private void Update()
		{
			if (isReady && frameCounter < 2)
			{
				Scroll.value = 1f;
				frameCounter++;
			}
		}

		private IEnumerator buildLanguageList()
		{
			while (!MultiManager.Ready())
			{
				yield return null;
			}
			RectTransform rowRectTransform = ItemPrefab.GetComponent<RectTransform>();
			RectTransform containerRectTransform = Target.GetComponent<RectTransform>();
			for (int num = Target.transform.childCount - 1; num >= 0; num--)
			{
				Transform child = Target.transform.GetChild(num);
				child.SetParent(null);
				Object.Destroy(child.gameObject);
			}
			List<Source> items = MultiManager.Sources(ManagerMask.BadWord);
			float width = containerRectTransform.rect.width / (float)ColumnCount - (SpaceWidth.x + SpaceWidth.y) * (float)ColumnCount;
			float height = rowRectTransform.rect.height - (SpaceHeight.x + SpaceHeight.y);
			int rowCount = items.Count / ColumnCount;
			if (rowCount > 0 && items.Count % rowCount > 0)
			{
				rowCount++;
			}
			float scrollHeight = height * (float)rowCount;
			containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, (0f - scrollHeight) / 2f);
			containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2f);
			int j = 0;
			for (int i = 0; i < items.Count; i++)
			{
				if (i % ColumnCount == 0)
				{
					j++;
				}
				GameObject gameObject = Object.Instantiate(ItemPrefab);
				gameObject.name = Target.name + " item at (" + i + "," + j + ")";
				gameObject.transform.SetParent(Target.transform);
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponent<SourceEntry>().Source = items[i];
				gameObject.GetComponent<SourceEntry>().GuiMain = GuiMain;
				RectTransform component = gameObject.GetComponent<RectTransform>();
				float x = (0f - containerRectTransform.rect.width) / 2f + (width + SpaceWidth.x) * (float)(i % ColumnCount) + SpaceWidth.x * (float)ColumnCount;
				float y = containerRectTransform.rect.height / 2f - height * (float)j;
				component.offsetMin = new Vector2(x, y);
				x = component.offsetMin.x + width;
				y = component.offsetMin.y + height;
				component.offsetMax = new Vector2(x, y);
			}
			isReady = true;
			frameCounter = 0;
		}
	}
}
