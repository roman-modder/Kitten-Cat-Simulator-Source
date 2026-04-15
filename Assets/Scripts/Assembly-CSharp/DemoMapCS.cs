using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoMapCS")]
public class DemoMapCS : MonoBehaviour
{
	public TouchController touchCtrl;

	public Texture2D mapTexture;

	public float mapWidth = 1800f;

	public float mapHeight = 1050f;

	public float mapMinScale = 0.1f;

	public float mapMaxScale = 20f;

	public float smoothingTime = 0.15f;

	public bool keepMapInside = true;

	public float marginFactor = 0.4f;

	public GUISkin guiSkin;

	public PopupBoxCS popupBox;

	private Vector2 mapOfs;

	private float mapScale;

	private float mapAngle;

	private Vector2 displayOfs;

	private float displayScale;

	private float displayAngle;

	private Matrix4x4 guiMatrix;

	public Rect debugBox;

	public Rect debugSafeRect;

	private const string CAPTION_COLOR_BEGIN = "<color='#FF0000'>";

	private const string CAPTION_COLOR_END = "</color>";

	private const string INSTRUCTIONS_TITLE = "Inctructions";

	private const string INSTRUCTIONS_BUTTON_TEXT = "";

	private const string INSTRUCTIONS_TEXT = "<color='#FF0000'>* Map Pan.\n</color>Drag to pan the map.\n\n<color='#FF0000'>* Map Zoom.\n</color>Place two fingers on the screen and spread them away to zoom out or pinch to zoom in.\n\n<color='#FF0000'>* Map Rotation.\n</color>Place two fingers on the screen and twist them to rotate the map.\n\n<color='#FF0000'>* Point Zoom-in.\n</color>Double tap with one finger on the map to zoom to that point.\n\n<color='#FF0000'>* Point Zoom-out.\n</color>Double tap with TWO fingers to zoom out.";

	private void SnapDisplayTransform()
	{
		displayOfs = mapOfs;
		displayAngle = mapAngle;
		displayScale = mapScale;
	}

