using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRestartObserver
{
	void Restart();
}

public class NotificationMaster : MonoBehaviour {

	public static List<IRestartObserver> restartObservers = new List<IRestartObserver> ();

	public void SendRestartNotification() {
		foreach(IRestartObserver ro in restartObservers) {
			ro.Restart ();
		}
	}
}
