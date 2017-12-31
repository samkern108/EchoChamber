using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IRestartObserver, ICheckpointObserver {

	public Text exitsText;
	public static UIManager instance;

	private int exitsCompleted = 0;

	public void Start() {
		instance = this;
		NotificationMaster.restartObservers.Add (this);
		NotificationMaster.checkpointObservers.Add (this);
	}

	public void Update() {
		if(Input.GetKeyDown(KeyCode.R)) {
			PressRestartButton ();
		}
		else if(Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit ();
		}
	}

	public void CheckpointActivated(float timeOpen) {
		exitsCompleted++;
		exitsText.text = "" + exitsCompleted;
	}

	public void Restart() {
		exitsCompleted = 0;
		exitsText.text = "" + exitsCompleted;
	}

	public void PressRestartButton() {
		NotificationMaster.SendRestartNotification ();
	}
}
