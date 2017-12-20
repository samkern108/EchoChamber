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
		transform.position = Room.GetRandomPointInRoomAvoidingPoints(new Vector2[]{transform.position, PlayerController.PlayerPosition});
		activated = false;
	}

	public void Restart() {
		RepositionExit ();
	}
}
