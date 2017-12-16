using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IRestartObserver, IPlayerObserver {

	public static EnemyManager self;
	public GameObject p_enemy;
	public Room room;

	public void Start() {
		self = this;
		NotificationMaster.restartObservers.Add (this);
		NotificationMaster.playerObservers.Add (this);
	}

	public void SpawnEnemy() {
		GameObject enemy = GameObject.Instantiate (p_enemy);
		enemy.transform.SetParent (this.transform);
	}

	private static int _numEnemies;
	public int NumEnemies
	{
		get { Debug.Log (transform.childCount); 
			return transform.childCount; }
	}

	public void Restart() {
		DestroyAllChildren ();
	}

	public void PlayerDied() {
		DestroyAllChildren ();
	}

	private void DestroyAllChildren() {
		while (transform.childCount > 0) {
			Transform child = transform.GetChild (0);
			child.parent = null;
			Destroy (child.gameObject);
		}
	}
}
