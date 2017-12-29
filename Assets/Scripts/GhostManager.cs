using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostManager : MonoBehaviour, IRestartObserver, IPlayerObserver {

	public GameObject p_ghost;
	public static GhostManager instance;
	public List<GhostAI> children = new List<GhostAI> ();

	public void Start() {
		instance = this;
		NotificationMaster.restartObservers.Add (this);
	}

/*	public void StartCapturingNewGhost(float[] ghostPositions) {

		// Create new ghost
		GameObject ghost = GameObject.Instantiate (p_ghost);
		ghost.transform.SetParent(transform);
		ghost.GetComponent <Ghost>().Initialize(ghostPositions);
		children.Add (ghost.GetComponent <Ghost>());

		List<Ghost> ghostsToRemove = new List<Ghost> ();
		// Restart ghost routines
		foreach (Ghost child in children) {
			if (!child) {
				ghostsToRemove.Add (child);
				continue;
			}
			child.gameObject.SetActive (true);
			child.EnactRoutine ();
		}

		children = children.Except(ghostsToRemove).ToList();
	}*/

	public void StartCapturingNewGhost(GhostAIStats stats) {

		// Create new ghost
		GameObject ghost = GameObject.Instantiate (p_ghost);
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
}
