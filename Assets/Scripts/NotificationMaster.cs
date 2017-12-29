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

public interface ICheckpointObserver
{
	void CheckpointActivated();
}

public interface IGhostDeathObserver
{
	void GhostDied(GhostAIStats ghost);
}

public class NotificationMaster : MonoBehaviour {

	public static List<IRestartObserver> restartObservers = new List<IRestartObserver> ();
	public static List<IPlayerObserver> playerObservers = new List<IPlayerObserver> ();
	public static List<ICheckpointObserver> checkpointObservers = new List<ICheckpointObserver> ();
	public static List<IGhostDeathObserver> ghostDeathObservers = new List<IGhostDeathObserver> ();

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

	public static void SendCheckpointReachedNotification() {
		foreach(ICheckpointObserver o in checkpointObservers) {
			o.CheckpointActivated ();
		}
	}

	public static void SendGhostDeathNotification(GhostAIStats stats) {
		foreach(IGhostDeathObserver o in ghostDeathObservers) {
			o.GhostDied (stats);
		}
	}
}
