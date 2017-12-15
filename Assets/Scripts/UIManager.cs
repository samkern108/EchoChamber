using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IRestartObserver {

	public Text exitsText, timerText;
	public static UIManager instance;

	private int exitsCompleted = 0;

	private bool stop = false;

	public void Start() {
		instance = this;
		NotificationMaster.restartObservers.Add (this);
	}

	public void Stop() {
		stop = true;
	}

	public void IncrementExits() {
		exitsCompleted++;
		exitsText.text = "" + exitsCompleted;
	}

	public void Restart() {
		exitsCompleted = 0;
		exitsText.text = "" + exitsCompleted;
	}
}
