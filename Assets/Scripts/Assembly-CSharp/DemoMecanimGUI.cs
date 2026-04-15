using Photon;
using UnityEngine;

public class DemoMecanimGUI : PunBehaviour
{
	public GUISkin Skin;

	private PhotonAnimatorView m_AnimatorView;

	private Animator m_RemoteAnimator;

	private float m_SlideIn;

	private float m_FoundPlayerSlideIn;

	private bool m_IsOpen;

	public void Awake()
	{
	}

	public void Update()
	{
		FindRemoteAnimator();
		m_SlideIn = Mathf.Lerp(m_SlideIn, (!m_IsOpen) ? 0f : 1f, Time.deltaTime * 9f);
		m_FoundPlayerSlideIn = Mathf.Lerp(m_FoundPlayerSlideIn, (!(m_AnimatorView == null)) ? 1f : 0f, Time.deltaTime * 5f);
	}

	public void FindRemoteAnimator()
	{
		if (m_RemoteAnimator != null)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PhotonView component = array[i].GetComponent<PhotonView>();
			if (component != null && !component.isMine)
			{
				m_RemoteAnimator = array[i].GetComponent<Animator>();
			}
		}
	}

	public void OnGUI()
	{
		GUI.skin = Skin;
		string[] texts = new string[3] { "Disabled", "Discrete", "Continuous" };
		GUILayout.BeginArea(new Rect((float)Screen.width - 200f * m_FoundPlayerSlideIn - 400f * m_SlideIn, 0f, 600f, Screen.height), GUI.skin.box);
		GUILayout.Label("Mecanim Demo", GUI.skin.customStyles[0]);
		GUI.color = Color.white;
		string text = "Settings";
		if (m_IsOpen)
		{
			text = "Close";
		}
		if (GUILayout.Button(text, GUILayout.Width(110f)))
		{
			m_IsOpen = !m_IsOpen;
		}
		string text2 = string.Empty;
		if (m_AnimatorView != null)
		{
			text2 += "Send Values:\n";
			for (int i = 0; i < m_AnimatorView.GetSynchronizedParameters().Count; i++)
			{
				PhotonAnimatorView.SynchronizedParameter synchronizedParameter = m_AnimatorView.GetSynchronizedParameters()[i];
				try
				{
					switch (synchronizedParameter.Type)
					{
					case PhotonAnimatorView.ParameterType.Bool:
					{
						string text3 = text2;
						text2 = text3 + synchronizedParameter.Name + " (" + ((!m_AnimatorView.GetComponent<Animator>().GetBool(synchronizedParameter.Name)) ? "False" : "True") + ")\n";
						break;
					}
					case PhotonAnimatorView.ParameterType.Int:
					{
						string text3 = text2;
						text2 = text3 + synchronizedParameter.Name + " (" + m_AnimatorView.GetComponent<Animator>().GetInteger(synchronizedParameter.Name) + ")\n";
						break;
					}
					case PhotonAnimatorView.ParameterType.Float:
					{
						string text3 = text2;
						text2 = text3 + synchronizedParameter.Name + " (" + m_AnimatorView.GetComponent<Animator>().GetFloat(synchronizedParameter.Name).ToString("0.00") + ")\n";
						break;
					}
					}
				}
				catch
				{
					Debug.Log("derrrr for " + synchronizedParameter.Name);
				}
			}
		}
		if (m_RemoteAnimator != null)
		{
			text2 += "\nReceived Values:\n";
			for (int j = 0; j < m_AnimatorView.GetSynchronizedParameters().Count; j++)
			{
				PhotonAnimatorView.SynchronizedParameter synchronizedParameter2 = m_AnimatorView.GetSynchronizedParameters()[j];
				try
				{
					switch (synchronizedParameter2.Type)
					{
					case PhotonAnimatorView.ParameterType.Bool:
					{
						string text3 = text2;
						text2 = text3 + synchronizedParameter2.Name + " (" + ((!m_RemoteAnimator.GetBool(synchronizedParameter2.Name)) ? "False" : "True") + ")\n";
						break;
					}
					case PhotonAnimatorView.ParameterType.Int:
					{
						string text3 = text2;
						text2 = text3 + synchronizedParameter2.Name + " (" + m_RemoteAnimator.GetInteger(synchronizedParameter2.Name) + ")\n";
						break;
					}
					case PhotonAnimatorView.ParameterType.Float:
					{
						string text3 = text2;
						text2 = text3 + synchronizedParameter2.Name + " (" + m_RemoteAnimator.GetFloat(synchronizedParameter2.Name).ToString("0.00") + ")\n";
						break;
					}
					}
				}
				catch
				{
					Debug.Log("derrrr for " + synchronizedParameter2.Name);
				}
			}
		}
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
		gUIStyle.alignment = TextAnchor.UpperLeft;
		GUI.color = new Color(1f, 1f, 1f, 1f - m_SlideIn);
		GUI.Label(new Rect(10f, 100f, 600f, Screen.height), text2, gUIStyle);
		if (m_AnimatorView != null)
		{
			GUI.color = new Color(1f, 1f, 1f, m_SlideIn);
			GUILayout.Space(20f);
			GUILayout.Label("Synchronize Parameters");
			for (int k = 0; k < m_AnimatorView.GetSynchronizedParameters().Count; k++)
			{
				GUILayout.BeginHorizontal();
				PhotonAnimatorView.SynchronizedParameter synchronizedParameter3 = m_AnimatorView.GetSynchronizedParameters()[k];
				GUILayout.Label(synchronizedParameter3.Name, GUILayout.Width(100f), GUILayout.Height(36f));
				int synchronizeType = (int)synchronizedParameter3.SynchronizeType;
				int num = GUILayout.Toolbar(synchronizeType, texts);
				if (num != synchronizeType)
				{
					m_AnimatorView.SetParameterSynchronized(synchronizedParameter3.Name, synchronizedParameter3.Type, (PhotonAnimatorView.SynchronizeType)num);
				}
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndArea();
	}

	public override void OnJoinedRoom()
	{
		CreatePlayerObject();
	}

	private void CreatePlayerObject()
	{
		Vector3 position = new Vector3(-2f, 0f, 0f);
		position.x += Random.Range(-3f, 3f);
		position.z += Random.Range(-4f, 4f);
		GameObject gameObject = PhotonNetwork.Instantiate("Robot Kyle Mecanim", position, Quaternion.identity, 0);
		m_AnimatorView = gameObject.GetComponent<PhotonAnimatorView>();
	}
}
