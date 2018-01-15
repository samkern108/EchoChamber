using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class RiftManager : MonoBehaviour, IRestartObserver, ICheckpointObserver {

	public GameObject p_Rift;

	private float spawnDelay = 0.0f;
	private float timer = 0.0f;
	public static int riftCount;

	public void Start() {
		NotificationMaster.restartObservers.Add (this);
		NotificationMaster.checkpointObservers.Add (this);
	}
		
	public void Restart() {
		riftCount = 0;
		spawnDelay = 0.0f;
	}

	public void CheckpointActivated(float size) {
		riftCount--;
	}

	private void SpawnNewRift() {
		riftCount++;
		Instantiate (p_Rift, transform);
	}

	public void Update() {
		timer += Time.deltaTime;
		if(timer >= spawnDelay) {
			SpawnNewRift ();
			spawnDelay = Random.Range (1.5f * (riftCount / 2), 3.0f * (riftCount / 2));
			timer = 0.0f;
		}
	}
}
