using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour, IRestartObserver {

	public GameObject p_ghost;
	public static GhostManager instance;
	public List<Ghost> children = new List<Ghost> ();

	public void Start() {
		instance = this;
		NotificationMaster.restartObservers.Add (this);
	}

	public void StartCapturingNewGhost(float[] ghostPositions) {

		// Create new ghost
		GameObject ghost = GameObject.Instantiate (p_ghost);
		ghost.transform.SetParent(transform);
		ghost.GetComponent <Ghost>().Initialize(ghostPositions);
		children.Add (ghost.GetComponent <Ghost>());

		// Restart ghost routines
		foreach (Ghost child in children) {
			child.EnactRoutine ();
		}
	}

	public void Restart() {
		children.Clear ();
		while (transform.childCount > 0) {
			Transform child = transform.GetChild (0);
			child.parent = null;
			Destroy (child.gameObject);
		}
	}
}
