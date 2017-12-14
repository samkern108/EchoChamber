using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance;

	public static AudioClip projectileShoot, projectileExplode, 
	playerBoostCharge, playerBoostRelease, playerWallHit, playerTurn, playerJump, playerDeath,
	levelComplete, gameStart, 
	dotPickup;

	private static GameObject sourceTemplate;
	private static int activeSources = 0;
	private static List<AudioSource> freeSources = new List<AudioSource>();

	// ## Initialization >> //

	// Member initialization
	public void Awake() {
		instance = this;

		sourceTemplate = new GameObject("SourceTemplate");
		sourceTemplate.AddComponent <AudioSource>();
		sourceTemplate.transform.SetParent (this.transform);

		Initialize ();
	}

	// Static initialization
	public static void Initialize () {
		projectileShoot = ResourceLoader.LoadAudioClip ("Shoot");
		projectileExplode = ResourceLoader.LoadAudioClip ("Projectile Hit Wall");
		playerBoostCharge = ResourceLoader.LoadAudioClip ("Player Boost Charge");
		playerBoostRelease = ResourceLoader.LoadAudioClip ("Player Boost 6");
		playerWallHit = ResourceLoader.LoadAudioClip ("Player Wall Hit");
		playerTurn = ResourceLoader.LoadAudioClip ("Player Turn");
		playerJump = ResourceLoader.LoadAudioClip ("Player Move 2");
		playerDeath = ResourceLoader.LoadAudioClip ("Player Death");
		levelComplete = ResourceLoader.LoadAudioClip ("Level Complete");
		gameStart = ResourceLoader.LoadAudioClip ("Game Start");
		dotPickup = ResourceLoader.LoadAudioClip ("Dot Pickup");
	}

	// << Initialization ## //

	// ## Audio Source Handling >> //

	private static AudioSource GetFreeAudioSource(float recycleTime) {
		AudioSource source;
		if (freeSources.Count == 0) {
			GameObject obj = Instantiate (sourceTemplate);
			obj.transform.SetParent (AudioManager.instance.transform);
			activeSources++;
			source = obj.GetComponent <AudioSource>();
		}
		else {
			// Race condition?
			source = freeSources[0];
			freeSources.RemoveAt (0);
			// TODO: Figure out a better way to determine if sources can be removed.
			if (freeSources.Count >= (int)Mathf.Ceil (activeSources / 2)) {
				freeSources.RemoveRange (0, (int)Mathf.Floor (activeSources / 2));
			}
		}
		Timing.RunCoroutine (RecycleSource(recycleTime + .1f, source));
		return source;
	}

	private static IEnumerator<float> RecycleSource (float wait, AudioSource source) {
		yield return wait;
		source.pitch = 1.0f;
		freeSources.Add (source);
	}

	// << Audio Source Handling ## //
		
	// ## Player SFX >> //

	public static void PlayEnemyDeath() {
		AudioSource source = GetFreeAudioSource (dotPickup.length);
		source.volume = .2f;
		source.pitch = Random.Range (1f, 1.3f);
		source.PlayOneShot (playerDeath);
	}

	public static void PlayPlayerDeath() {
		AudioSource source = GetFreeAudioSource (dotPickup.length);
		source.volume = .6f;
		source.pitch = Random.Range (.8f, 1.1f);
		source.PlayOneShot (playerDeath);
	}
		
	public static void PlayPlayerJump() {
		AudioSource source = GetFreeAudioSource (playerJump.length);
		source.volume = .9f;
		source.pitch = Random.Range (.8f, 1.1f);
		source.PlayOneShot (playerJump);
	}

	/*private static bool playerTurnSafeToPlay = true;
	private void PlayerTurnSafeToPlay() {
		playerTurnSafeToPlay = true;
	}*/

	public static void PlayPlayerTurn() {
		//if (!playerTurnSafeToPlay) return;
		//playerTurnSafeToPlay = false;
		AudioSource source = GetFreeAudioSource (dotPickup.length);
		source.volume = Random.Range(.2f, .3f);
		source.pitch = Random.Range(.6f, .8f);
		source.PlayOneShot (playerTurn);
		//AudioManager.instance.Invoke ("PlayerTurnSafeToPlay", playerTurn.length/3);
	}

	// << Player SFX ## //

	// ## Enemy SFX >> //

	public static void PlayEnemyShoot() {
		AudioSource source = GetFreeAudioSource(projectileShoot.length);
		source.volume = Random.Range (.4f, .8f);
		source.pitch = Random.Range (1.2f, 1.6f);
		source.PlayOneShot (projectileShoot);
	}

	public static void PlayShoot() {
		AudioSource source = GetFreeAudioSource(projectileShoot.length);
		source.volume = Random.Range (.4f, .8f);
		source.pitch = Random.Range (.8f, 1.2f);
		source.PlayOneShot (projectileShoot);
	}

	public static void PlayProjectileExplode() {
		AudioSource source = GetFreeAudioSource(projectileExplode.length);
		source.volume = Random.Range (.2f, .4f);
		source.pitch = Random.Range (1.0f, 1.8f);
		source.PlayOneShot (projectileExplode);
	}

	// << Enemy SFX ## //

	public static void PlayDotPickup() {
		AudioSource source = GetFreeAudioSource (dotPickup.length);
		source.pitch = .75f;
		source.volume = .7f;
		source.PlayOneShot (dotPickup);
	}
}