	private void Start()
	{
		if (touchCtrl == null)
		{
			Debug.LogError("Touch Controller not assigned!!");
			return;
		}
		if (mapTexture == null)
		{
			Debug.LogError("Map texture not assigned!");
			return;
		}
		mapScale = 1f;
		mapAngle = 0f;
		mapOfs = new Vector2((float)Screen.width / 2f - mapWidth / 2f, (float)Screen.height / 2f - mapHeight / 2f);
		SnapDisplayTransform();
		touchCtrl.InitController();
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			DemoSwipeMenuCS.LoadMenuScene();
			return;
		}
		if (popupBox != null)
		{
			if (!popupBox.IsVisible())
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					popupBox.Show("Inctructions", "<color='#FF0000'>* Map Pan.\n</color>Drag to pan the map.\n\n<color='#FF0000'>* Map Zoom.\n</color>Place two fingers on the screen and spread them away to zoom out or pinch to zoom in.\n\n<color='#FF0000'>* Map Rotation.\n</color>Place two fingers on the screen and twist them to rotate the map.\n\n<color='#FF0000'>* Point Zoom-in.\n</color>Double tap with one finger on the map to zoom to that point.\n\n<color='#FF0000'>* Point Zoom-out.\n</color>Double tap with TWO fingers to zoom out.", string.Empty);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Space))
			{
				popupBox.Hide();
			}
		}
		TouchZone zone = touchCtrl.GetZone(0);
		if (zone.MultiPressed(false, true))
		{
			Vector2 multiPos = zone.GetMultiPos(TouchCoordSys.SCREEN_PX);
			if (zone.Pinched())
			{
				ScaleMap(zone.GetPinchRelativeScale(false), multiPos);
			}
			if (zone.Twisted())
			{
				RotateMap(zone.GetTwistDelta(false), multiPos);
			}
		}
		else
		{
			if (zone.UniPressed(false, true) && zone.UniDragged())
			{
				Vector2 uniDragDelta = zone.GetUniDragDelta(TouchCoordSys.SCREEN_PX, false);
				SetMapOffset(mapOfs + uniDragDelta);
			}
			if (zone.JustMultiDoubleTapped())
			{
				ScaleMap(0.5f, zone.GetMultiTapPos(TouchCoordSys.SCREEN_PX));
			}
			else if (zone.JustDoubleTapped())
			{
				ScaleMap(2f, zone.GetTapPos(TouchCoordSys.SCREEN_PX));
			}
		}
		if (keepMapInside)
		{
			marginFactor = Mathf.Clamp(marginFactor, 0f, 0.5f);
			Rect rect = new Rect((float)Screen.width * marginFactor, (float)Screen.height * marginFactor, (float)Screen.width * (1f - 2f * marginFactor), (float)Screen.height * (1f - 2f * marginFactor));
			Rect mapBoundingBox = GetMapBoundingBox();
			if (mapBoundingBox.xMax < rect.xMin)
			{
				mapOfs.x -= mapBoundingBox.xMax - rect.xMin;
			}
			else if (mapBoundingBox.xMin > rect.xMax)
			{
				mapOfs.x -= mapBoundingBox.xMin - rect.xMax;
			}
			if (mapBoundingBox.yMax < rect.yMin)
			{
				mapOfs.y -= mapBoundingBox.yMax - rect.yMin;
			}
			else if (mapBoundingBox.yMin > rect.yMax)
			{
				mapOfs.y -= mapBoundingBox.yMin - rect.yMax;
			}
		}
		if (Time.deltaTime >= smoothingTime)
		{
			SnapDisplayTransform();
			return;
		}
		float t = Time.deltaTime / smoothingTime;
		displayOfs = Vector2.Lerp(displayOfs, mapOfs, t);
		displayScale = Mathf.Lerp(displayScale, mapScale, t);
		displayAngle = Mathf.Lerp(displayAngle, mapAngle, t);
	}

	private void OnGUI()
	{
		GUI.skin = guiSkin;
		Matrix4x4 matrix = GUI.matrix;
		GUI.matrix = GetMapDisplayMatrix();
		GUI.DrawTexture(new Rect(0f, 0f, mapWidth, mapHeight), mapTexture);
		GUI.matrix = GetMarkerMatrix();
		GUI.Button(GetMarkerRect(new Vector2(900f, 400f), new Vector2(50f, 50f)), "HEY!!");
		GUI.matrix = matrix;
		if (popupBox != null && popupBox.IsVisible())
		{
			popupBox.DrawGUI();
			return;
		}
		GUI.color = Color.white;
		GUI.Label(new Rect(10f, 10f, Screen.width - 100, 100f), "Map Demo - Press [Space] for help, [Esc] to quit.");
	}

	private void RotateMap(float angleDelta, Vector2 pivotPos)
	{
		Vector3 vector = Quaternion.Euler(0f, 0f, 0f - angleDelta) * (mapOfs - pivotPos);
		mapOfs.x = pivotPos.x + vector.x;
		mapOfs.y = pivotPos.y + vector.y;
		SetMapOffset(mapOfs);
		mapAngle -= angleDelta;
	}

	private void ScaleMap(float relativeScale, Vector2 pivotPos)
	{
		float num = mapScale;
		SetScale(mapScale * relativeScale);
		SetMapOffset(pivotPos + (mapOfs - pivotPos) * (mapScale / num));
	}

	private void SetScale(float scale)
	{
		mapScale = Mathf.Clamp(scale, mapMinScale, mapMaxScale);
	}

	private void SetMapOffset(Vector2 ofs)
	{
		mapOfs.x = ofs.x;
		mapOfs.y = ofs.y;
	}

	private Matrix4x4 GetMapTargetMatrix()
	{
		return Matrix4x4.TRS(mapOfs, Quaternion.Euler(0f, 0f, mapAngle), new Vector3(mapScale, mapScale, mapScale));
	}

	private Matrix4x4 GetMapDisplayMatrix()
	{
		return Matrix4x4.TRS(displayOfs, Quaternion.Euler(0f, 0f, displayAngle), new Vector3(displayScale, displayScale, displayScale));
	}

	private Matrix4x4 GetMarkerMatrix()
	{
		return Matrix4x4.TRS(displayOfs, Quaternion.Euler(0f, 0f, displayAngle), Vector3.one);
	}

	private Rect GetMarkerRect(Vector2 pos, Vector2 size, Vector2 markerRelAnchor)
	{
		return new Rect(pos.x * displayScale - size.x * markerRelAnchor.x, pos.y * displayScale - size.y * markerRelAnchor.y, size.x, size.y);
	}

	private Rect GetMarkerRect(Vector2 pos, Vector2 size)
	{
		return GetMarkerRect(pos, size, new Vector2(0.5f, 0.5f));
	}

	private Rect GetMapBoundingBox()
	{
		Matrix4x4 mapTargetMatrix = GetMapTargetMatrix();
		Bounds bounds = new Bounds(mapTargetMatrix.MultiplyPoint3x4(new Vector3(0f, 0f, 0f)), Vector3.zero);
		bounds.Encapsulate(mapTargetMatrix.MultiplyPoint3x4(new Vector3(mapWidth, 0f, 0f)));
		bounds.Encapsulate(mapTargetMatrix.MultiplyPoint3x4(new Vector3(0f, mapHeight, 0f)));
		bounds.Encapsulate(mapTargetMatrix.MultiplyPoint3x4(new Vector3(mapWidth, mapHeight, 0f)));
		return new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
	}
}
