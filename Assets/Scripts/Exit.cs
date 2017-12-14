using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour, IRestartObserver {

	private bool activated = false;

	public void Start() {
		RepositionExit ();
		NotificationMaster.restartObservers.Add (this);
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (!activated) {
			AudioManager.PlayDotPickup ();
			activated = true;
			EnemyManager.self.SpawnEnemy ();
			RepositionExit ();
			UIManager.instance.IncrementExits ();
		}
	}

	void RepositionExit() {
		transform.position = Room.GetRandomPointInRoom();
		activated = false;
	}

	public void Restart() {
		RepositionExit ();
	}
}
