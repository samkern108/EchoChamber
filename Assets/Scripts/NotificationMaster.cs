using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRestartObserver
{
	void Restart();
}

public interface IPlayerObserver
{
	void PlayerDied();
}

public class NotificationMaster : MonoBehaviour {

	public static List<IRestartObserver> restartObservers = new List<IRestartObserver> ();
	public static List<IPlayerObserver> playerObservers = new List<IPlayerObserver> ();

	public static void SendRestartNotification() {
		foreach(IRestartObserver o in restartObservers) {
			o.Restart ();
		}
	}

	public static void SendPlayerDeathNotification() {
		foreach(IPlayerObserver o in playerObservers) {
			o.PlayerDied ();
		}
	}
}
