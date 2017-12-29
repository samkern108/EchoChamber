using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class Rift : MonoBehaviour, IRestartObserver {

	// TODO(samkern): Should rifts get bigger over time? Spawn little enemies? How should we close them?
	// TODO(samkern): Idea: maybe if shit's really hitting the fan, a checkpoint has a random chance of spawning a friendly ghost? Or like, if you catch it really early it becomes friendly?

	private bool activated = false;

	private Animate animate;

	public void Start() {
		NotificationMaster.restartObservers.Add (this);
		animate = GetComponent <Animate>();

		Vector3 point;
		float distance, minDistance;
		do {
			point = Room.GetRandomPointInRoom ();

			distance = Vector2.Distance (PlayerController.PlayerPosition, point);
			minDistance = 2.0f;
		} while (distance < minDistance);

		transform.position = point;

		StartCoroutine ("C_AnimateSize");
	}

	private IEnumerator C_AnimateSize () {
		float delay;
		while(true) {
			delay = Random.Range (3.0f, 8.0f);
			animate.AnimateToSize (transform.localScale, transform.localScale + new Vector3(.25f, .25f, 0), 1f);
			yield return new WaitForSeconds(delay);
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (!activated) {
			NotificationMaster.SendCheckpointReachedNotification ();
			AudioManager.PlayDotPickup ();
			animate.enabled = false;
			Destroy (this.gameObject);
		}
	}

	public void Restart() {
		this.enabled = false;
		animate.enabled = false;
		Destroy (this.gameObject);
	}
}
