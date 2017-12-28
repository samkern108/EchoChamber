using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class Rift : MonoBehaviour {

	// TODO(samkern): Should rifts get bigger over time? Spawn little enemies? How should we close them?
	// TODO(samkern): Idea: maybe if shit's really hitting the fan, a checkpoint has a random chance of spawning a friendly ghost? Or like, if you catch it really early it becomes friendly?

	private bool activated = false;

	private Animate animate;

	public void Start() {
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
		while(true) {
			animate.AnimateToSize (transform.localScale, transform.localScale + new Vector3(.5f, .5f, 0), 1f);
			yield return new WaitForSeconds(4.0f);
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (!activated) {
			NotificationMaster.SendCheckpointReachedNotification ();
			AudioManager.PlayDotPickup ();
			Destroy (this.gameObject);
		}
	}
}
