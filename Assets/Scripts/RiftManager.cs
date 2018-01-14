using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class RiftManager : MonoBehaviour, IRestartObserver {

	//TODO(samkern): If there are too few rifts open, we should maybe speed up spawning them?

	public GameObject p_Rift;

	private float spawnDelay = 0.0f;
	private float timer = 0.0f;

	public void Start() {
		NotificationMaster.restartObservers.Add (this);
	}

	private void SpawnNewRift() {
		Instantiate (p_Rift, transform);
	}

	public void Restart() {
		spawnDelay = 0.0f;
	}

	public void Update() {
		timer += Time.deltaTime;
		if(timer >= spawnDelay) {
			SpawnNewRift ();
			spawnDelay = Random.Range (2.5f, 4.0f);
			timer = 0.0f;
		}
	}
}
