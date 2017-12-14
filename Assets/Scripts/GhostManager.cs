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

	public void StartCapturingNewGhost(float[] ghostPositions, float[] shootTimes) {

		// Create new ghost
		GameObject ghost = GameObject.Instantiate (p_ghost);
		ghost.transform.SetParent(transform);
		ghost.GetComponent <Ghost>().Initialize(ghostPositions, shootTimes);
		children.Add (ghost.GetComponent <Ghost>());

		// Restart ghost routines
		foreach (Ghost child in children) {
			child.EnactRoutine ();
		}
	}

	// TODO(samkern): Will these be out of order?
	/*public void RegisterAction(GhostAction action) {
		int index = actions.Count;
		if (actions [index - 1].time > action.time) {
			index--;
		}
		actions.Insert (index, action);
	}*/

	public void Restart() {
		while (transform.childCount > 0) {
			Transform child = transform.GetChild (0);
			child.parent = null;
			Destroy (child.gameObject);
		}
	}
}
