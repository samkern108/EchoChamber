using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostManager : MonoBehaviour, IRestartObserver, IPlayerObserver, IGhostDeathObserver {

	public GameObject p_ghost_air, p_ghost_ground;
	public static GhostManager instance;
	public List<GhostAI> children = new List<GhostAI> ();

	public void Start() {
		instance = this;
		NotificationMaster.restartObservers.Add (this);
		NotificationMaster.ghostDeathObservers.Add (this);
	}

	// TODO(samkern): Instead, when we spawn a new enemy or kill an enemy, we can remove its aggressiveness from a running total.
	public float TotalGhostAggressiveness() {
		float aggressiveness = 0;
		foreach (GhostAI ghost in children) {
			aggressiveness += ghost.stats.Aggressiveness ();
		}
		return aggressiveness;
	}

	public void SpawnGhost(GhostAIStats stats) {
		GameObject ghost;
		if(stats.airTime > .5f) {
			ghost = GameObject.Instantiate (p_ghost_air);
			ghost.AddComponent <GhostMovement_Fly>();
		}
		else {
			ghost = GameObject.Instantiate (p_ghost_ground);
			ghost.AddComponent <GhostMovement>();
		}

		ghost.transform.SetParent(transform);
		ghost.GetComponent <GhostAI>().Initialize(stats);
		children.Add (ghost.GetComponent <GhostAI>());
	}

	public void Restart() {
		children.Clear ();
		while (transform.childCount > 0) {
			Transform child = transform.GetChild (0);
			child.parent = null;
			Destroy (child.gameObject);
		}
	}

	public void PlayerDied() {
		foreach (GhostAI ai in children) {
			ai.enabled = false;
		}
	}

	public void GhostDied(GhostAIStats ghost) {
		children.Remove (ghost.self);
	}
}
