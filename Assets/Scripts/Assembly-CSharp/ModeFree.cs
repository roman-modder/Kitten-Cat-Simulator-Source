using UnityEngine;

public class ModeFree : MonoBehaviour
{
	public GameObject world;

	public Logic logic;

	private void Start()
	{
		InteractiveItem[] componentsInChildren = world.GetComponentsInChildren<InteractiveItem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].shouldReset = false;
		}
	}
}
