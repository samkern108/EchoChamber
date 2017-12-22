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
			NotificationMaster.SendCheckpointReachedNotification ();
			AudioManager.PlayDotPickup ();
			activated = true;
			RepositionExit ();
		}
	}

	void RepositionExit() {
		Vector3 point;
		float distance, minDistance;
		do {
			point = Room.GetRandomPointInRoom ();

			distance = Vector2.Distance (PlayerController.PlayerPosition, point);
			minDistance = 2.0f;
		} while (distance < minDistance);
			
		transform.position = point;
		activated = false;
	}

	public void Restart() {
		RepositionExit ();
	}
}
