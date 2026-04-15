using UnityEngine;

public class ModeSurvival : MonoBehaviour
{
	public GameObject followerTemplate;

	public GameObject[] follower;

	public GameObject target;

	public Transform[] spawnPoints;

	public Logic logic;

	private float timeLeft;

	private FollowPlayer[] scFollower;

	private void Start()
	{
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			spawnPoints[i].gameObject.SetActive(false);
		}
		if (!PhotonNetwork.offlineMode && !PhotonNetwork.isMasterClient)
		{
			return;
		}
		int num = PlayerPrefs.GetInt("HumanCnt");
		follower = new GameObject[num];
		scFollower = new FollowPlayer[num];
		for (int j = 0; j < 3; j++)
		{
			for (int k = 0; k < spawnPoints.Length; k++)
			{
				Transform transform = spawnPoints[k];
				int num2 = Random.Range(k, spawnPoints.Length);
				spawnPoints[k] = spawnPoints[num2];
				spawnPoints[num2] = transform;
			}
		}
		for (int l = 0; l < num; l++)
		{
			int num3 = l;
			Vector3 position = followerTemplate.transform.position;
			position.x = spawnPoints[num3].position.x;
			position.z = spawnPoints[num3].position.z;
			if (PhotonNetwork.offlineMode)
			{
				follower[l] = Object.Instantiate(followerTemplate, position, Quaternion.identity);
			}
			else
			{
				follower[l] = PhotonNetwork.InstantiateSceneObject("Follower", position, Quaternion.identity, 0, null);
			}
			follower[l].SetActive(true);
			scFollower[l] = follower[l].GetComponentInChildren<FollowPlayer>();
		}
	}

	private void Update()
	{
		timeLeft += Time.deltaTime;
		if (timeLeft > 30f)
		{
			logic.AddScore(100);
			timeLeft = 0f;
		}
		if (scFollower == null)
		{
			scFollower = Object.FindObjectsOfType<FollowPlayer>();
		}
		if (!PhotonNetwork.offlineMode && !PhotonNetwork.isMasterClient)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < PlayerPrefs.GetInt("HumanCnt"); i++)
		{
			if (scFollower[i].isOver)
			{
				flag = true;
				scFollower[i].isOver = false;
			}
		}
		if (flag)
		{
			logic.GameOver();
		}
	}
}
