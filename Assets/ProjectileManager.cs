using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour, IRestartObserver {

	// Can we recycle projectiles?

	void Start () {
		NotificationMaster.restartObservers.Add (this);
	}

	public void Restart() {
	}
}
