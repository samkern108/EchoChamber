using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class RiftOld : MonoBehaviour, IRestartObserver, IGhostDeathObserver, IPlayerShootObserver {

	GhostAIStats ghostStats;

	// TODO(samkern): Should rifts get bigger over time? Spawn little enemies? How should we close them?
	// TODO(samkern): Idea: maybe if shit's really hitting the fan, a checkpoint has a random chance of spawning a friendly ghost? Or like, if you catch it really early it becomes friendly?

	private bool activated = false;

	private float size = 0.08f;
	private float timeOpened;

	//private Animate animate;

	public void Start() {
		timeOpened = Time.time;

		ghostStats = new GhostAIStats ();

		NotificationMaster.restartObservers.Add (this);
		NotificationMaster.ghostDeathObservers.Add (this);
		NotificationMaster.playerShootObservers.Add (this);

		//animate = GetComponent <Animate>();

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
			newSize = transform.localScale + new Vector3 (.1f, .1f, 0);
			//animate.AnimateToSize (transform.localScale, newSize, 1f);
			// TODO(samkern): Figure out an appropriate scaling measure between rift & ghost
			size += .03f;
			delay = Random.Range (3.0f, 8.0f);
			yield return new WaitForSeconds(delay);
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (!activated) {
			ghostStats.timeOpen = Time.time - timeOpened;
			ghostStats.size = size;

			// TODO(samkern): Should we record ALL ghosts killed while the rift is active, or just some? Should they decay over time?
			// ghosts alive during this rift's time in the level = ghosts currently alive plus ghosts murdered.
			ghostStats.totalGhostsInLevel = (GhostManager.instance.children.Count + ghostStats.ghostsKilled);
			ghostStats.totalGhostAggressiveness = GhostManager.instance.TotalGhostAggressiveness ();

			GhostManager.instance.SpawnGhost (ghostStats);

			NotificationMaster.SendCheckpointReachedNotification (Time.time - timeOpened);
			AudioManager.PlayDotPickup ();
			Destroy (this.gameObject);
		}
	}

	public void Restart() {
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
