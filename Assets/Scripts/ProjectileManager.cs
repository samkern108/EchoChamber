using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour, IRestartObserver {

	public static Transform myTransform;

	// Can we recycle projectiles?

	void Start () {
		NotificationMaster.restartObservers.Add (this);
		myTransform = transform;
	}

	public void Restart() {
		while (transform.childCount > 0) {
			Transform child = transform.GetChild (0);
			child.parent = null;
			Destroy (child.gameObject);
		}
	}
}
