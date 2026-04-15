using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Image Effects/Camera Info")]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraInfo : MonoBehaviour
{
	public DepthTextureMode currentDepthMode;

	public RenderingPath currentRenderPath;

	public int recognizedPostFxCount;

	public virtual void Main()
	{
	}
}
