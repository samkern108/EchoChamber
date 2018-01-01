using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IRestartObserver
{
	void Restart();
}

public interface IPlayerObserver
{
	void PlayerDied();
}

public interface IPlayerShootObserver
{
	void PlayerShoot();
}

public interface ICheckpointObserver
{
	void CheckpointActivated(float timeOpen);
}

public interface IGhostDeathObserver
{
	void GhostDied(GhostAIStats ghost);
}

// TODO(samkern): This absolutely doesn't scale. Go figure out how to make this better.
public class NotificationMaster : MonoBehaviour {

	public static List<IRestartObserver> restartObservers = new List<IRestartObserver> ();
	public static List<IPlayerObserver> playerObservers = new List<IPlayerObserver> ();
	public static List<ICheckpointObserver> checkpointObservers = new List<ICheckpointObserver> ();
	public static List<IGhostDeathObserver> ghostDeathObservers = new List<IGhostDeathObserver> ();
	public static List<IPlayerShootObserver> playerShootObservers = new List<IPlayerShootObserver> ();

	/* TODO(samkern): Possible simplification? go learn about delegates.
	public delegate void RestartDelegate();
	public static List<RestartDelegate> restartDelegates = new List<RestartDelegate>();
	public static void SendRestartNotification() {
		foreach (RestartDelegate del in restartDelegates) {
			del ();
		}
	}*/

	public static void SendRestartNotification() {
		List<IRestartObserver> toRemove = new List<IRestartObserver> ();
		foreach(IRestartObserver o in restartObservers) {
			if (o.Equals (null))
				toRemove.Add (o);
			else
				o.Restart ();
		}
		restartObservers = restartObservers.Except (toRemove).ToList();
	}

	/*public static void SendRestartNotification() {
		foreach(IRestartObserver o in restartObservers) {
			o.Restart ();
		}
	}*/

	public static void SendPlayerDeathNotification() {
		List<IPlayerObserver> toRemove = new List<IPlayerObserver>();
		foreach(IPlayerObserver o in playerObservers) {
			if (o.Equals (null))
				toRemove.Add (o);
			else
				o.PlayerDied ();
		}
		playerObservers = playerObservers.Except (toRemove).ToList ();
	}

	public static void SendPlayerShootNotification() {
		List<IPlayerShootObserver> toRemove = new List<IPlayerShootObserver> ();
		foreach(IPlayerShootObserver o in playerShootObservers) {
			if (o.Equals (null))
				toRemove.Add (o);
			else
				o.PlayerShoot ();
		}
		playerShootObservers = playerShootObservers.Except (toRemove).ToList ();
	}

	public static void SendCheckpointReachedNotification(float timeOpen) {
		List<ICheckpointObserver> toRemove = new List<ICheckpointObserver> ();
		foreach(ICheckpointObserver o in checkpointObservers) {
			if (o.Equals (null))
				toRemove.Add (o);
			else
				o.CheckpointActivated (timeOpen);
		}
		checkpointObservers = checkpointObservers.Except (toRemove).ToList ();
	}

	public static void SendGhostDeathNotification(GhostAIStats stats) {
		List<IGhostDeathObserver> toRemove = new List<IGhostDeathObserver> ();
		foreach(IGhostDeathObserver o in ghostDeathObservers) {
			if (o.Equals (null))
				toRemove.Add (o);
			else
				o.GhostDied (stats);
		}
		ghostDeathObservers = ghostDeathObservers.Except (toRemove).ToList ();
	}
}
