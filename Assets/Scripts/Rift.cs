using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class Rift : MonoBehaviour, IRestartObserver, IGhostDeathObserver, IPlayerShootObserver {

	GhostAIStats ghostStats;

	// TODO(samkern): Should rifts get bigger over time? Spawn little enemies? How should we close them?
	// TODO(samkern): Idea: maybe if shit's really hitting the fan, a checkpoint has a random chance of spawning a friendly ghost? Or like, if you catch it really early it becomes friendly?

	private bool activated = false;

	private float size = 0.05f;
	private float timeOpened;

	private Animate animate;

	public void Start() {
		timeOpened = Time.time;

		ghostStats = new GhostAIStats ();
		ghostStats.Init ();
		ghostStats.totalGhostsInLevel = GhostManager.instance.children.Count;
		ghostStats.totalGhostAggressiveness = GhostManager.instance.TotalGhostAggressiveness ();

		NotificationMaster.restartObservers.Add (this);
		NotificationMaster.ghostDeathObservers.Add (this);
		NotificationMaster.playerShootObservers.Add (this);

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
		Vector3 newSize;
		float delay;
		while(true) {
			delay = Random.Range (3.0f, 8.0f);
			newSize = transform.localScale + new Vector3 (.25f, .25f, 0);
			animate.AnimateToSize (transform.localScale, newSize, 1f);
			// TODO(samkern): Figure out an appropriate scaling measure between rift & ghost
			size += .05f;
			yield return new WaitForSeconds(delay);
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (!activated) {
			ghostStats.timeOpen = Time.time - timeOpened;
			ghostStats.size = size;
			GhostManager.instance.StartCapturingNewGhost (ghostStats);

			NotificationMaster.SendCheckpointReachedNotification (Time.time - timeOpened);
			AudioManager.PlayDotPickup ();
			animate.enabled = false;

			Destroy (this.gameObject);
		}
	}

	public void Restart() {
		NotificationMaster.restartObservers.Remove (this);
		this.enabled = false;
		animate.enabled = false;
		Destroy (this.gameObject);
	}

	public void GhostDied(GhostAIStats stats) {
		ghostStats.ghostsKilled++;
		ghostStats.killedGhostAggressiveness += stats.Aggressiveness ();
	}

	public void PlayerShoot() {
		ghostStats.shotsFired++;
	}
}
